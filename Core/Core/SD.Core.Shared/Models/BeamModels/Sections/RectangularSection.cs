using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;
public class RectangularSection : Section
{
    public RectangularSection(double b,
                              double d,
                              double t1,
                              double t2,
                              Material material,
                              double? agr = null,
                              double? iMajor = null,
                              double? iMinor = null,
                              double? j = null) : base(SectionType.RectangularHollow, material)
    {
        B1 = b;
        B2 = 0;
        D = d;
        T1 = t1;
        T2 = t2;
        T3 = 0;

        InitialiseSectionProperties(agr: agr,
                                    iMajor: iMajor,
                                    iMinor: iMinor,
                                    j: j);
    }

    private void InitialiseSectionProperties(double? agr, double? iMajor, double? iMinor, double? j)
    {
        SetAgr(agr);
        SetIMajor(iMajor);
        SetIMinor(iMinor);
        SetRMajor();
        SetRMinor();
        SetZeMajor();
        SetZeMinor();
        SetZplMajor();
        SetZplMinor();
        SetJ(j);
    }

    private void SetAgr(double? value = null)
    {
        Agr = value != null ? (double)value : B1 * D - (B1 - 2 * T1) * (D - 2 * T1);
    }
    private void SetIMajor(double? value = null)
    {
        IMajor = value != null ? (double)value : (B1 * Math.Pow(D, 3) - (B1 - 2 * T1) * Math.Pow(D - 2 * T1, 3)) / 12;
    }
    private void SetIMinor(double? value = null)
    {
        IMinor = value != null ? (double)value : (D * Math.Pow(B1, 3) - (D - 2 * T1) * Math.Pow(B1 - 2 * T1, 3)) / 12;
    }
    private void SetRMajor(double? value = null)
    {
        RMajor = value != null ? (double)value : Math.Sqrt(IMajor / Agr);
    }
    private void SetRMinor(double? value = null)
    {
        RMinor = value != null ? (double)value : Math.Sqrt(IMinor / Agr);
    }
    private void SetZeMajor(double? value = null)
    {
        ZeMajor = value != null ? (double)value : IMajor / (D / 2);
    }
    private void SetZeMinor(double? value = null)
    {
        ZeMinor = value != null ? (double)value : IMinor / (B1 / 2);
    }
    private void SetZplMajor(double? value = null)
    {
        ZplMajor = value != null ? (double)value : (B1 * Math.Pow(D, 2) - (B1 - 2 * T2) * Math.Pow(D - 2 * T1, 2)) / 4;
    }
    private void SetZplMinor(double? value = null)
    {
        ZplMinor = value != null ? (double)value : (D * Math.Pow(B1, 2) - (D - 2 * T1) * Math.Pow(B1 - 2 * T2, 2)) / 4;
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : CalculateJ();
    }

    private double CalculateJ()
    {
        var rc = 1.5 * T1; // Mean corner radius
        var ap = (D - T1) * (B1 - T1) - Math.Pow(rc, 2) * (4 - Math.PI); // Enclosed area
        var p = 2 * (D - T1 + (B1 - T1)) - 2 * rc * (4 - Math.PI); // Mid-contour length

        return 4 * Math.Pow(ap, 2) * T1 / p;
    }

    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = true;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = true;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{B1.Cut(1)}x{T1.Cut(1)}{(T2 != T1 ? 'x' + T2.Cut(1).ToString() : string.Empty)} {TypeDisplay}";
    protected override string GetTypeDisplay() => D == B1 && D == B2 ? "SHS" : "RHS";
}
