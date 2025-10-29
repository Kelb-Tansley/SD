using SD.Fem.Strand7.Interfaces;

namespace SD.Element.Design.Sans.Services;
public class SansDeflectionService : IDeflectionService
{
    private readonly IDesignModel _designModel;
    private readonly IStrandApiService _strandApiService;
    private readonly IStrandResultsService _strandResultsService;
    private readonly IFemModelParameters _femModelParameters;

    public SansDeflectionService(
        IStrandApiService strandApiService,
        IStrandResultsService strandResultsService,
        IDesignModel designModel,
        IFemModelParameters femModelParameters)
    {
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
        _strandResultsService = strandResultsService ?? throw new ArgumentNullException(nameof(strandResultsService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
    }

    public async Task<List<DeflectionResult>> GetDeflectionResults(int modelId, IEnumerable<LoadCaseCombination> loadCaseCombinations, IEnumerable<Beam> beams, DeflectionAxis deflectionAxis, DeflectionMethod deflectionMethod)
    {
        var allResults = new List<DeflectionResult>();
        if (_femModelParameters?.Beams == null)
            return allResults;

        var beamResults = _strandApiService.GetBeamResults(modelId, ResultType.Deflection, beams, loadCaseCombinations, _designModel.DesignSettings.BeamMinStations);

        var tasks = new List<Task>();
        foreach (var beamResult in beamResults)
        {
            tasks.Add(Task.Run(() =>
            {
                var result = GetDeflectionResult(beamResult, _femModelParameters.UnitFactor, deflectionAxis, deflectionMethod);
                if (result != null)
                {
                    lock (allResults)
                        allResults.Add(result);
                }
            }));
        }
        await Task.WhenAll(tasks);
        return GetPeakForEachBeam(allResults);
    }

    private static List<DeflectionResult> GetPeakForEachBeam(List<DeflectionResult> allResults)
    {
        var peakResults = new List<DeflectionResult>();
        var beamIds = allResults.Select(res => res.BeamId)?.Distinct()?.ToList();
        foreach (var beamId in beamIds)
        {
            var beamResults = allResults.Where(res => res.BeamId == beamId).MinBy(res => res.DeflectionRatio);
            if (beamResults != null)
                peakResults.Add(beamResults);
        }

        return peakResults;
    }

    private DeflectionResult GetDeflectionResult(StrandBeamResults beamResult, UnitFactor unitFactor, DeflectionAxis deflectionAxis, DeflectionMethod deflectionMethod)
    {
        var maxDeflection = double.NegativeInfinity;
        var minDeflection = double.PositiveInfinity;

        switch (deflectionMethod)
        {
            case DeflectionMethod.Relative:
                {
                    var beamlength = GetBeamLengthByGlobalAxis(deflectionAxis, beamResult.Beam);
                    var relativeSlope = GetDeflectionSlope(beamResult, unitFactor, deflectionAxis, beamlength);
                    maxDeflection = Math.Max(maxDeflection, _strandResultsService.GetMaxDeflectionBySlope(beamResult.BeamRes, deflectionAxis, unitFactor, beamResult.NumStations, relativeSlope, beamlength));
                    minDeflection = Math.Min(minDeflection, _strandResultsService.GetMinDeflectionBySlope(beamResult.BeamRes, deflectionAxis, unitFactor, beamResult.NumStations, relativeSlope, beamlength));

                    var deflectionRatio = GetDeflectionRatio(deflectionAxis, maxDeflection, minDeflection, beamResult.Beam);
                    return new DeflectionResult()
                    {
                        BeamId = beamResult.Beam.Number,
                        LoadCaseId = beamResult.LoadCaseId,
                        DeflectionRatio = deflectionRatio > 1000 ? 1000 : deflectionRatio,
                    };
                }
            case DeflectionMethod.Absolute:
                {
                    maxDeflection = Math.Max(maxDeflection, _strandResultsService.GetMaxDeflection(beamResult.BeamRes, deflectionAxis, unitFactor, beamResult.NumStations));
                    minDeflection = Math.Min(minDeflection, _strandResultsService.GetMinDeflection(beamResult.BeamRes, deflectionAxis, unitFactor, beamResult.NumStations));

                    var deflectionRatio = GetDeflectionRatio(deflectionAxis, maxDeflection, minDeflection, beamResult.Beam);
                    return new DeflectionResult()
                    {
                        BeamId = beamResult.Beam.Number,
                        LoadCaseId = beamResult.LoadCaseId,
                        DeflectionRatio = deflectionRatio > 1000 ? 1000 : deflectionRatio,
                    };
                }
            default:
                throw new NotImplementedException();
        }
    }

    private double GetDeflectionSlope(StrandBeamResults beamResult, UnitFactor unitFactor, DeflectionAxis deflectionAxis, double beamlength)
    {
        var startDeflection = _strandResultsService.GetStartDeflection(beamResult.BeamRes, deflectionAxis, unitFactor);
        var endDeflection = _strandResultsService.GetEndDeflection(beamResult.BeamRes, deflectionAxis, unitFactor, beamResult.NumStations);
        var slopeDirection = startDeflection > endDeflection ? -1 : 1;
        return slopeDirection * _strandResultsService.GetDeflectionDifference(startDeflection, endDeflection) / beamlength;
    }

    private double GetDeflectionRatio(DeflectionAxis deflectionAxis, double maxDeflection, double minDeflection, Beam beam)
    {
        var deflectionDifference = _strandResultsService.GetDeflectionDifference(maxDeflection, minDeflection);

        var length = GetBeamLengthByGlobalAxis(deflectionAxis, beam);

        return length / deflectionDifference;
    }

    private static double GetBeamLengthByGlobalAxis(DeflectionAxis deflectionAxis, Beam beam)
    {
        //TODO: Big change needed here
        return beam.BeamL2;
    }
}
