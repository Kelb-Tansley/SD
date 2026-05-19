using SD.Core.Shared.Models.Sans;

namespace SD.Core.Infrastructure.Interfaces;

public interface IUlsDataExportService
{
    void ExportToExcel(IEnumerable<SansUlsResult> rows);
}
