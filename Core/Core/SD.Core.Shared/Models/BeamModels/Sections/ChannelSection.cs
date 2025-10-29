using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;
public class ChannelSection : Section
{
    /// <summary>
    /// Height of the web. Element 2 height.
    /// </summary>
    public double Hw { get; private set; }

    public ChannelSection(double b,
                          double d,
                          double t1,
                          double t2,
                          Material material,
                          double? agr = null,
                          double? ceMajor = null,
                          double? ceMinor = null,
                          double? iMajor = null,
                          double? iMinor = null,
                          double? j = null,
                          double? aMajor = null,
                          double? aMinor = null) : base(SectionType.LipChannel, material)
    {
        B1 = b;
        B2 = 0;
        D = d;
        T1 = t1;
        T2 = t2; //Web thickness
        T3 = 0;

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
        SetXeNA();
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
        Hw = D - T1 * 2;
    }
    private void SetAgr(double? value = null)
    {
        Agr = value != null ? (double)value : Ag1 * 2 + Ag3;
    }
    private void SetCeMajor(double? value = null)
    {
        CeMajor = value != null ? (double)value : D / 2;
    }
    private void SetCeMinor(double? value = null)
    {
        CeMinor = value != null ? (double)value : XeNA;
    }
    private void SetAMajor(double? value = null)
    {
        AMajor = value != null ? (double)value : 0;
    }
    private void SetAMinor(double? value = null)
    {
        AMinor = value != null ? (double)value : CeMinor + 3 * T1 * Math.Pow(B1 - T2 / 2, 2) / (6 * T1 * (B1 - T2 / 2) + (D - T1) * T2) - T2 / 2;
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
        ZeMajor = value != null ? (double)value : IMajor / (D / 2); // Strand7 Channels will always be symmetric about the major axis
    }
    private void SetZeMinor(double? value = null)
    {
        ZeMinor = value != null ? (double)value : CalculateZeMinor(); // Strand7 I and H sections will always be symmetric about the minor axis
    }
    private void SetZplMajor(double? value = null)
    {
        ZplMajor = value != null ? (double)value : CalculateZplMajor();
    }
    private void SetZplMinor(double? value = null)
    {
        ZplMinor = value != null ? (double)value : CalculateZplMinor();
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : (2 * (B1 - T2 / 2) * Math.Pow(T1, 3) + (D - T1) * Math.Pow(T2, 3)) / 3;
    }
    private void SetCw(double? value = null)
    {
        Cw = value != null ? (double)value : CalculateCw();
    }
    private void SetXeNA() => XeNA = (Hw * Math.Pow(T2, 2) / 2 + T1 * Math.Pow(B1, 2)) / Agr;

    private double CalculateIMajor(double? yNA = null)
    {
        yNA ??= D / 2; //All strand7 channel section are symmetric about the major axis

        // Calculate the area of element 3
        var Ag3 = Hw * T2;

        // Calculate the centroid of the section elements
        var y1 = D - T1 / 2;
        var y3 = T1 + (Hw / 2);

        // Calculate the moment of inertia about the strong axis
        var i1 = B1 * Math.Pow(T1, 3) / 12 + Ag1 * Math.Pow(y1 - (double)yNA, 2);
        var i3 = T2 * Math.Pow(Hw, 3) / 12 + Ag3 * Math.Pow(y3 - (double)yNA, 2);

        var iSectionMajor = i1 * 2 + i3;
        return iSectionMajor;
    }
    private double CalculateIMinor()
    {
        return Hw * Math.Pow(T2, 3) / 3 + 2 * (T1 * Math.Pow(B1, 3)) / 3 - Agr * Math.Pow(XeNA, 2);
    }
    private double CalculateZeMinor()
    {
        return IMinor / Math.Max(XeNA, B1 - XeNA);
    }

    private double CalculateZplMajor()
    {
        // Calculate the plastic neutral axis of the section
        var y1 = T1 / 2;
        var yNA = D / 2;

        // Calculate plastic modulus of the section
        var y3Above = (Hw + T1 - yNA) / 2;
        var Ag3Above = y3Above * 2 * T2;

        var y3Below = (yNA - T1) / 2;
        var Ag3Below = y3Below * 2 * T2;

        return Ag1 * (yNA - y1) * 2 + Ag3Above * y3Above + Ag3Below * y3Below;
    }
    private double CalculateZplMinor()
    {
        if (T2 > Agr / (2 * D))
        {
            //Plastic neutral axis lands inside the web
            var xPNA = Agr / (2 * D);
            return 2 * (T1 * Math.Pow(B1 - xPNA, 2)) / 2 + Hw * Math.Pow(T2 - xPNA, 2) / 2 + D * Math.Pow(xPNA, 2) / 2;
        }
        else if (T2 <= Agr / (2 * D))
        {
            //Plastic neutral axis lands outside the web
            var xPNA = B1 - Agr / (4 * T1);
            return 2 * (T1 * Math.Pow(B1 - xPNA, 2)) / 2 + D * Math.Pow(xPNA, 2) / 2 - Hw * Math.Pow(T2 - xPNA, 2) / 2;
        }
        return 0;
    }
    private double CalculateCw()
    {
        var bw = B1 - T2 / 2;

        // TODO: This Cw value does not sccurately match the red book for taper flange channels
        return (bw - 3 * (Math.Pow(bw, 2) * T1 / (2 * bw * T1 + (D - T1) * T2 / 3))) * Math.Pow(D - T1, 2) * Math.Pow(bw, 2) * T1 / 6 + Math.Pow(Math.Pow(bw, 2) * T1 / (2 * bw * T1 + (D - T1) * T2 / 3), 2) * IMajor;
    }
    public double Ag1 => B1 * T1;
    public double Ag3 => Hw * T2;

    /// <summary>
    /// Elastic neutral axis, from top of flange.
    /// </summary>
    private double XeNA { get; set; }
    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = false;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = true;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{B1.Cut(1)} {TypeDisplay}";
    protected override string GetTypeDisplay() => "PFC";
}
