using SD.Core.Shared.Models;
using SD.Core.Shared.Contracts;
using SD.MathcadPrime.Interfaces;
using SD.Core.Shared.Models.AS;
using SD.Core.Shared.Enum;
using SD.Element.Design.AS.Enums;
using SD.Element.Design.AS.Services;
using System.Diagnostics;
using SD.Core.Infrastructure.Interfaces;

namespace SD.MathcadPrime.Services;

public partial class AsMathcadService(INotificationService notificationService, IAppSettings appSettings) : MathcadService(notificationService, appSettings), IAsMathcadService
{
    public ASCapacity ReadASMathcadCombinationResults(string mathcadSheet)
    {
        return new ASCapacity()
        {
            Tr = _mathcadPrimeWorksheet.OutputGetRealValue("Nt").RealResult,
            Cr = Math.Min(_mathcadPrimeWorksheet.OutputGetRealValue("Ncx").RealResult, _mathcadPrimeWorksheet.OutputGetRealValue("Ncy").RealResult),
            MrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Mbx").RealResult,
            MrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Msy").RealResult,
            MajorBendingShear = _mathcadPrimeWorksheet.OutputGetRealValue("Vvmx").RealResult,
            MinorBendingShear = _mathcadPrimeWorksheet.OutputGetRealValue("Vvmy").RealResult,
            MajorSectionBendingTensionMrx = _mathcadPrimeWorksheet.OutputGetRealValue("Mrtx").RealResult,
            MinorSectionBendingTensionMry = _mathcadPrimeWorksheet.OutputGetRealValue("Mrty").RealResult,
            MajorSectionBendingCompressionMrx = _mathcadPrimeWorksheet.OutputGetRealValue("Mrcx").RealResult,
            MinorSectionBendingCompressionMry = _mathcadPrimeWorksheet.OutputGetRealValue("Mrcy").RealResult,
            MajorMemberBendingCompressionMix = _mathcadPrimeWorksheet.OutputGetRealValue("Micx").RealResult,
            MinorMemberBendingCompressionMiy = _mathcadPrimeWorksheet.OutputGetRealValue("Micy").RealResult,
            MajorMemberBendingCompressionMox = _mathcadPrimeWorksheet.OutputGetRealValue("Mocx").RealResult,
            MajorMemberBendingTensionMox = _mathcadPrimeWorksheet.OutputGetRealValue("Motx").RealResult
        };
    }

    public void ExportToMathcadFile(string? templateFile, ASUlsResult asUlsResult, bool saveFile = false)
    {
        try
        {
            var result = CopyAndOpenTemplateWorksheet(templateFile, asUlsResult.Beam.Section.SectionType);
            if (!result)
                return;

            SetMathcadRegions(asUlsResult);

            if (!saveFile)
                return;

            var savedAsFileName = SaveAsWorksheet(GetMathCadFileName(asUlsResult.Beam.Section.SectionType), true);
            if (!string.IsNullOrWhiteSpace(savedAsFileName))
                ShowMathcad();
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", $"Unable to populate worksheet. {ex.Message}"));
        }
    }

    private void SetMathcadRegions(ASUlsResult asUlsResult) // TODO: UNITS?
    {
        if (_mathcadPrimeWorksheet == null)
            throw new ArgumentNullException("Mathcad prime worksheet has failed to be initialised.");

        var beam = asUlsResult.Beam;
        var forces = asUlsResult.Forces;
        var bendingConstants = asUlsResult.Capacity.BendingConstants;
        var material = beam.Section.Material;
        var minFlangeFy = 0D;
        var minWebFy = 0D;
        var tw = 0D;

        switch (beam.Section.SectionType)
        {
            case SectionType.IorH:
                minFlangeFy = Math.Min(material.FyElement1, material.FyElement2);
                minWebFy = material.FyElement3;
                tw = beam.Section.T3;
                break;

            case SectionType.LipChannel:
            case SectionType.CircularHollow:
            case SectionType.RectangularHollow:
            case SectionType.T:
                minFlangeFy = material.FyElement1;
                minWebFy = material.FyElement2;
                tw = beam.Section.T2;
                break;
            case SectionType.Angle:
                minFlangeFy = material.MinFy;
                minWebFy = material.MinFy;
                break;
        }

        // Section Properties
        _mathcadPrimeWorksheet.SetRealValue("D", beam.Section.D, "mm");
        _mathcadPrimeWorksheet.SetRealValue("Bf", beam.Section.B1, "mm");
        _mathcadPrimeWorksheet.SetRealValue("tf", beam.Section.T1, "mm");
        _mathcadPrimeWorksheet.SetRealValue("tw", tw, "mm");
        _mathcadPrimeWorksheet.SetRealValue("Ag", beam.Section.Agr, "mm^2");
        _mathcadPrimeWorksheet.SetRealValue("Ix", beam.Section.IMajor, "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("Iy", beam.Section.IMinor, "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("rx", beam.Section.RMajor, "mm");
        _mathcadPrimeWorksheet.SetRealValue("ry", beam.Section.RMinor, "mm");
        _mathcadPrimeWorksheet.SetRealValue("Zex", beam.Section.ZeMajor, "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zey", beam.Section.ZeMinor, "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zpx", beam.Section.ZplMajor, "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zpy", beam.Section.ZplMinor, "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Jt", beam.Section.J, "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("Iw", beam.Section.Cw, "mm^6");

        //_mathcadPrimeWorksheet.SetRealValue("df", beam.Section.df, "mm");
        //_mathcadPrimeWorksheet.SetRealValue("Cx", beam.Section.Cx, "mm^4");
        //_mathcadPrimeWorksheet.SetRealValue("Cy", beam.Section.Cy, "mm^4");
        //_mathcadPrimeWorksheet.SetRealValue("Alpha", beam.Section.Alpha, "deg");
        //_mathcadPrimeWorksheet.SetRealValue("mu", x, "kg/m");

        // Loading Properties
        _mathcadPrimeWorksheet.SetRealValue("Mmx", forces.MaxAbsMuMajor / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("Mmy", forces.MaxAbsMuMinor / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M2x", bendingConstants.MuMajorQuarter / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M3x", bendingConstants.MuMajorHalf / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M4x", bendingConstants.MuMajorThreeQuarter / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M2y", bendingConstants.MuMinorQuarter / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M3y", bendingConstants.MuMinorHalf / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("M4y", bendingConstants.MuMinorThreeQuarter / 1000000, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("Vmx", forces.MaxAbsVuMajor / 1000, "kN");
        _mathcadPrimeWorksheet.SetRealValue("Vmy", forces.MaxAbsVuMinor / 1000, "kN");
        _mathcadPrimeWorksheet.SetRealValue("Nm", forces.MaxAxialForce / 1000, "kN");

        // Material and Fabrication Properties
        _mathcadPrimeWorksheet.SetRealValue("fy", minFlangeFy, "MPa");
        _mathcadPrimeWorksheet.SetRealValue("fyw", minWebFy, "MPa");
        _mathcadPrimeWorksheet.SetRealValue("fu", material.FuElement1, "MPa");

        // Member Properties
        Debug.Assert(beam.BeamChain.L1 == beam.BeamChain.L2); // TODO: Handle different values

        _mathcadPrimeWorksheet.SetRealValue("Restraint", ASMomentService.HasFullLateralRestraint(beam, BeamAxisPart.MajorTop, ASMomentService.GetSectionModulus(beam.Section, BeamAxisPart.MajorTop)) ? 1 : 2, "");
        _mathcadPrimeWorksheet.SetRealValue("Lm", beam.BeamChain.L2 / 1000, "m");
        _mathcadPrimeWorksheet.SetRealValue("Kt_Bending", beam.Kt_Bending, "");
        _mathcadPrimeWorksheet.SetRealValue("Kr", beam.Kr, "");
        _mathcadPrimeWorksheet.SetRealValue("alpha_muser", Math.Min((1.7D * forces.MaxAbsMuMajor) /
                                                           Math.Sqrt(Math.Pow(bendingConstants.MuMajorQuarter, 2) +
                                                           Math.Pow(bendingConstants.MuMajorHalf, 2) +
                                                           Math.Pow(bendingConstants.MuMajorThreeQuarter, 2)), 2.5D), "");
        _mathcadPrimeWorksheet.SetRealValue("Kl", beam.Kl, "");
        _mathcadPrimeWorksheet.SetRealValue("ke", beam.BeamChain.K2, "");
    }
}