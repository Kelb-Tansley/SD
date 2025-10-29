using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;

/// <summary>
/// A structural T section where the flange and webs yield stress may differ but the modulas of elasticity is the same.
/// For T sections, the local (as per Strand7 properties) x-x axis is the major axis. But, this can be opposite for tees cut from H sections.
/// </summary>
public class TSection : Section
{
    /// <summary>
    /// Height of the web. Element 2 height.
    /// </summary>
    public double Hw { get; private set; }
    public double Ixx { get; private set; }
    public double Iyy { get; private set; }

    public bool IsYMajor { get => Iyy > Ixx; }

    public TSection(double b,
                    double d,
                    double t1,
                    double t2,
                    Material material,
                    double? agr = null,
                    double? ceMajor = null,
                    double? ceMinor = null,
                    double? iMajor = null,
                    double? iMinor = null,
                    double? j = null) : base(SectionType.T, material)
    {
        B1 = b;
        B2 = 0;
        D = d;
        T1 = t1;
        T2 = t2;
        T3 = 0;

        InitialiseSectionProperties(agr: agr,
                                    ceMajor: ceMajor,
                                    ceMinor: ceMinor,
                                    iMajor: iMajor,
                                    iMinor: iMinor,
                                    j: j);
    }

    private void InitialiseSectionProperties(double? agr, double? ceMajor, double? ceMinor, double? iMajor, double? iMinor, double? j)
    {
        SetHw();
        SetAgr(agr);
        SetYeNA();
        SetIxx();
        SetIyy();
        SetCeMajor(ceMajor);
        SetCeMinor(ceMinor);
        SetAMajor();
        SetAMinor();
        SetIMajor(iMajor);
        SetIMinor(iMinor);
        SetRMajor();
        SetRMinor();
        SetZeMajor();
        SetZeMinor();
        SetZplMajor();
        SetZplMinor();
        SetJ(j);
        SetCw();
    }

    private void SetHw()
    {
        Hw = D - T1;
    }
    private void SetAgr(double? value = null)
    {
        Agr = value != null ? (double)value : Ag1 + Ag2;
    }
    private void SetCeMajor(double? value = null)
    {
        CeMajor = value != null ? (double)value : IsYMajor ? B1 / 2 : D - YeNA;
    }
    private void SetCeMinor(double? value = null)
    {
        CeMinor = value != null ? (double)value : IsYMajor ? D - YeNA : B1 / 2;
    }
    private void SetAMajor(double? value = null)
    {
        AMajor = value != null ? (double)value : IsYMajor ? 0 : YeNA - T1 / 2;
    }
    private void SetAMinor(double? value = null)
    {
        AMinor = value != null ? (double)value : IsYMajor ? YeNA - T1 / 2 : 0;
    }
    private void SetIMajor(double? value = null)
    {
        IMajor = value != null ? (double)value : Math.Max(Ixx, Iyy);
    }
    private void SetIMinor(double? value = null)
    {
        IMinor = value != null ? (double)value : Math.Min(Ixx, Iyy);
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
        ZeMajor = value != null ? (double)value : IsYMajor ? IMajor / CeMajor : IMajor / Math.Max(CeMajor, YeNA);
        ZeMajorTop = IsYMajor ? ZeMajor : IMajor / YeNA;
        ZeMajorBottom = IsYMajor ? ZeMajor : IMajor / CeMajor;
    }
    private void SetZeMinor(double? value = null)
    {
        ZeMinor = value != null ? (double)value : IsYMajor ? IMinor / Math.Max(CeMinor, D - YeNA) : IMinor / CeMinor;
        ZeMinorTop = IsYMajor ? IMinor / YeNA : ZeMinor;
        ZeMinorBottom = IsYMajor ? IMinor / CeMinor : ZeMinor;
    }
    private void SetZplMajor(double? value = null)
    {
        ZplMajor = value != null ? (double)value : IsYMajor ? CalculateZplyy() : CalculateZplxx();
    }
    private void SetZplMinor(double? value = null)
    {
        ZplMinor = value != null ? (double)value : IsYMajor ? CalculateZplxx() : CalculateZplyy();
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : (B1 * Math.Pow(T1, 3) + (D - T1 / 2) * Math.Pow(T2, 3)) / 3;
    }
    private void SetCw(double? value = null)
    {
        Cw = value != null ? (double)value : Math.Pow(B1, 3) * Math.Pow(T1, 3) / 144 + Math.Pow(D - T1 / 2, 3) * Math.Pow(T2, 3) / 36;
    }
    private void SetIxx(double? value = null)
    {
        Ixx = value != null ? (double)value : T2 * Math.Pow(D, 3) / 3 + (B1 - T2) * Math.Pow(T1, 3) / 3 - Agr * Math.Pow(YeNA, 2);
    }
    private void SetIyy(double? value = null)
    {
        Iyy = value != null ? (double)value : (D - T1) * Math.Pow(T2, 3) / 12 + T1 * Math.Pow(B1, 3) / 12;
    }
    private void SetYeNA()
    {
        YeNA = (T2 * Math.Pow(D, 2) + (B1 - T2) * Math.Pow(T1, 2)) / (2 * Agr);
    }

    private double CalculateZplyy()
    {
        return T1 * Math.Pow(B1, 2) / 4 + (D - T1) * Math.Pow(T2, 2) / 4;
    }
    private double CalculateZplxx()
    {
        if (T1 > Agr / (2 * B1))
        {
            //Plastic neutral axis lands inside the flange
            var yPNA = Agr / (2 * B1);
            return B1 * Math.Pow(yPNA, 2) / 2 + T2 * Math.Pow(D - yPNA, 2) / 2 + (B1 - T2) * Math.Pow(T1 - yPNA, 2) / 2;
        }
        else if (T1 <= Agr / (2 * B1))
        {
            //Plastic neutral axis lands outside the flange
            var yPNA = D - Agr / (2 * T2);
            return T2 * Math.Pow(D - yPNA, 2) / 2 + B1 * Math.Pow(yPNA, 2) / 2 - (B1 - T2) * Math.Pow(yPNA - T1, 2) / 2;
        }
        return 0;
    }

    public double Ag1 => B1 * T1;
    public double Ag2 => Hw * T2;

    /// <summary>
    /// Elastic neutral axis, from top of flange.
    /// </summary>
    private double YeNA { get; set; }

    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = false;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = true;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{B1.Cut(1)} {TypeDisplay}";
    protected override string GetTypeDisplay() => "T";
}
