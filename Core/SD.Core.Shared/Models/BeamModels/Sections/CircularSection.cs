using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;
public class CircularSection : Section
{
    public CircularSection(double d,
                           double t,
                           Material material,
                           double? agr = null,
                           double? iMajor = null,
                           double? iMinor = null,
                           double? j = null) : base(SectionType.CircularHollow, material)
    {
        B1 = 0;
        B2 = 0;
        D = d;
        T1 = t;
        T2 = 0;
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
        Agr = value != null ? (double)value : Math.PI * (Math.Pow(D, 2) - Math.Pow(D - 2 * T1, 2)) / 4;
    }
    private void SetIMajor(double? value = null)
    {
        IMajor = value != null ? (double)value : Math.PI * (Math.Pow(D, 4) - Math.Pow(D - 2 * T1, 4)) / 64;
    }
    private void SetIMinor(double? value = null)
    {
        IMinor = value != null ? (double)value : Math.PI * (Math.Pow(D, 4) - Math.Pow(D - 2 * T1, 4)) / 64;
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
        ZeMinor = value != null ? (double)value : IMinor / (D / 2);
    }
    private void SetZplMajor(double? value = null)
    {
        ZplMajor = value != null ? (double)value : (Math.Pow(D, 3) - Math.Pow(D - 2 * T1, 3)) / 6;
    }
    private void SetZplMinor(double? value = null)
    {
        ZplMinor = value != null ? (double)value : (Math.Pow(D, 3) - Math.Pow(D - 2 * T1, 3)) / 6;
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : 2 * IMajor;
    }

    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = true;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = true;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{T1.Cut(1)} {TypeDisplay}";
    protected override string GetTypeDisplay() => "CHS";
}
