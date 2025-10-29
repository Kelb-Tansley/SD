using SD.Core.Shared.Models.Sans;
using SD.Core.Shared.Models;
using SD.Core.Infrastructure.Interfaces;

namespace SD.Core.Infrastructure.Services;

public class AsExportResultsService : IExportResultsService
{
    public void ExportToExcel<T>(List<T> results) where T : UlsResult
    {
        if (results is List<SansUlsResult> sansUlsResults)
            ExportToExcel(sansUlsResults);
    }

    private static void ExportToExcel(List<SansUlsResult> results)
    {
            
    }
}
