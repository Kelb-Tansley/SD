using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Models.Loading;

namespace SD.Fem.Strand7.Interfaces;
public interface IStrandApiService
{
    public bool OpenFemFile(int modelId, string fileName, bool closeFirst = true, bool openReadOnly = true);
    public void UpdateFemFile(int modelId, nint handle);
    public void UpdateBeamFemFile(int modelId, nint handle);
    public bool SetSelectedBeams(int modelId, IEnumerable<Beam> beams);
    public void DisplayFemFile(int modelId, nint handle);
    public void DisplayFocusedFemFile(int modelId, nint handle, int loadCase, ref int childId, bool isInitialized, ZoomLevel zoomLevel, Beam focusedBeam, IEnumerable<Beam> beams, BeamDisplayComponent beamDisplayComponent);
    public void ApplyBeamWindLoads(int modelId, int loadCase, double[] windLoadVector, WindLoadingModel windLoadingModel, IEnumerable<Beam> beams, UnitFactor unitFactor);
    public StrandResultFile OpenFemResultsFile(int modelId, string fileName, SolverType solverType, bool closeFirst = false);
    public void CloseFemResultsFile(int modelId);
    public void CloseAllFemFiles(int modelId);
    public void GetFemModelParameters(IFemModelParameters femModelParameters, DesignCode designCode, int modelId, SolverType solverType, StrandResultFile strandResultFile);
    public List<LoadCaseCombination> GetFemModelLoadCaseCombinations(int modelId, SolverType solverType, StrandResultFile strandResultFile);
    public List<Section> GetFemBeamSections(int modelId, UnitFactor unitFactor, DesignCode designCode);
    public Task DisplayFemDesignResults(int modelId, IEnumerable<UlsResultPeak> results);
    public Task DisplayDesignLengths(int modelId, BeamAxis beamAxisEnum, IEnumerable<Beam> beams, double lengthFactor);
    public Task DisplayDesignSlenderness(int modelId, BeamAxis beamAxisEnum, IEnumerable<Beam> beams, double lengthFactor);
    public void ClearFemDisplayModel(int modelId);
    public IEnumerable<Beam> GetDisplayedByGroupBeams(int modelId, IEnumerable<Beam> beams);
    public Task DisplayDeflectionContours(int modelId, double minDeflectionRatio, DeflectionAxis deflectionAxis, IEnumerable<DeflectionResult>? results);

    /// <summary>
    /// Gets the strand beam force or deflection results for each beam provided and each included load case combination, refined by the number of stations specified per beam. 
    /// The return count is a product of the number of beams and the number of included load case combinations.
    /// </summary>
    public List<StrandBeamResults> GetBeamResults(int modelId, ResultType resultType, IEnumerable<Beam> beams, IEnumerable<LoadCaseCombination> loadCaseCombinations, int minStations);
    public List<LoadCase> GetPrimaryLoadCases(int modelId);
    public int GetNumberOfPrimaryLoadCases(int modelId);
    public int GetNumberOfLSALoadCaseCombinations(int modelId);
    public double GetBucklingFactor(int modelId, int modeNumber);
    public void RunLinearStaticAnalysis(int modelId);
    public void RunLinearBucklingAnalysis(int modelId);
    public void EnableFirstLoadFreedomCase(int modelId, string resultFilePath);
    public void SetLinearBucklingModes(int modelId, string fileName, string resultFilePath, int numModes, int variableCaseNum, int fixedCaseNum);
}