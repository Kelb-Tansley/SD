using Microsoft.Win32;
using OfficeOpenXml;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Sans;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SD.Core.Infrastructure.Services;

public class UlsDataExportService : IUlsDataExportService
{
    private readonly INotificationService _notificationService;

    public UlsDataExportService(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void ExportToExcel(IEnumerable<SansUlsResult> rows)
    {
        var results = rows.ToList();
        if (results.Count == 0)
            return;

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel Files|*.xlsx",
            DefaultExt = "xlsx",
            FileName = "ULS data export.xlsx"
        };

        if (saveFileDialog.ShowDialog() != true)
            return;

        ExcelPackage.License.SetNonCommercialOrganization("Aurestruct");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("ULS Data");

        ws.Cells.Style.Font.Name = "Arial";
        ws.Cells.Style.Font.Size = 10;

        var headers = new[]
        {
            "Beam", "LCC", "Section", "Peak %",
            "K2", "K1", "Kz", "Ke", "KeB", "Reason",
            "L(2) [mm]", "L(1) [mm]", "L(z) [mm]", "L(e) [mm]",
            "Cr [kN]", "Tr [kN]", "Vrx-x [kN]", "Vry-y [kN]", "Mrx-x [kN.m]", "Mry-y [kN.m]",
            "Tension", "Compression", "Major Bend", "Minor Bend",
            "Major Shear", "Minor Shear", "Biaxial", "Biaxial+Axial",
            "Von Mises [MPa]", "Tension [N]", "Compression [N]",
            "Mu Major [kN.m]", "Mu Minor [kN.m]", "Vu Major [kN]", "Vu Minor [kN]",
            "Section Name", "Section Type", "Ag [mm²]", "Mass [kg/m]", "J [mm⁴]", "Cw [mm⁶]",
            "I Major [mm⁴]", "I Minor [mm⁴]", "Ze Major [mm³]", "Ze Minor [mm³]", "Zpl Major [mm³]", "Zpl Minor [mm³]"
        };

        for (int col = 0; col < headers.Length; col++)
            ws.Cells[1, col + 1].Value = headers[col];

        for (int i = 0; i < results.Count; i++)
        {
            var r = results[i];
            int row = i + 2;
            int col = 1;

            ws.Cells[row, col++].Value = r.Beam.Number;
            ws.Cells[row, col++].Value = r.LoadCaseNumber;
            ws.Cells[row, col++].Value = r.Beam.Section.DisplayName;
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.MaxUtilizationPercentage, 2);

            ws.Cells[row, col++].Value = r.Beam.BeamChain.K2;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.K1;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.Kz;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.KeTop;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.KeBottom;
            ws.Cells[row, col++].Value = r.Utilization.MaxUtilizationDescription;

            ws.Cells[row, col++].Value = r.Beam.BeamChain.L2;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.L1;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.Lz;
            ws.Cells[row, col++].Value = r.Beam.BeamChain.LeTop;

            ws.Cells[row, col++].Value = Math.Round(r.Capacity.Cr, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Capacity.Tr, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Capacity.VrMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Capacity.VrMinor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Capacity.MrMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Capacity.MrMinor, 2);

            ws.Cells[row, col++].Value = Math.Round(r.Utilization.Tension * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.Compression * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.BendingMajor * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.BendingMinor * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.ShearMajor * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.ShearMinor * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.BiAxialBending * 100, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Utilization.CompressionAndBendingMemberStrength * 100, 2);

            ws.Cells[row, col++].Value = Math.Round(r.Forces.VonMises, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.Tension, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.Compression, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.MaxAbsMuMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.MaxAbsMuMinor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.MaxAbsVuMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Forces.MaxAbsVuMinor, 2);

            ws.Cells[row, col++].Value = r.Beam.Section.Name;
            ws.Cells[row, col++].Value = r.Beam.Section.TypeDisplay;
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.Agr, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.SectionMass, 3);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.J, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.Cw, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.IMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.IMinor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.ZeMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.ZeMinor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.ZplMajor, 2);
            ws.Cells[row, col++].Value = Math.Round(r.Beam.Section.ZplMinor, 2);

        }

        package.SaveAs(new FileInfo(saveFileDialog.FileName));

        var openResult = _notificationService.NotifyUserWithYesNoOption(
            new Notification("Export Complete", "ULS data exported successfully. Would you like to open the file?"));

        if (openResult == MessageBoxResult.Yes)
            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
    }
}
