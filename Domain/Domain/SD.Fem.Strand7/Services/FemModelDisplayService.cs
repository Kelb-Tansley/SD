namespace SD.Fem.Strand7.Services;
public class FemModelDisplayService(IStrandApiService strandApiService,
    IDesignModel designModel,
    IFemModelParameters femModelParameters) : IFemModelDisplayService
{
    private readonly IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
    private readonly IStrandApiService _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
    private readonly IFemModelParameters _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));

    public async Task DisplayDesignResults(int modelId, string fileName, nint handle, IEnumerable<UlsResultPeak> results)
    {
        _strandApiService.ClearFemDisplayModel(modelId);
        _strandApiService.DisplayFemFile(modelId, handle);
        await _strandApiService.DisplayFemDesignResults(modelId, results);
    }

    public async Task DisplayDesignLengths(int modelId, string fileName, nint handle, BeamAxis beamAxisEnum)
    {
        _strandApiService.DisplayFemFile(modelId, handle);
        await _strandApiService.DisplayDesignLengths(modelId, beamAxisEnum, _femModelParameters.Beams.ToList(), _femModelParameters.UnitFactor.Length);
    }
    public async Task DisplayDesignSlenderness(int modelId, string fileName, nint handle, BeamAxis beamAxisEnum)
    {
        _strandApiService.DisplayFemFile(modelId, handle);
        await _strandApiService.DisplayDesignSlenderness(modelId, beamAxisEnum, _femModelParameters.Beams.ToList(), _femModelParameters.UnitFactor.Length);
    }
    public bool OpenFemFile(int modelId, string fileName, bool closeFirst = true)
    {
        return _strandApiService.OpenFemFile(modelId, fileName, closeFirst);
    }
    public void ClearFemDisplayModel(int modelId)
    {
        _strandApiService.ClearFemDisplayModel(modelId);
    }
    public void ReloadFemDisplayModel(int modelId, string fileName, bool closeFirst = true)
    {
        _strandApiService.OpenFemFile(modelId, fileName, closeFirst);
        _strandApiService.CloseFemResultsFile(modelId);
    }
    public async Task DisplayDeflectionContours(int modelId, nint handle, double minDeflectionRatio, DeflectionAxis deflectionAxis, IEnumerable<DeflectionResult>? results)
    {
        _strandApiService.DisplayFemFile(modelId, handle);
        await _strandApiService.DisplayDeflectionContours(modelId, minDeflectionRatio, deflectionAxis, results);
    }
    public IEnumerable<Beam> GetDisplayedByGroupBeams(int modelId, IEnumerable<Beam> beams)
    {
        return _strandApiService.GetDisplayedByGroupBeams(modelId, beams);
    }
    public void UpdateFemModel(int modelId, nint handle)
    {
        _strandApiService.UpdateFemFile(modelId, handle);
    }
    public void UpdateBeamFemModel(int modelId, nint handle, int loadCase, ref int childId, bool isInitialized, ZoomLevel zoomLevel, Beam beam, BeamDisplayComponent beamDisplayComponent)
    {
        lock (_femModelParameters.Beams)
        {
            _strandApiService.DisplayFocusedFemFile(modelId, handle, loadCase, ref childId, isInitialized, zoomLevel, beam, _femModelParameters.Beams, beamDisplayComponent);
        }
    }
    public bool SetSelectedBeams(int modelId)
    {
        return _strandApiService.SetSelectedBeams(modelId, _femModelParameters.Beams.ToList());
    }

    public StrandResultFile OpenFemResultsFile(int modelId, string fileName)
    {
        return _strandApiService.OpenFemResultsFile(modelId, fileName, _designModel.SolverType);
    }

    public void CloseFemResultsFile(int modelId)
    {
        _strandApiService.CloseFemResultsFile(modelId);
    }

    public Result LoadFemModelProperties(int modelId, DesignCode designCode, string fileName, bool closeFirst = false)
    {
        StrandResultFile resultsFile;
        try
        {
            resultsFile = _strandApiService.OpenFemResultsFile(modelId, fileName, _designModel.SolverType, closeFirst);
        }
        catch (Exception)
        {
            return Result.Fail("A result file could not be detected for this Strand7 file. Would you like to run the solver and try again?");
        }

        _strandApiService.GetFemModelParameters(_femModelParameters, designCode, modelId, _designModel.SolverType, resultsFile);
        if (_femModelParameters.IsInitialized)
            return Result.Ok();

        return Result.Fail("Model parameters could not be loaded.");
    }
}
