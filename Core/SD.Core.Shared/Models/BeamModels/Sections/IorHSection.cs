using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;
public class IorHSection : Section
{
    /// <summary>
    /// Height of the web. Element 3 height.
    /// </summary>
    public double Hw { get; private set; }

    public IorHSection(double b1,
                       double b2,
                       double d,
                       double t1,
                       double t2,
                       double t3,
                       Material material,
                       double? agr = null,
                       double? ceMajor = null,
                       double? ceMinor = null,
                       double? iMajor = null,
                       double? iMinor = null,
                       double? j = null,
                       double? aMajor = null,
                       double? aMinor = null) : base(SectionType.IorH, material)
    {
        B1 = b1;
        B2 = b2;
        D = d;
        T1 = t1;
        T2 = t2;
        T3 = t3;

        InitialiseSectionProperties(agr: agr,
                                    ceMajor: ceMajor,
                                    ceMinor: ceMinor,
                                    iMajor: iMajor,
                                    iMinor: iMinor,
                                    j: j,
                                    aMajor: aMajor,
                                    aMinor: aMinor);
    }

    private void InitialiseSectionProperties(double? agr, double? ceMajor, double? ceMinor, double? iMajor, double? iMinor, double? j, double? aMajor, double? aMinor)
    {
        SetHw();
        SetAgr(agr);
        SetYeNA();
        SetCeMajor(ceMajor);
        SetCeMinor(ceMinor);
        SetAMajor(aMajor);
        SetAMinor(aMinor);
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
        Hw = D - T1 - T2;
    }
    private void SetAgr(double? value = null)
    {
        Agr = value != null ? (double)value : Ag1 + Ag2 + Ag3;
    }
    private void SetCeMajor(double? value = null)
    {
        CeMajor = value != null ? (double)value : D - YeNA;
    }
    private void SetCeMinor(double? value = null)
    {
        CeMinor = value != null ? (double)value : B1 / 2;
    }
    private void SetAMajor(double? value = null)
    {
        AMajor = value != null ? (double)value : 0;
    }
    private void SetAMinor(double? value = null)
    {
        AMinor = value != null ? (double)value : 0;
    }
    private void SetIMajor(double? value = null)
    {
        IMajor = value != null ? (double)value : CalculateIMajor();
    }
    private void SetIMinor(double? value = null)
    {
        IMinor = value != null ? (double)value : CalculateIMinor();
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
        ZeMajor = value != null ? (double)value : IMajor / Math.Max(YeNA, D - YeNA);
    }
    private void SetZeMinor(double? value = null)
    {
        ZeMinor = value != null ? (double)value : IMinor / (Math.Max(B1, B2) / 2); // TODO: ADD TOP AND BOTTOM FOR ASCODE
    }
    private void SetZplMajor(double? value = null)
    {
        ZplMajor = value != null ? (double)value : CalculateZplMajor();
    }
    private void SetZplMinor(double? value = null)
    {
        ZplMinor = value != null ? (double)value : T1 * Math.Pow(B1, 2) / 4 + T2 * Math.Pow(B2, 2) / 4 + (Hw) * Math.Pow(T3, 2) / 4;
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : 0;
    }
    private void SetCw(double? value = null)
    {
        Cw = value != null ? (double)value : CalculateCw();
    }
    private void SetYeNA()
    {
        // Calculate the area of element 3
        var H3 = Hw;
        var Ag3 = H3 * T3;

        // Calculate the centroid of the section from the top down
        var y1 = D - T1 / 2;
        var y2 = T2 / 2;
        var y3 = T2 + (H3 / 2);

        var yNA = (Ag1 * y1 + Ag3 * y3 + Ag2 * y2) / (Ag1 + Ag3 + Ag2);

        if (yNA < 0 || yNA > H3 + T1 || Math.Abs(yNA) < T1)
            throw new NotImplementedException("The elastic neutral axis has landed outside the limits.");

        YeNA = yNA;
    }

    private double CalculateIMajor()
    {
        // Calculate the area of element 3
        var H3 = Hw;
        var Ag3 = H3 * T3;

        // Calculate the centroid of the section elements
        var y1 = D - T1 / 2;
        var y2 = T2 / 2;
        var y3 = T2 + (H3 / 2);

        // Calculate the moment of inertia about the strong axis
        var i1 = B1 * Math.Pow(T1, 3) / 12 + Ag1 * Math.Pow(y1 - YeNA, 2);
        var i3 = T3 * Math.Pow(H3, 3) / 12 + Ag3 * Math.Pow(y3 - YeNA, 2);
        var i2 = B2 * Math.Pow(T2, 3) / 12 + Ag2 * Math.Pow(y2 - YeNA, 2);
        return i1 + i3 + i2;
    }
    private double CalculateIMinor()
    {
        // Calculate the moment of inertia about the weak axis
        var i1 = T1 * Math.Pow(B1, 3) / 12;
        var i3 = Hw * Math.Pow(T3, 3) / 12;
        var i2 = T2 * Math.Pow(B2, 3) / 12;
        return i1 + i3 + i2;
    }
    private double CalculateZplMajor()
    {
        // Calculate the plastic neutral axis of the section
        var y1 = T1 / 2;
        var y2 = D - T2 / 2;
        var yNA = (Hw + 2 * T1 + (Ag2 - Ag1) / T3) / 2;

        if (yNA < 0 || yNA > Hw + T1 || Math.Abs(yNA) < T1)
            throw new NotImplementedException("The plastic neutral axis has landed outside the limits.");

        // Calculate plastic modulus of the section
        var y3Above = (Hw + T1 - yNA) / 2;
        var Ag3Above = y3Above * 2 * T3;

        var y3Below = (yNA - T1) / 2;
        var Ag3Below = y3Below * 2 * T3;

        return Ag1 * (yNA - y1) + Ag2 * (y2 - yNA) + Ag3Above * y3Above + Ag3Below * y3Below;
    }
    private double CalculateCw()
    {
        var a = YeNA + AMajor - T2 / 2;
        var p = 1 - a / (D - T1 / 2 - T2 / 2);

        return p * (1 - p) * IMinor * Math.Pow(D - T1 / 2 - T2 / 2, 2);
    }

    public double Ag1 => B1 * T1;
    public double Ag2 => B2 * T2;
    public double Ag3 => Hw * T3;

    /// <summary>
    /// Elastic neutral axis, from top of flange.
    /// </summary>
    private double YeNA { get; set; }

    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = true;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = true;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{Math.Max(B1, B2).Cut(1)} {TypeDisplay}";
    protected override string GetTypeDisplay() => IsH() ? "H" : "I";
    private bool IsH()
    {
        var error = 6D; //mm
        return Math.Abs(D - B1) < error && Math.Abs(D - B2) < error;
    }
}
