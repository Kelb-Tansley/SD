using SD.Core.Shared.Models;

namespace SD.Core.Infrastructure.Interfaces;

public interface IExportResultsService
{
    public void ExportToExcel<T>(List<T> results) where T : UlsResult;
}
