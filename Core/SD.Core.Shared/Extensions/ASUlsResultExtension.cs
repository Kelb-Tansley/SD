using SD.Core.Shared.Models;
using SD.Core.Shared.Models.AS;

namespace SD.Core.Shared.Extensions;
public static class ASUlsResultExtension
{
    public static List<UlsResultPeak> ToUlsPeakResults(this IEnumerable<ASUlsResult> asUlsResults)
    {
        var ulsResults = new List<UlsResultPeak>();
        foreach (var asUlsResult in asUlsResults)
        {
            ulsResults.Add(new UlsResultPeak()
            {
                BeamId = asUlsResult.Beam.Number,
                LoadCaseId = asUlsResult.LoadCaseNumber,
                Utilization = asUlsResult.Utilization.MaxUtilization,
                DesignCode = Enum.DesignCode.AS
            });
        }
        return ulsResults;
    }
    public static UlsResultPeak ToUlsPeakResult(this ASUlsResult asUlsResult, int loadCaseNumber)
    {
        return new UlsResultPeak()
        {
            BeamId = asUlsResult.Beam.Number,
            LoadCaseId = loadCaseNumber,
            Utilization = asUlsResult.Utilization.MaxUtilization,
            DesignCode = Enum.DesignCode.AS
        };
    }
}
