using SD.Core.Shared.Models.AS;
using SD.Core.Strand.Enum;
using SD.Element.Design.AS.Enums;
using SD.Element.Design.Services;
using SD.Fem.Strand7.Extensions;
using SD.Fem.Strand7.Interfaces;

namespace SD.Element.Design.AS.Services;
public class ASDesignService(
    IDesignModel designModel,
    IUlsDesignResults ulsDesignResults,
    IStrandApiService strandApiService,
    IFemModelParameters femModelParameters) : IElementDesignService
{
    private const double phi = 0.9D;
    private readonly IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
    private readonly IUlsDesignResults _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
    private readonly IStrandApiService _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
    private readonly IFemModelParameters _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));

    public async Task RunUlsDesignUpdate(int modelId, List<Beam> beams)
    {
        var updates = await GetElementUlsUtilization(modelId, beams, _femModelParameters, _designModel.DesignSettings.BeamMinStations);

        foreach (var update in updates)
        {
            var oldResult = _ulsDesignResults.AsUlsResults.FirstOrDefault(r => r.Beam.Number == update.Beam.Number && r.LoadCaseNumber == update.LoadCaseNumber);
            if (oldResult != null)
            {
                var index = _ulsDesignResults.AsUlsResults.IndexOf(oldResult);
                if (index != -1)
                    _ulsDesignResults.AsUlsResults[index] = update;
            }
        }
    }

    public async Task<IEnumerable<UlsResultPeak>> RunUlsDesign(int modelId, List<Beam> beams)
    {
        _ulsDesignResults.AsUlsResults = await GetElementUlsUtilization(modelId, beams, _femModelParameters, _designModel.DesignSettings.BeamMinStations);

        return _ulsDesignResults.AsUlsResults.ToUlsPeakResults();
    }

    private async Task<List<ASUlsResult>> GetElementUlsUtilization(int modelId, List<Beam> beams, IFemModelParameters femDesignParameters, int minStations)
    {
        var asUlsResults = new List<ASUlsResult>();
        if (femDesignParameters == null)
            return asUlsResults;

        var designableBeams = beams.Where(beam => beam.CanDesign()).ToList();
        SetASBeamResistances(designableBeams);

        var beamResults = _strandApiService.GetBeamResults(modelId, ResultType.Force, designableBeams, femDesignParameters.LoadCaseCombinations, minStations);

        var tasks = new List<Task>();
        foreach (var beamResult in beamResults)
        {
            tasks.Add(Task.Run(() =>
            {
                GetBeamUlsUtilisation(femDesignParameters, asUlsResults, beamResults, beamResult);
            }));
        }
        await Task.WhenAll(tasks);
        return asUlsResults;
    }

    private static void SetASBeamResistances(List<Beam> beams)
    {
        foreach (var beam in beams)
            beam.Resistance = new Models.ASBeamResistance(beam);
    }

    private void GetBeamUlsUtilisation(IFemModelParameters femDesignParameters, List<ASUlsResult> allResults, List<StrandBeamResults> beamResults, StrandBeamResults beamResult)
    {
        var connectedResults = new List<StrandBeamResults>();
        Beam[] chain = [beamResult.Beam]; // TODO: Check

        // We collect the beam results along the entire chain
        foreach (var beam in chain)
        {
            var connected = beamResults.FirstOrDefault(br => br.Beam.Number == beam.Number);
            if (connected != null)
                connectedResults.Add(connected);
        }

        var result = GetASBeamCapacity(beamResult, connectedResults, femDesignParameters.UnitFactor);
        if (result != null)
        {
            result.SetLoadCaseNumber(beamResult.LoadCaseId);
            lock (allResults)
                allResults.Add(result);
        }
    }

    private static ASUlsResult GetASBeamCapacity(StrandBeamResults beamResults, List<StrandBeamResults> connectedResults, UnitFactor unitFactor)
    {
        var beamForces = GetAllPeakBeamResults(unitFactor, connectedResults);

        var bendingConstants = SharedDesignService.SetBeamStationParameters(unitFactor, beamResults);

        var designType = SharedDesignService.GetDesignType(beamResults.Beam, beamForces); // TODO: Check AS Compatibility

        return GetASDesignUlsResult(beamForces, bendingConstants, beamResults, designType);
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

    private static ASUlsResult GetASDesignUlsResult(BeamForces beamForces, BendingConstants bendingConstants, StrandBeamResults beamResults, DesignType designType)
    {
        var utilisation = new ASUtilisation();
        var beam = beamResults.Beam;
        var section = beam.Section;

        if (beam.Resistance == null)
            throw new ArgumentNullException(nameof(beamResults));

        var Cap = new ASCapacity();

        switch (designType)
        {
            case DesignType.Tension:
                {
                    utilisation.AxialTension = Math.Abs(beamForces.MaxAxialForce / beam.Resistance.Tr);
                    break;
                }
            case DesignType.Compression:
                {
                    // Variables
                    var kf = ASCompressionService.CalculateFormFactor(section);

                    // Compression Capacity
                    Cap.Cr = ASCompressionService.CompressiveResistance(beam, kf, out _, out _, out _);

                    // Check Axial compression ratio
                    utilisation.AxialCompression = Math.Abs(beamForces.MinAxialForce / Cap.Cr);
                    break;
                }
            case DesignType.Bending:
                {
                    // Bending Capacity
                    double Msy;
                    Cap.MrMajor = ASMomentService.MomentResistance(beam, beamForces, bendingConstants, out double Msx, out Msy);
                    Cap.MrMinor = Msy;

                    // Check Shear alone
                    utilisation.MajorAxisShear = Math.Abs(beamForces.MaxAbsVuMajor / beam.Resistance.VrMajor);
                    utilisation.MinorAxisShear = Math.Abs(beamForces.MaxAbsVuMinor / beam.Resistance.VrMinor);

                    // Check Bending alone
                    utilisation.MajorAxisBending = Math.Abs(beamForces.MaxAbsMuMajor / Cap.MrMajor);
                    utilisation.MinorAxisBending = Math.Abs(beamForces.MaxAbsMuMinor / Msy);

                    // Calculate combined shear and moment capacity for the major axis
                    var majorShearCapacity = beam.Resistance.VrMajor;
                    if (beamForces.MaxAbsMuMajor > 0.75 * Msx)
                        majorShearCapacity *= (2.2 - ((1.6 * beamForces.MaxAbsMuMajor) / Msx));

                    // Calculate combined shear and moment capacity for the minor axis
                    var minorShearCapacity = beam.Resistance.VrMinor;
                    if (beamForces.MaxAbsMuMinor > 0.75 * Cap.MrMinor)
                        minorShearCapacity *= (2.2 - ((1.6 * beamForces.MaxAbsMuMinor) / Cap.MrMinor));

                    // Check Bending and Shear
                    utilisation.MajorBendingShear = Math.Abs(beamForces.MaxAbsVuMajor / majorShearCapacity);
                    utilisation.MinorBendingShear = Math.Abs(beamForces.MaxAbsVuMinor / minorShearCapacity);

                    // Check Bi-Axial Bending
                    utilisation.BiaxialMemberBending = Math.Pow(beamForces.MaxAbsMuMajor / Msx, 1.4) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MrMinor, 1.4);
                    utilisation.BiaxialSectionBending = Math.Pow(beamForces.MaxAbsMuMajor / Cap.MrMajor, 1.4) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MrMinor, 1.4);

                    break;
                }
            case DesignType.BendingAxial:
                {
                    // Variables
                    var majorSlenderness = ASMomentService.GetSectionSlenderness(beam.Section, BeamAxisPart.MajorTop);
                    var minorSlenderness = ASMomentService.GetSectionSlenderness(beam.Section, BeamAxisPart.MinorTop);
                    var kf = ASCompressionService.CalculateFormFactor(section);
                    var alternativeCase = ((section.SectionType == SectionType.IorH &&
                        section.B1 == section.B2 &&
                        section.T1 == section.T2) ||
                        section.SectionType == SectionType.RectangularHollow) &&
                        majorSlenderness == Slenderness.Compact;

                    // Bending and Compression Capacity
                    Cap.MrMajor = ASMomentService.MomentResistance(beam, beamForces, bendingConstants, out double Msx, out double Msy);
                    Cap.MrMinor = Msy;

                    Cap.Cr = ASCompressionService.CompressiveResistance(beam, kf, out double Ns, out _, out double Ncy);

                    // Check Shear alone
                    utilisation.MajorAxisShear = Math.Abs(beamForces.MaxAbsVuMajor / beam.Resistance.VrMajor);
                    utilisation.MinorAxisShear = Math.Abs(beamForces.MaxAbsVuMinor / beam.Resistance.VrMinor);

                    // Check Bending alone
                    utilisation.MajorAxisBending = Math.Abs(beamForces.MaxAbsMuMajor / Cap.MrMajor);
                    utilisation.MinorAxisBending = Math.Abs(beamForces.MaxAbsMuMinor / Cap.MrMinor);

                    // Check Bi-Axial Bending
                    utilisation.BiaxialSectionBending = Math.Pow(beamForces.MaxAbsMuMajor / Msx, 1.4) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MrMinor, 1.4);
                    utilisation.BiaxialMemberBending = Math.Pow(beamForces.MaxAbsMuMajor / Cap.MrMajor, 1.4) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MrMinor, 1.4);

                    // Calculate combined shear and moment capacity for the major axis // TODO: Move to combined actions?
                    Cap.MajorBendingShear = beam.Resistance.VrMajor;
                    if (beamForces.MaxAbsMuMajor > 0.75 * Msx)
                        Cap.MajorBendingShear *= (2.2 - ((1.6 * beamForces.MaxAbsMuMajor) / Msx));

                    // Calculate combined shear and moment capacity for the minor axis // TODO: Move to combined actions?
                    Cap.MinorBendingShear = beam.Resistance.VrMinor;
                    if (beamForces.MaxAbsMuMinor > 0.75 * Cap.MrMinor)
                        Cap.MinorBendingShear *= (2.2 - ((1.6 * beamForces.MaxAbsMuMinor) / Cap.MrMinor));

                    // Check Bending and Shear
                    utilisation.MajorBendingShear = Math.Abs(beamForces.MaxAbsVuMajor / Cap.MajorBendingShear);
                    utilisation.MinorBendingShear = Math.Abs(beamForces.MaxAbsVuMinor / Cap.MinorBendingShear);

                    // Uniaxial bending about the major x-axis with axial force
                    Cap.MajorSectionBendingTensionMrx = ASCombinedService.MajorSectionBendingAxial(Msx / phi, kf, alternativeCase, beam, beamForces, true, Ns);
                    Cap.MajorSectionBendingCompressionMrx = ASCombinedService.MajorSectionBendingAxial(Msx / phi, kf, alternativeCase, beam, beamForces, false, Ns);

                    utilisation.MajorSectionBendingAxialMrx = Math.Abs(beamForces.MaxAbsMuMajor / Math.Min(Cap.MajorSectionBendingTensionMrx, Cap.MajorSectionBendingCompressionMrx));

                    // Uniaxial bending about the minor y-axis with axial force
                    Cap.MinorSectionBendingTensionMry = ASCombinedService.MinorSectionBendingAxial(Msy / phi, alternativeCase, beam, beamForces, true, minorSlenderness, Ns);
                    Cap.MinorSectionBendingCompressionMry = ASCombinedService.MinorSectionBendingAxial(Msy / phi, alternativeCase, beam, beamForces, false, minorSlenderness, Ns);
                    utilisation.MinorSectionBendingAxialMry = Math.Abs(beamForces.MaxAbsMuMinor / Math.Min(Cap.MinorSectionBendingTensionMry, Cap.MinorSectionBendingCompressionMry));

                    // Initialise Capacities
                    Cap.MajorMemberBendingCompressionMix = Msx;
                    Cap.MinorMemberBendingCompressionMiy = Msy;
                    Cap.MajorMemberBendingCompressionMox = Cap.MrMajor;
                    Cap.MajorMemberBendingTensionMox = Math.Min(Cap.MrMajor, Cap.MajorSectionBendingTensionMrx);

                    // Tension
                    if (beamForces.MaxAxialForce > 0)
                    {
                        // === Section ===

                        // Check Axial Tension
                        utilisation.AxialTension = Math.Abs(beamForces.MaxAxialForce / beam.Resistance.Tr);

                        // Biaxial section bending with axial force
                        if (alternativeCase && minorSlenderness == Slenderness.Compact)
                        {
                            var gamma = Math.Min(1.4D + (beamForces.MaxAxialForce / beam.Resistance.Tr), 2D);
                            utilisation.BiaxialSectionBendingAxial = Math.Pow(beamForces.MaxAbsMuMajor / Cap.MajorSectionBendingTensionMrx, gamma) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MinorSectionBendingTensionMry, gamma);
                        }
                        else
                        {
                            utilisation.BiaxialSectionBendingAxial = beamForces.MaxAxialForce / beam.Resistance.Tr + beamForces.MaxAbsMuMajor / Msx + beamForces.MaxAbsMuMinor / Cap.MrMinor;
                        }

                        // === Member ===

                        // Out-of-plane Capacity according to Section 8.4.4.2 of AS 4100-2020
                        Cap.MajorMemberBendingTensionMox = Math.Min(Cap.MrMajor * (1D + beamForces.MaxAxialForce / beam.Resistance.Tr), Cap.MajorSectionBendingTensionMrx);
                        utilisation.MajorMemberBendingTensionMox = Math.Abs(beamForces.MaxAbsMuMajor / Cap.MajorMemberBendingTensionMox);

                        // Biaxial Bending Capacity according to Section 8.4.5.2 of AS 4100-2020
                        var Mtx = Math.Min(Cap.MajorSectionBendingTensionMrx, Cap.MajorMemberBendingTensionMox);
                        utilisation.BiaxialMemberBendingTension = Math.Pow(beamForces.MaxAbsMuMajor / Mtx, 1.4D) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MinorSectionBendingTensionMry, 1.4D);
                    }

                    // Compression
                    if (beamForces.MinAxialForce < 0)
                    {
                        // === Section ===

                        // Check Axial Compression
                        utilisation.AxialCompression = Math.Abs(beamForces.MinAxialForce / Cap.Cr);

                        // Biaxial section bending with axial force
                        if (alternativeCase && minorSlenderness == Slenderness.Compact)
                        {
                            var gamma = Math.Min(1.4D + (Math.Abs(beamForces.MinAxialForce) / Ns), 2D);
                            utilisation.BiaxialSectionBendingAxial = Math.Pow(beamForces.MaxAbsMuMajor / Cap.MajorSectionBendingCompressionMrx, gamma) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MinorSectionBendingCompressionMry, gamma);
                        }
                        else
                        {
                            utilisation.BiaxialSectionBendingAxial = Math.Abs(beamForces.MinAxialForce) / Ns + beamForces.MaxAbsMuMajor / Msx + beamForces.MaxAbsMuMinor / Cap.MrMinor;
                        }

                        // === Member ===

                        var newCr = ASCompressionService.CompressiveResistance(beam, kf, out _, out _, out _, true);

                        // In-plane Capacity Elastic (8.4.2.2)
                        if (alternativeCase && kf == 1)
                        {

                            Cap.MajorMemberBendingCompressionMix = Math.Min(Msx * ((1D - Math.Pow((1D + beam.Beta_m) / 2D, 3)) * (1D - Math.Abs(beamForces.MinAxialForce) / newCr) + 1.18D * Math.Pow((1D + beam.Beta_m) / 2D, 3) * Math.Sqrt(1D - Math.Abs(beamForces.MinAxialForce) / newCr)), Cap.MajorSectionBendingCompressionMrx); // Nominal in-plane member momemnt capacity in the x-axis

                            Cap.MinorMemberBendingCompressionMiy = Math.Min(Msy * ((1D - Math.Pow((1D + beam.Beta_m) / 2D, 3)) * (1D - Math.Abs(beamForces.MinAxialForce) / newCr) + 1.18D * Math.Pow((1D + beam.Beta_m) / 2D, 3) * Math.Sqrt(1D - Math.Abs(beamForces.MinAxialForce) / newCr)), Cap.MinorSectionBendingCompressionMry); // Nominal in-plane member momemnt capacity in the y-axis
                        }
                        else
                        {
                            Cap.MajorMemberBendingCompressionMix = Math.Min(Msx * 1D - Math.Abs(beamForces.MinAxialForce) / newCr, Cap.MajorSectionBendingCompressionMrx); // Nominal in-plane member momemnt capacity in the x-axis
                            Cap.MinorMemberBendingCompressionMiy = Math.Min(Msy * 1D - Math.Abs(beamForces.MinAxialForce) / newCr, Cap.MinorSectionBendingCompressionMry); // Nominal in-plane member momemnt capacity in the y-axis
                        }

                        utilisation.MajorMemberBendingCompressionMix = Math.Abs(beamForces.MaxAbsMuMajor / Cap.MajorMemberBendingCompressionMix);
                        utilisation.MinorMemberBendingCompressionMiy = Math.Abs(beamForces.MaxAbsMuMinor / Cap.MinorMemberBendingCompressionMiy);

                        // Out-of-plane Capacity (8.4.4.1)
                        var specialCase = false; // TODO: Assign real value
                        var alpha_bc = 0D; // TODO: Assign real value
                        var Mbxo = 0D; // TODO: Assign real value
                        var Noz = 0D; // TODO: Assign real value

                        if (specialCase)
                        {
                            Cap.MajorMemberBendingCompressionMox = Math.Min(alpha_bc * Mbxo * Math.Sqrt((1D - Math.Abs(beamForces.MinAxialForce) / Ncy) * (1 - Math.Abs(beamForces.MinAxialForce) / Noz)), Cap.MajorSectionBendingCompressionMrx);
                        }
                        else
                        {
                            Cap.MajorMemberBendingCompressionMox = Cap.MrMajor * (1D - Math.Abs(beamForces.MinAxialForce) / Ncy);
                        }

                        utilisation.MajorMemberBendingCompressionMox = Math.Abs(beamForces.MaxAbsMuMajor / Cap.MajorMemberBendingCompressionMox);

                        // Bi-axial Bending Capacity (8.4.5.1)
                        utilisation.BiaxialMemberBendingCompression = Math.Pow(beamForces.MaxAbsMuMajor / Math.Min(Cap.MajorMemberBendingCompressionMix, Cap.MajorMemberBendingCompressionMox), 1.4) + Math.Pow(beamForces.MaxAbsMuMinor / Cap.MinorMemberBendingCompressionMiy, 1.4);
                    }

                    break;
                }
            default:
                break;
        }

        utilisation.AllowableStress = beamForces.VonMises / (phi * beam.Section.Material.MinFy);
        var digits = 3;
        var capacity = new ASCapacity
        {
            Tr = Math.Round(beam.Resistance.Tr / 1000, digits),
            VrMajor = Math.Round(beam.Resistance.VrMajor / 1000, digits),
            VrMinor = Math.Round(beam.Resistance.VrMinor / 1000, digits),
            AllowableStress = phi * beam.Section.Material.MinFy,
            BendingConstants = bendingConstants,
            Cr = Math.Round(Cap.Cr / 1000, digits),
            MrMajor = Math.Round(Cap.MrMajor / 1000000, digits),
            MrMinor = Math.Round(Cap.MrMinor / 1000000, digits),
            MajorBendingShear = Math.Round(Cap.MajorBendingShear / 1000, digits),
            MinorBendingShear = Math.Round(Cap.MinorBendingShear / 1000, digits),
            MajorSectionBendingTensionMrx = Math.Round(Cap.MajorSectionBendingTensionMrx / 1000000, digits),
            MinorSectionBendingTensionMry = Math.Round(Cap.MinorSectionBendingTensionMry / 1000000, digits),
            MajorSectionBendingCompressionMrx = Math.Round(Cap.MajorSectionBendingCompressionMrx / 1000000, digits),
            MinorSectionBendingCompressionMry = Math.Round(Cap.MinorSectionBendingCompressionMry / 1000000, digits),
            MajorMemberBendingCompressionMix = Math.Round(Cap.MajorMemberBendingCompressionMix / 1000000, digits),
            MinorMemberBendingCompressionMiy = Math.Round(Cap.MinorMemberBendingCompressionMiy / 1000000, digits),
            MajorMemberBendingCompressionMox = Math.Round(Cap.MajorMemberBendingCompressionMox / 1000000, digits),
            MajorMemberBendingTensionMox = Math.Round(Cap.MajorMemberBendingTensionMox / 1000000, digits),
        };

        var loads = new UlsLoads
        {
            Cu = Math.Round(beamForces.MinAxialForce / 1000, digits),
            Tu = Math.Round(beamForces.MaxAxialForce / 1000, digits),
            MuMajor = Math.Round(beamForces.MaxAbsMuMajor / 1000000, digits),
            MuMinor = Math.Round(beamForces.MaxAbsMuMinor / 1000000, digits),
            VuMajor = Math.Round(beamForces.MaxAbsVuMajor / 1000, digits),
            VuMinor = Math.Round(beamForces.MaxAbsVuMinor / 1000, digits),
            VonMisses = Math.Round(beamForces.VonMises / 1000, digits),
        };

        return new ASUlsResult()
        {
            DesignType = designType,
            Beam = beam,
            Utilization = utilisation,
            Capacity = capacity,
            Loads = loads,
            Forces = beamForces
        };

    }
}