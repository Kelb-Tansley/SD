using SD.Core.Infrastructure.Interfaces;
using SD.Data.Interfaces;

namespace SD.Fem.Strand7.Services;

public class BucklingAnalysisService(IFemModelDisplayService femModelDisplayService,
                                     IStrandApiService strandApiService,
                                     IFemFilePathService femFilePathService,
                                     INotificationService notificationService) : IBucklingAnalysisService
{
    private readonly IFemModelDisplayService _femModelDisplayService = femModelDisplayService;
    private readonly IStrandApiService _strandApiService = strandApiService;
    private readonly IFemFilePathService _femFilePathService = femFilePathService;
    private readonly INotificationService _notificationService = notificationService;

    private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Buckling analysis results.csv");

    public async Task SolveModelAsync(int modelId,
                                      string fileName,
                                      int numModes,
                                      int startLoadCase,
                                      int endLoadCase,
                                      int fixedLoadCase,
                                      bool overwriteCsv,
                                      bool includeEndLoadCase)
    {
        ValidateFileName(fileName);

        _femModelDisplayService.OpenFemFile(modelId, fileName, true);

        var totalLoadCases = CalculateTotalLoadCases(modelId, includeEndLoadCase, endLoadCase);

        ConfigureLSA(modelId, fileName);

        var csvResults = InitializeCsvResults(numModes);

        ProcessLoadCases(modelId, fileName, numModes, startLoadCase, totalLoadCases, fixedLoadCase, csvResults);

        await SaveAndOpenCsvResultsAsync(csvResults);
    }

    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("The analysis file name cannot be null or empty.", nameof(fileName));
    }

    private int CalculateTotalLoadCases(int modelId, bool includeEndLoadCase, int endLoadCase)
    {
        var numLoadCase = _strandApiService.GetNumberOfPrimaryLoadCases(modelId);
        var numCombinations = _strandApiService.GetNumberOfLSALoadCaseCombinations(modelId);
        return includeEndLoadCase ? endLoadCase : numCombinations + numLoadCase;
    }

    private void ConfigureLSA(int modelId, string fileName)
    {
        var basePath = Path.ChangeExtension(fileName, null);
        _strandApiService.EnableFirstLoadFreedomCase(modelId, basePath);
        _strandApiService.RunLinearStaticAnalysis(modelId);
    }

    private static List<string> InitializeCsvResults(int numModes)
    {
        var header = new List<string> { "Load Case" };
        for (int i = 0; i < numModes; i++) 
            header.Add($"Mode {i + 1}");

        return [string.Join(",", header)];
    }

    private void ProcessLoadCases(int modelId, string fileName, int numModes, int startLoadCase, int totalLoadCases, int fixedLoadCase, List<string> csvResults)
    {
        var basePath = Path.ChangeExtension(fileName, null);

        for (int loadCase = startLoadCase; loadCase <= totalLoadCases; loadCase++)
        {
            NotifyLoadCaseRunning(loadCase);

            var lbaResultPath = GenerateLbaResultPath(basePath, loadCase);

            RunBucklingAnalysis(modelId, basePath, lbaResultPath, numModes, loadCase, fixedLoadCase);

            var results = GetBucklingResults(modelId, numModes);

            CleanupResultFiles(lbaResultPath);

            AppendResultsToCsv(csvResults, loadCase, results);
        }
    }

    private void NotifyLoadCaseRunning(int loadCase)
    {
        _notificationService.ShowSnackNotification(new Notification("Running", $"Running load case: {loadCase}"));
    }

    private static string GenerateLbaResultPath(string basePath, int loadCase)
    {
        return $"{basePath?.Replace(basePath[(basePath.LastIndexOf('\\') + 1)..], basePath[(basePath.LastIndexOf('\\') + 1)..].Replace(".", "_"))}_LoadCase_{loadCase}";
    }

    private void RunBucklingAnalysis(int modelId, string basePath, string lbaResultPath, int numModes, int loadCase, int fixedLoadCase)
    {
        _strandApiService.SetLinearBucklingModes(modelId, basePath, lbaResultPath, numModes, loadCase, fixedLoadCase);
        _strandApiService.RunLinearBucklingAnalysis(modelId);
        _strandApiService.OpenFemResultsFile(modelId, $"{lbaResultPath}.LBA", SolverType.LBA, true);
    }

    private List<double> GetBucklingResults(int modelId, int numModes)
    {
        var results = new List<double>();
        for (int mode = 1; mode <= numModes; mode++)
        {
            var bucklingFactor = _strandApiService.GetBucklingFactor(modelId, mode);
            results.Add(bucklingFactor);
        }
        _strandApiService.CloseFemResultsFile(modelId);
        return results;
    }

    private void CleanupResultFiles(string filePath)
    {
        try
        {
            File.Delete($"{filePath}.LBA");
            File.Delete($"{filePath}.LBL");
        }
        catch (Exception)
        {
            _notificationService.ShowSnackNotification(new Notification("Cleanup Error", $"Could not delete a .LBA or .LBL for: {filePath[(filePath.LastIndexOf('\\') + 1)..]}"));
        }
    }

    private static void AppendResultsToCsv(List<string> bucklingAnalysisResults, int loadCase, List<double> results)
    {
        bucklingAnalysisResults.Add($"{loadCase},{string.Join(",", results)}");
    }

    private async Task SaveAndOpenCsvResultsAsync(List<string> csvResults)
    {
        await _femFilePathService.StoreBlobFileAsync(_csvFilePath, csvResults);
        _femFilePathService.OpenBlobFile(_csvFilePath);
    }
}