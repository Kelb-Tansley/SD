using Microsoft.Win32;
using OfficeOpenXml;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.AS;
using SD.Core.Shared.Models.Sans;
using System.ComponentModel;
using System.IO;

namespace SD.Core.Infrastructure.Services;

public class SansExportResultsService : IExportResultsService
{
    private readonly INotificationService _notificationService;

    public SansExportResultsService(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void ExportToExcel<T>(List<T> results) where T : UlsResult
    {
        ExportToExcel(results.Cast<SansUlsResult>().ToList());
    }

    private void ExportToExcel(List<SansUlsResult> results)
    {
        var filePath = GetSaveFilePath();
        if (string.IsNullOrEmpty(filePath))
            return;

        var backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += (sender, e) => ExcelExportWorkerDoWork(e, filePath, results);
        backgroundWorker.RunWorkerCompleted += ExcelMathcadExportWorkerRunWorkerCompleted;
        backgroundWorker.RunWorkerCompleted += (sender, e) => backgroundWorker.Dispose();
        backgroundWorker.RunWorkerAsync();
    }

    private void ExcelExportWorkerDoWork(DoWorkEventArgs e, string filePath, List<SansUlsResult> results)
    {
        ExcelPackage.License.SetNonCommercialOrganization("Hatch");
        using var package = new ExcelPackage();

        _notificationService.ShowSnackNotification(new Notification("Started", "Excel export process has started..."));

        var worksheet = package.Workbook.Worksheets.Add("ULS Results");

        // Set font and size
        worksheet.Cells.Style.Font.Name = "Arial";
        worksheet.Cells.Style.Font.Size = 10;

        SetTableHeaders(worksheet);

        SetTableRowValues(results, worksheet, 2);

        // Save to file
        var file = new FileInfo(filePath);
        package.SaveAs(file);

        e.Result = filePath;
    }

    private void ExcelMathcadExportWorkerRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
            _notificationService.ShowSnackNotification(new Notification("Error", e.Error.Message));
        else if (e.Result != null)
            _notificationService.ShowSnackNotification(new Notification("Saved", $"Excel export process has saved:{e.Result}."));
    }

    private static void SetTableRowValues(List<SansUlsResult> results, ExcelWorksheet worksheet, int decimalPlaces = 3)
    {
        for (int i = 0; i < results.Count; i++)
        {
            var res = results[i];

            // Add title values
            worksheet.Cells[i + 2, 1].Value = res.Beam.Number;
            worksheet.Cells[i + 2, 2].Value = res.Beam.Section.Number;
            worksheet.Cells[i + 2, 3].Value = res.LoadCaseNumber;
            worksheet.Cells[i + 2, 4].Value = res.Beam.Section.DisplayName;

            // Add ULS result values
            worksheet.Cells[i + 2, 5].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.Tension, decimalPlaces);
            worksheet.Cells[i + 2, 6].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.Compression, decimalPlaces);
            worksheet.Cells[i + 2, 7].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.BendingMajor, decimalPlaces);
            worksheet.Cells[i + 2, 8].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.BendingMinor, decimalPlaces);
            worksheet.Cells[i + 2, 9].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.BiAxialBending, decimalPlaces);
            worksheet.Cells[i + 2, 10].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.ShearMajor, decimalPlaces);
            worksheet.Cells[i + 2, 11].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.ShearMinor, decimalPlaces);
            worksheet.Cells[i + 2, 12].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.CompressionAndBendingSectionStrength, decimalPlaces);
            worksheet.Cells[i + 2, 13].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.CompressionAndBendingMemberStrength, decimalPlaces);
            worksheet.Cells[i + 2, 14].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.CompressionAndBendingBucklingStrength, decimalPlaces);
            worksheet.Cells[i + 2, 15].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.ShearAndBendingMajor, decimalPlaces);
            worksheet.Cells[i + 2, 16].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.ShearAndBendingMinor, decimalPlaces);
            worksheet.Cells[i + 2, 17].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.TensionAndBending, decimalPlaces);
            worksheet.Cells[i + 2, 18].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.AllowableStress, decimalPlaces);
            worksheet.Cells[i + 2, 19].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.SlendernessMajor, decimalPlaces);
            worksheet.Cells[i + 2, 20].Value = RoundToPercentageNonZeroDecimalPlaces(res.Utilization.SlendernessMinor, decimalPlaces);
        }
    }

    private static double RoundToPercentageNonZeroDecimalPlaces(double value, int decimalPlaces)
    {
        value *= 100;

        if (value == 0)
            return 0;

        var scale = Math.Pow(10, decimalPlaces);
        return Math.Round(value * scale) / scale;
    }

    private static void SetTableHeaders(ExcelWorksheet worksheet)
    {
        // Add title headers
        worksheet.Cells[1, 1].Value = "Beam No.";
        worksheet.Cells[1, 2].Value = "Prop. No.";
        worksheet.Cells[1, 3].Value = "LLCC";
        worksheet.Cells[1, 4].Value = "St7 Property Name";

        // Add ULS result headers
        worksheet.Cells[1, 5].Value = "Tu/Tr";
        worksheet.Cells[1, 6].Value = "Cu/Cr";
        worksheet.Cells[1, 7].Value = "Mu/Mr(Major)";
        worksheet.Cells[1, 8].Value = "Mu/Mr(Minor)";
        worksheet.Cells[1, 9].Value = "Mu/Mr(1+2)";
        worksheet.Cells[1, 10].Value = "Vu/Vr(Major)";
        worksheet.Cells[1, 11].Value = "Vu/Vr(Minor)";
        worksheet.Cells[1, 12].Value = "13.8 a)";
        worksheet.Cells[1, 13].Value = "13.8 b)";
        worksheet.Cells[1, 14].Value = "13.8 c)";
        worksheet.Cells[1, 15].Value = "V+M(Major)";
        worksheet.Cells[1, 16].Value = "V+M(Minor)";
        worksheet.Cells[1, 17].Value = "T+M(1+2)";
        worksheet.Cells[1, 18].Value = "Von Mises";
        worksheet.Cells[1, 19].Value = "Slenderness(Major)";
        worksheet.Cells[1, 20].Value = "VSlenderness(Minor)";
    }

    private static string GetSaveFilePath()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel Files|*.xlsx",
            DefaultExt = "xlsx",
            FileName = "Steel design results.xlsx"
        };

        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
    }
}