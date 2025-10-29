using SD.Core.Shared.Models.Sans;

namespace SD.Element.Design.Sans.Extensions;
public static class SansUlsResultExtension
{
    public static List<UlsResultPeak> ToUlsPeakResults(this IEnumerable<SansUlsResult> sansUlsResults)
    {
        var ulsResults = new List<UlsResultPeak>();
        foreach (var sansUlsResult in sansUlsResults)
        {
            ulsResults.Add(new UlsResultPeak()
            {
                BeamId = sansUlsResult.Beam.Number,
                LoadCaseId = sansUlsResult.LoadCaseNumber,
                Utilization = sansUlsResult.Utilization.MaxUtilization
            });
        }
        return ulsResults;
    }
    public static UlsResultPeak ToUlsPeakResult(this SansUlsResult sansUlsResult, int loadCaseNumber)
    {
        return new UlsResultPeak()
        {
            BeamId = sansUlsResult.Beam.Number,
            LoadCaseId = loadCaseNumber,
            Utilization = sansUlsResult.Utilization.MaxUtilization
        };
    }
}
