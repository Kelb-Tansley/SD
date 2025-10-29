namespace SD.Fem.Strand7.Interfaces;

public interface IBucklingAnalysisService
{
    public Task SolveModelAsync(int modelId,
            string fileName,
            int numModes,
            int startLoadCase,
            int endLoadCase,
            int fixedLoadCase,
            bool overwriteCsv,
            bool includeEndLoadCase);
}
