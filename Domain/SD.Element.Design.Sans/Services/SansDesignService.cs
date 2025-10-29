using SD.Core.Strand.Enum;
using SD.Element.Design.Sans.Models;
using SD.Fem.Strand7.Extensions;
using SD.Fem.Strand7.Interfaces;
using SD.Element.Design.Sans.Extensions;
using SD.Element.Design.Services;
using SD.Core.Shared.Models.Sans;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace SD.Element.Design.Sans.Services;
public class SansDesignService : SansService, IElementDesignService
{
    private readonly IDesignModel _designModel;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IStrandApiService _strandApiService;
    private readonly IFemModelParameters _femModelParameters;

    public SansDesignService(
        IDesignModel designModel,
        IUlsDesignResults ulsDesignResults,
        IStrandApiService strandApiService,
        IFemModelParameters femModelParameters)
    {
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
    }

    public async Task RunUlsDesignUpdate(int modelId, List<Beam> beams)
    {
        var updates = await GetElementUlsUtilization(modelId, beams, _femModelParameters, _designModel.DesignSettings.BeamMinStations);

        foreach (var update in updates)
        {
            var oldResult = _ulsDesignResults.SansUlsResults.FirstOrDefault(r => r.Beam.Number == update.Beam.Number && r.LoadCaseNumber == update.LoadCaseNumber);
            if (oldResult != null)
            {
                var index = _ulsDesignResults.SansUlsResults.IndexOf(oldResult);
                if (index != -1)
                    _ulsDesignResults.SansUlsResults[index] = update;
            }
        }
    }

    public async Task<IEnumerable<UlsResultPeak>> RunUlsDesign(int modelId, List<Beam> beams)
    {
        _ulsDesignResults.SansUlsResults = await GetElementUlsUtilization(modelId, beams, _femModelParameters, _designModel.DesignSettings.BeamMinStations);

        return _ulsDesignResults.SansUlsResults.ToUlsPeakResults();
    }

    private async Task<List<SansUlsResult>> GetElementUlsUtilization(int modelId, List<Beam> beams, IFemModelParameters femDesignParameters, int minStations)
    {
        var designSingleResult = true; // TODO: Determine whether or not this is required later.
        var isAsync = false;

        var sansUlsResults = new List<SansUlsResult>();
        if (femDesignParameters == null)
            return sansUlsResults;

        var designableBeams = beams.Where(beam => beam.CanDesign()).ToList();
        SetSansBeamResistances(designableBeams);

        var strand7BeamResults = _strandApiService.GetBeamResults(modelId, ResultType.Force, designableBeams, femDesignParameters.LoadCaseCombinations, minStations);

        var tasks = new List<Task>();
        foreach (var strand7BeamResult in strand7BeamResults)
        {
            if (isAsync)
            {
                tasks.Add(Task.Run(() =>
                {
                    GetBeamUlsUtilization(femDesignParameters, designSingleResult, sansUlsResults, strand7BeamResults, strand7BeamResult);
                }));
            }
            else
                GetBeamUlsUtilization(femDesignParameters, designSingleResult, sansUlsResults, strand7BeamResults, strand7BeamResult);
        }
        await Task.WhenAll(tasks);
        return sansUlsResults;
    }

    private static void SetSansBeamResistances(List<Beam> beams)
    {
        foreach (var beam in beams)
            beam.Resistance = new SansBeamResistance(beam);
    }

    private void GetBeamUlsUtilization(IFemModelParameters femDesignParameters, bool designSingleResult, List<SansUlsResult> allResults, List<StrandBeamResults> beamResults, StrandBeamResults beamResult)
    {
        var connectedResults = new List<StrandBeamResults>();
        var chain = designSingleResult ? [beamResult.Beam] : beamResult.Beam.BeamChain.ConnectedChaineTop;

        // We collect the beam results along the entire chain for SANS ULS checks
        foreach (var beam in chain)
        {
            var connected = beamResults.FirstOrDefault(br => br.Beam.Number == beam.Number);
            if (connected != null)
                connectedResults.Add(connected);
        }

        var result = GetSansBeamCapacity(beamResult, connectedResults, femDesignParameters.UnitFactor);
        if (result != null)
        {
            result.SetLoadCaseNumber(beamResult.LoadCaseId);
            allResults.Add(result);
        }
    }

    private SansUlsResult GetSansBeamCapacity(StrandBeamResults beamResult, List<StrandBeamResults> connectedResults, UnitFactor unitFactor)
    {
        var beamForces = GetAllPeakBeamResults(unitFactor, connectedResults);

        var bendingConstants = SetBeamStationParameters(unitFactor, beamResult);

        var designType = SharedDesignService.GetDesignType(beamResult.Beam, beamForces);

        return GetSansDesignUlsResult(beamForces, bendingConstants, beamResult, designType);
    }

    private static BendingConstants SetBeamStationParameters(UnitFactor unitFactor, StrandBeamResults results)
    {
        var quarterPosition = (results.NumStations / 4 - 1) * results.NumColumns;
        var halfPosition = (results.NumStations / 2 - 1) * results.NumColumns;
        var threeQuarterPosition = (results.NumStations * 3 / 4 - 1) * results.NumColumns;

        return new BendingConstants()
        {
            MuMajorQuarter = results.BeamRes[quarterPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMajorHalf = results.BeamRes[halfPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMajorThreeQuarter = results.BeamRes[threeQuarterPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMinorQuarter = results.BeamRes[quarterPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
            MuMinorHalf = results.BeamRes[halfPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
            MuMinorThreeQuarter = results.BeamRes[threeQuarterPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
        };
    }

    private static BeamForces GetAllPeakBeamResults(UnitFactor unitFactor, List<StrandBeamResults> results)
    {
        return new BeamForces
        {
            MaxAxialForce = results.MaxResult(BeamResultType.AxialForce) * unitFactor.Force,
            MinAxialForce = results.MinResult(BeamResultType.AxialForce) * unitFactor.Force,
            MaxMuMinor = results.MaxResult(BeamResultType.BendingMomentMinor) * unitFactor.Force * unitFactor.Length,
            MinMuMinor = results.MinResult(BeamResultType.BendingMomentMinor) * unitFactor.Force * unitFactor.Length,
            MaxMuMajor = results.MaxResult(BeamResultType.BendingMomentMajor) * unitFactor.Force * unitFactor.Length,
            MinMuMajor = results.MinResult(BeamResultType.BendingMomentMajor) * unitFactor.Force * unitFactor.Length,
            MinVuMinor = results.MinResult(BeamResultType.ShearForceMinor) * unitFactor.Force,
            MaxVuMinor = results.MaxResult(BeamResultType.ShearForceMinor) * unitFactor.Force,
            MinVuMajor = results.MinResult(BeamResultType.ShearForceMajor) * unitFactor.Force,
            MaxVuMajor = results.MaxResult(BeamResultType.ShearForceMajor) * unitFactor.Force,
            VonMises = results.MaxStressResult() * unitFactor.Stress,
            //End of beam moments
            StartMuMajor = results.StartResult(BeamResultType.BendingMomentMajor) * unitFactor.Force * unitFactor.Length,
            EndMuMajor = results.EndResult(BeamResultType.BendingMomentMajor) * unitFactor.Force * unitFactor.Length,
            StartMuMinor = results.StartResult(BeamResultType.BendingMomentMinor) * unitFactor.Force * unitFactor.Length,
            EndMuMinor = results.EndResult(BeamResultType.BendingMomentMinor) * unitFactor.Force * unitFactor.Length,
        };
    }

    private static void Calculateω1Values(BeamForces forces, BendingConstants sbc)
    {
        var κMajor = forces.SmallerStartOrEndMuMajor() / forces.LargerStartOrEndMuMajor();
        var κMinor = forces.SmallerStartOrEndMuMinor() / forces.LargerStartOrEndMuMinor();

        //Positive implies double curvature while - is single
        var curvature2 = forces.MinMuMajor < 0 & forces.MaxMuMajor > 0 ? 1 : -1;
        var curvature1 = forces.MinMuMinor < 0 & forces.MaxMuMinor > 0 ? 1 : -1;

        κMajor *= curvature2;
        κMinor *= curvature1;

        //Here the w1 value is determined by assuming that if the end moment is greater than the moment at any other point within the element
        //then it is not subjected to transverse loads between supports.
        if (forces.MaxAbsMuMinor == Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)) && Math.Abs(sbc.MuMinorQuarter) <= forces.MaxAbsMuMinor
            && Math.Abs(sbc.MuMinorHalf) <= forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinorThreeQuarter) <= forces.MaxAbsMuMinor)
        {
            sbc.ω1Minor = Math.Max(0.6 - 0.4 * κMinor, 0.4);
        }
        else if (forces.MaxAbsMuMinor == Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor))
            && (Math.Abs(sbc.MuMinorQuarter) == forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinorHalf) == forces.MaxAbsMuMinor || Math.Abs(sbc.MuMinorHalf) == forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinorThreeQuarter) == forces.MaxAbsMuMinor))
            sbc.ω1Minor = 0.85;
        else if (forces.MaxAbsMuMinor > Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)))
            sbc.ω1Minor = 1;
        else
            sbc.ω1Minor = 0.85;


        if (forces.MaxAbsMuMajor == Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)) && Math.Abs(sbc.MuMajorHalf) <= forces.MaxAbsMuMajor
            && Math.Abs(sbc.MuMajorHalf) <= forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajorThreeQuarter) <= forces.MaxAbsMuMajor)
        {
            sbc.Loadω1Case = 1;
            sbc.ω1Major = Math.Max(0.6 - 0.4 * κMajor, 0.4);
        }
        else if (forces.MaxAbsMuMajor == Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor))
            && (Math.Abs(sbc.MuMajorQuarter) == forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajorHalf) == forces.MaxAbsMuMajor || Math.Abs(sbc.MuMajorHalf) == forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajorThreeQuarter) == forces.MaxAbsMuMajor))
        {
            sbc.Loadω1Case = 3;
            sbc.ω1Major = 0.85;
        }
        else if (forces.MaxAbsMuMajor > Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)))
        {
            sbc.Loadω1Case = 2; sbc.ω1Major = 1;
        }
        else
            sbc.ω1Major = 0.85;
    }

    private SansUlsResult GetSansDesignUlsResult(BeamForces bf, BendingConstants sbc, StrandBeamResults sbr, DesignType sansDesignType)
    {
        var util = new SansUtilization();
        SectionClassification? flexClass = null;

        var beam = sbr.Beam;
        if (beam.Resistance == null)
            throw new ArgumentNullException(nameof(sbr));

        var axialClass = GetAxialClass(beam);

        CompressionResistance? cr = null;
        MomentResistance? mr = null;

        switch (sansDesignType)
        {
            case DesignType.Tension:
                {
                    util.Tension = Math.Abs(bf.MaxAxialForce / beam.Resistance.Tr);
                    AddTensionSlendernessChecks(util, beam);
                    break;
                }
            case DesignType.Compression:
                {
                    cr = CompressionService.CompressiveResistance(beam, axialClass, Math.Abs(bf.MinAxialForce));
                    util.Compression = Math.Abs(bf.MinAxialForce / cr.Cr);
                    AddCompressionSlendernessChecks(util, beam);
                    break;
                }
            case DesignType.Bending: //Bending Only
                {
                    flexClass = ClassificationService.ClassifyFlexuralCompression(beam.Section, bf.MinAxialForce);
                    mr = BendingService.MomentResistance(beam, bf, sbc, flexClass);

                    //Check shear alone
                    util.ShearMajor = Math.Abs(bf.MaxAbsVuMajor / beam.Resistance.VrMajor);
                    util.ShearMinor = Math.Abs(bf.MaxAbsVuMinor / beam.Resistance.VrMinor);

                    if (mr == null)
                        break;

                    //Check Bending alone
                    util.BendingMajor = Math.Abs(bf.MaxAbsMuMajor / mr.MrMajor);
                    util.BendingMinor = Math.Abs(bf.MaxAbsMuMinor / mr.MrMinor);

                    //Check bi-axial bending
                    util.BiAxialBending = util.BendingMajor + util.BendingMinor;

                    //Check combined shear and moment
                    util.ShearAndBendingMajor = Math.Abs(0.455D * (bf.MaxVuMajor / beam.Resistance.VrMajor)) + Math.Abs(0.727D * (bf.MaxAbsMuMajor / mr.MrMajor));
                    util.ShearAndBendingMinor = Math.Abs(0.455D * (bf.MaxVuMinor / beam.Resistance.VrMinor)) + Math.Abs(0.727D * (bf.MaxAbsMuMinor / mr.MrMinor));

                    break;
                }
            case DesignType.BendingAxial: //Bending and Tension
                {
                    flexClass = ClassificationService.ClassifyFlexuralCompression(beam.Section, bf.MinAxialForce);
                    mr = BendingService.MomentResistance(beam, bf, sbc, flexClass);

                    //Check shear alone
                    util.ShearMajor = Math.Abs(bf.MaxAbsVuMajor / beam.Resistance.VrMajor);
                    util.ShearMinor = Math.Abs(bf.MaxAbsVuMinor / beam.Resistance.VrMinor);

                    //Check tension alone
                    if (bf.MaxAxialForce > 0)
                    {
                        util.Tension = Math.Abs(bf.MaxAxialForce / beam.Resistance.Tr);
                        AddTensionSlendernessChecks(util, beam);
                    }

                    //Check compression alone
                    if (bf.MinAxialForce < 0)
                    {
                        cr = CompressionService.CompressiveResistance(beam, axialClass, Math.Abs(bf.MinAxialForce));
                        util.Compression = Math.Abs(bf.MinAxialForce / cr.Cr);
                        AddCompressionSlendernessChecks(util, beam);
                    }

                    if (mr == null)
                        break;

                    //Check bending alone
                    util.BendingMajor = Math.Abs(bf.MaxAbsMuMajor / mr.MrMajor);
                    util.BendingMinor = Math.Abs(bf.MaxAbsMuMinor / mr.MrMinor);

                    //Check bi-axial bending
                    util.BiAxialBending = util.BendingMajor + util.BendingMinor;

                    //Check combined shear and moment
                    util.ShearAndBendingMajor = Math.Abs(0.455D * (bf.MaxVuMajor / beam.Resistance.VrMajor)) + Math.Abs(0.727D * (bf.MaxAbsMuMajor / mr.MrMajor));
                    util.ShearAndBendingMinor = Math.Abs(0.455D * (bf.MaxVuMinor / beam.Resistance.VrMinor)) + Math.Abs(0.727D * (bf.MaxAbsMuMinor / mr.MrMinor));

                    //Combined combined tension and bending 
                    if (bf.MaxAxialForce > 0)
                        util.TensionAndBending = CombinedService.CombinedTensionAndBending(bf, beam, mr, flexClass);

                    //Combined combined compression and bending
                    if (bf.MinAxialForce < 0 && cr != null)
                    {
                        Calculateω1Values(bf, sbc);
                        var bendingAndCompression = CombinedService.CombinedCompressionAndBending(bf, beam, sbc.ω1Major, sbc.ω1Minor, cr, mr, flexClass, beam.Section.IsBracedFrame);

                        util.CompressionAndBendingSectionStrength = bendingAndCompression.CrossSectional;
                        util.CompressionAndBendingMemberStrength = bendingAndCompression.OverallMember;
                        util.CompressionAndBendingBucklingStrength = bendingAndCompression.LateralTorsionalBuckling;
                    }
                    break;
                }
            default:
                break;
        }

        if (_designModel.DesignSettings.IncludeAllowableStressCheck)
            util.AllowableStress = bf.VonMises / CombinedService.AllowableStress(beam.Section.Material);

        var capacity = new SansCapacity()
        {
            Tr = beam.Resistance.Tr / 1000,
            VrMajor = beam.Resistance.VrMajor / 1000,
            VrMinor = beam.Resistance.VrMinor / 1000,
            ω1Major = sbc.ω1Major,
            ω1Minor = sbc.ω1Minor,
            ω2Major = sbc.McrMajorω,
            ω2Minor = sbc.McrMinorω,
            SlendernessMajor = beam.Resistance.SlendernessMajor,
            SlendernessMinor = beam.Resistance.SlendernessMinor,
            AllowableStress = CombinedService.AllowableStress(beam.Section.Material),
            BendingConstants = sbc,
        };

        if (cr != null)
        {
            capacity.Cr = cr.Cr / 1000;
            capacity.CrMajor = cr.CrMajor / 1000;
            capacity.CrMinor = cr.CrMinor / 1000;
        }

        if (mr != null)
        {
            capacity.MrMajor = mr.MrMajor / 1000000;
            capacity.MrMinor = mr.MrMinor / 1000000;
        }

        var loads = new UlsLoads
        {
            Cu = bf.MinAxialForce / 1000,
            Tu = bf.MaxAxialForce / 1000,
            MuMajor = bf.MaxAbsMuMajor / 1000000,
            MuMinor = bf.MaxAbsMuMinor / 1000000,
            VuMajor = bf.MaxAbsVuMajor / 1000,
            VuMinor = bf.MaxAbsVuMinor / 1000,
            VonMisses = bf.VonMises / 1000,
        };

        return new SansUlsResult()
        {
            DesignType = sansDesignType,
            Beam = beam,
            Utilization = util,
            Capacity = capacity,
            Loads = loads,
            BracedState = beam.Section.IsBracedFrame,
            Forces = bf,
            FlexuralClass = flexClass,
            AxialClass = axialClass,
        };
    }

    private void AddCompressionSlendernessChecks(SansUtilization util, Beam beam)
    {
        if (_designModel.DesignSettings.IncludeSlendernessCheck)
        {
            util.SlendernessMajor = beam.Resistance.SlendernessMajor / 200D;
            util.SlendernessMinor = beam.Resistance.SlendernessMinor / 200D;
        }
    }

    private void AddTensionSlendernessChecks(SansUtilization util, Beam beam)
    {
        if (_designModel.DesignSettings.IncludeSlendernessCheck)
        {
            util.SlendernessMajor = beam.Resistance.SlendernessMajor / 300D;
            util.SlendernessMinor = beam.Resistance.SlendernessMinor / 300D;
        }
    }
}



//private static void DetermineWValuesOld(UnitFactor unitFactor, BeamForces forces, SansBendingConstants sbc, StrandBeamResults sbr)
//{
//    #region Determine w2 values

//    if (forces.MaxAbsMuMajor <= Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)))
//    {
//        sbc.McrMajorω = 12 / (3 * Math.Abs(sbc.MuMajor1ω2 / forces.MaxAbsMuMajor) + 4 * Math.Abs(sbc.MuMajor2ω2 / forces.MaxAbsMuMajor) + 3 * Math.Abs(sbc.MuMajor3ω2 / forces.MaxAbsMuMajor) + 2);
//        sbc.McrMajorω = Math.Min(sbc.McrMajorω, 2.5);
//    }
//    else
//        sbc.McrMajorω = 1;

//    if (double.IsNaN(sbc.McrMajorω) || double.IsInfinity(sbc.McrMajorω))
//        sbc.McrMajorω = 1;
//    #endregion

//    #region Estimating w1 values - Needs to be checked/improved!!! NB!~!
//    //Curvature variables    
//    var curveState2 = forces.MinMuMajor < 0 & forces.MaxMuMajor > 0;
//    var curvature2 = curveState2 ? 1 : -1;

//    var curveState1 = forces.MinMuMinor < 0 & forces.MaxMuMinor > 0;
//    var curvature1 = curveState1 ? 1 : -1;

//    //Here the w1 value is determined by assuming that if the end moment is greater than the moment at any other point within the element
//    //then it is not subjected to transverse loads between supports.
//    if (forces.MaxAbsMuMinor == Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)) && Math.Abs(sbc.MuMinor1ω2) <= forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinor2ω2) <= forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinor3ω2) <= forces.MaxAbsMuMinor)
//    {
//        sbc.ω1Minor = 0.6 - 0.4 * curvature1 * Math.Min(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)) / Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor));
//        sbc.ω1Minor = Math.Max(sbc.ω1Minor, 0.4);
//    }
//    else if (forces.MaxAbsMuMinor == Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)) && (Math.Abs(sbc.MuMinor1ω2) == forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinor2ω2) == forces.MaxAbsMuMinor || Math.Abs(sbc.MuMinor2ω2) == forces.MaxAbsMuMinor && Math.Abs(sbc.MuMinor3ω2) == forces.MaxAbsMuMinor))
//        sbc.ω1Minor = 0.85;
//    else if (forces.MaxAbsMuMinor > Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)))
//        sbc.ω1Minor = 1;
//    else
//        sbc.ω1Minor = 0.85;

//    if (double.IsNaN(sbc.ω1Minor) || double.IsInfinity(sbc.ω1Minor))
//        sbc.ω1Minor = 0;

//    if (forces.MaxAbsMuMajor == Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)) && Math.Abs(sbc.MuMajor2ω2) <= forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajor2ω2) <= forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajor3ω2) <= forces.MaxAbsMuMajor)
//    {
//        sbc.ω1Major = 0.6 - 0.4 * curvature2 * Math.Min(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)) / Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor));
//        sbc.ω1Major = Math.Max(sbc.ω1Major, 0.4);
//    }
//    else if (forces.MaxAbsMuMajor == Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)) && (Math.Abs(sbc.MuMajor1ω2) == forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajor2ω2) == forces.MaxAbsMuMajor || Math.Abs(sbc.MuMajor2ω2) == forces.MaxAbsMuMajor && Math.Abs(sbc.MuMajor3ω2) == forces.MaxAbsMuMajor))
//        sbc.ω1Major = 0.85;
//    else if (forces.MaxAbsMuMajor > Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)))
//        sbc.ω1Major = 1;
//    else
//        sbc.ω1Major = 0.85;

//    if (double.IsNaN(sbc.ω1Major) || double.IsInfinity(sbc.ω1Major))
//        sbc.ω1Major = 0;
//    #endregion
//}