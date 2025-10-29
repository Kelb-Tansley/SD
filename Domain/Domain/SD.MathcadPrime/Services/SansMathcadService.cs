using SD.Core.Shared.Models.Sans;
using SD.Core.Shared.Models;
using SD.Core.Shared.Contracts;
using SD.MathcadPrime.Interfaces;
using SD.Core.Shared.Enum;
using SD.Element.Design.Sans.Constants;
using SD.Core.Infrastructure.Interfaces;

namespace SD.MathcadPrime.Services;

public partial class SansMathcadService(INotificationService notificationService, IAppSettings appSettings) : MathcadService(notificationService, appSettings), ISansMathcadService
{
    public SansCapacity ReadSansMathcadResults(string mathcadSheet)
    {
        return new SansCapacity()
        {
            SlendernessMajor = _mathcadPrimeWorksheet.OutputGetRealValue("λx").RealResult,
            SlendernessMinor = _mathcadPrimeWorksheet.OutputGetRealValue("λy").RealResult,
            Tr = _mathcadPrimeWorksheet.OutputGetRealValue("Tr").RealResult,
            CrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Crx").RealResult,
            CrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Cry").RealResult,
            MrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Mry").RealResult,
            MrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Mrxu").RealResult,
            VrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Vrx").RealResult,
            VrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Vry").RealResult,
        };
    }

    public void ExportToMathcadFile(string? templateFile, SansUlsResult asUlsResult, bool saveFile = false)
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

    private void SetMathcadRegions(SansUlsResult sansUlsResult)
    {
        const int digits = 3;
        var beam = sansUlsResult.Beam;

        if (_mathcadPrimeWorksheet == null)
            throw new ArgumentNullException("Mathcad prime worksheet has failed to be initialised.");

        _mathcadPrimeWorksheet.PauseCalculation();
        //_mathcadPrimeWorksheet.SetStringValue("Member_Description", beam.Section.DisplayName);
        //_mathcadPrimeWorksheet.SetStringValue("Strand7_MemberID", beam.Number.ToString());
        //_mathcadPrimeWorksheet.SetStringValue("Load_Case_CombinationID", asUlsResult.LoadCaseNumber.ToString());

        _mathcadPrimeWorksheet.SetRealValue("Fabtype", beam.Section.IsPlateGirder ? 2 : 1, null);
        _mathcadPrimeWorksheet.SetRealValue("D", Math.Round(beam.Section.D, digits), "mm");


        //_mathcadPrimeWorksheet.SetRealValue("h", Math.Round(beam.Section.D, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("Bf", Math.Round(beam.Section.B1, digits), "mm");
        //_mathcadPrimeWorksheet.SetRealValue("b", Math.Round(beam.Section.B1, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("tw", Math.Round(beam.Section.T3, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("tf", Math.Round(beam.Section.T1, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("Ix", Math.Round(beam.Section.IMajor, digits), "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("Iy", Math.Round(beam.Section.IMinor, digits), "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("Zex", Math.Round(beam.Section.ZeMajor, digits), "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zpx", Math.Round(beam.Section.ZplMajor, digits), "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zey", Math.Round(beam.Section.ZeMinor, digits), "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("Zpy", Math.Round(beam.Section.ZplMinor, digits), "mm^3");
        _mathcadPrimeWorksheet.SetRealValue("rx", Math.Round(beam.Section.RMajor, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("ry", Math.Round(beam.Section.RMinor, digits), "mm");
        _mathcadPrimeWorksheet.SetRealValue("Jt", Math.Round(beam.Section.J, digits), "mm^4");
        _mathcadPrimeWorksheet.SetRealValue("Iw", Math.Round(beam.Section.Cw, digits), "mm^6");
        //_mathcadPrimeWorksheet.SetRealValue("Cw", Math.Round(beam.Section.Cw, digits), "mm^6");
        _mathcadPrimeWorksheet.SetRealValue("Ag", Math.Round(beam.Section.Agr, digits), "mm^2");

        _mathcadPrimeWorksheet.SetRealValue("Nf", -1 * sansUlsResult.Loads.Cu, "kN"); // In sheet, compression is positive, tension is negative
                                                                                      //_mathcadPrimeWorksheet.SetRealValue("Cu", Math.Abs(asUlsResult.Loads.Cu), "kN"); 
                                                                                      // _mathcadPrimeWorksheet.SetRealValue("Tu", asUlsResult.Loads.Tu, "kN");
        _mathcadPrimeWorksheet.SetRealValue("Mfx", sansUlsResult.Loads.MuMajor, "kN*m"); // In sheet, positive causes compression at top of section
        //_mathcadPrimeWorksheet.SetRealValue("Mux", asUlsResult.Loads.MuMajor, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("Mfy", sansUlsResult.Loads.MuMinor, "kN*m"); // In sheet, positive causes compression at right of section
        //_mathcadPrimeWorksheet.SetRealValue("Muy", asUlsResult.Loads.MuMinor, "kN*m");
        _mathcadPrimeWorksheet.SetRealValue("Vfx", sansUlsResult.Loads.VuMajor, "kN"); //Shear force parallel to x-axis.
        //_mathcadPrimeWorksheet.SetRealValue("Vux", asUlsResult.Loads.VuMajor, "kN");
        _mathcadPrimeWorksheet.SetRealValue("Vfy", sansUlsResult.Loads.VuMinor, "kN");//Shear force parallel to y-axis.
                                                                                      //_mathcadPrimeWorksheet.SetRealValue("Vuy", asUlsResult.Loads.VuMinor, "kN");
        _mathcadPrimeWorksheet.SetRealValue("MfxA", Math.Round(sansUlsResult.Forces.StartMuMajor / 1000000, digits), "kN*m");//Factored moments about x axis at end 'A' of member.
        _mathcadPrimeWorksheet.SetRealValue("MfxB", Math.Round(sansUlsResult.Forces.EndMuMajor / 1000000, digits), "kN*m");//Factored moments about x axis at end 'B' of member.
        _mathcadPrimeWorksheet.SetRealValue("Curvature", sansUlsResult.Capacity.BendingConstants.Curvature, null);

        _mathcadPrimeWorksheet.SetRealValue("fy", beam.Section.Material.FyElement1, "MPa");
        _mathcadPrimeWorksheet.SetRealValue("ϕs", SansMaterialConstants.Φ, null);
        _mathcadPrimeWorksheet.SetRealValue("fu", beam.Section.Material.FuElement1, "MPa");
        _mathcadPrimeWorksheet.SetRealValue("Es", beam.Section.Material.Es, "MPa");
        _mathcadPrimeWorksheet.SetRealValue("Gs", beam.Section.Material.Gs, "MPa");


        _mathcadPrimeWorksheet.SetRealValue("Sstf", beam.Section.WebStiffenerSpacing, null);
        _mathcadPrimeWorksheet.SetRealValue("Sstf", beam.Section.WebStiffenerSpacing, null);

        switch (sansUlsResult.Beam.Section.SectionType)
        {
            case SectionType.IorH:
                if (sansUlsResult.AxialClass != null)
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classaf", Math.Max((int)sansUlsResult.AxialClass.Element1, (int)sansUlsResult.AxialClass.Element2), null);
                    _mathcadPrimeWorksheet.SetRealValue("Classaw", (int)sansUlsResult.AxialClass.Element3, null);
                }
                else
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classaf", 0, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classaw", 0, null);
                }

                if (sansUlsResult.FlexuralClass != null)
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classff", Math.Max((int)sansUlsResult.FlexuralClass.Element1, (int)sansUlsResult.FlexuralClass.Element2), null);
                    _mathcadPrimeWorksheet.SetRealValue("Classfw", (int)sansUlsResult.FlexuralClass.Element3, null);
                }
                else
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classff", 0, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classfw", 0, null);
                }

                _mathcadPrimeWorksheet.SetRealValue("Lx", Math.Round(beam.BeamChain.L1, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("Ly", Math.Round(beam.BeamChain.L2, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("Lz", Math.Round(beam.BeamChain.Lz, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("LuTop", Math.Round(beam.BeamChain.LeTop, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("LuBottom", Math.Round(beam.BeamChain.LeBottom, digits), "mm");

                _mathcadPrimeWorksheet.SetRealValue("Kx", beam.BeamChain.K2, null);
                _mathcadPrimeWorksheet.SetRealValue("Ky", beam.BeamChain.K1, null);
                //_mathcadPrimeWorksheet.SetRealValue("Kz", beam.BeamChain.K_z, null);
                _mathcadPrimeWorksheet.SetRealValue("KeTop", beam.BeamChain.KeTop, null);
                _mathcadPrimeWorksheet.SetRealValue("KeBottom", beam.BeamChain.KeBottom, null);
                _mathcadPrimeWorksheet.SetRealValue("Cx", Math.Round(beam.Section.CeMinor, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("Cy", Math.Round(beam.Section.CeMajor, digits), "mm");
                break;
            case SectionType.LipChannel:
                if (sansUlsResult.AxialClass != null)
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classaf", (int)sansUlsResult.AxialClass.Element1, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classaw", (int)sansUlsResult.AxialClass.Element3, null);
                }
                else
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classaf", 0, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classaw", 0, null);
                }

                if (sansUlsResult.FlexuralClass != null)
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classff", (int)sansUlsResult.FlexuralClass.Element1, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classfw", (int)sansUlsResult.FlexuralClass.Element3, null);
                }
                else
                {
                    _mathcadPrimeWorksheet.SetRealValue("Classff", 0, null);
                    _mathcadPrimeWorksheet.SetRealValue("Classfw", 0, null);
                }

                _mathcadPrimeWorksheet.SetRealValue("Lx", Math.Round(beam.BeamChain.L2, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("Ly", Math.Round(beam.BeamChain.L1, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("Lz", Math.Round(beam.BeamChain.Lz, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("LuTop", Math.Round(beam.BeamChain.LeTop, digits), "mm");
                _mathcadPrimeWorksheet.SetRealValue("LuBottom", Math.Round(beam.BeamChain.LeBottom, digits), "mm");

                _mathcadPrimeWorksheet.SetRealValue("Kx", beam.BeamChain.K2, null);
                _mathcadPrimeWorksheet.SetRealValue("Ky", beam.BeamChain.K1, null);
                //_mathcadPrimeWorksheet.SetRealValue("Kz", beam.BeamChain.K_z, null);
                _mathcadPrimeWorksheet.SetRealValue("KeTop", beam.BeamChain.KeTop, null);
                _mathcadPrimeWorksheet.SetRealValue("KeBottom", beam.BeamChain.KeBottom, null);
                _mathcadPrimeWorksheet.SetRealValue("Cx", Math.Round(beam.Section.CeMinor, digits), "mm");
                break;
            case SectionType.Angle:
                break;
            case SectionType.CircularHollow:
                break;
            case SectionType.RectangularHollow:
                break;
            case SectionType.T:
                break;
            case SectionType.Unknown:
                break;
            default:
                break;
        }

        //_mathcadPrimeWorksheet.SetRealValue("Classweb", (int)asUlsResult.Classification.Element3, null);
        //_mathcadPrimeWorksheet.SetRealValue("Classflange", (int)asUlsResult.Classification.Element1, null);

        _mathcadPrimeWorksheet.SetRealValue("ω2_case", sansUlsResult.Capacity.BendingConstants.Loadω2Case, null);
        _mathcadPrimeWorksheet.SetRealValue("ω1_case", sansUlsResult.Capacity.BendingConstants.Loadω1Case, null);

        _mathcadPrimeWorksheet.SetRealValue("Membr", beam.Section.IsBracedFrame ? 1 : 2, null);
        _mathcadPrimeWorksheet.SetRealValue("NSF", sansUlsResult.Beam.Section.NetAreaFactor, null);

        _mathcadPrimeWorksheet.ResumeCalculation();
    }
}