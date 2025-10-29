using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels.Sections;
public class AngleSection : Section
{
    public double Ixx { get; private set; }
    public double Iyy { get; private set; }
    public double Ixy { get; private set; }

    public double Alpha { get; private set; }

    public double Zxx { get; private set; }
    public double Zyy { get; private set; }

    public double U1 { get; private set; }
    public double U2 { get; private set; }
    public double V1 { get; private set; }
    public double V2 { get; private set; }

    public AngleSection(double b,
                        double d,
                        double t,
                        Material material,
                        double? agr = null,
                        double? ceMajor = null,
                        double? ceMinor = null,
                        double? iMajor = null,
                        double? iMinor = null,
                        double? j = null,
                        double? aMajor = null,
                        double? aMinor = null) : base(SectionType.Angle, material)
    {
        B1 = b;
        B2 = 0;
        D = d;
        T1 = t;
        T2 = 0;
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

    private void InitialiseSectionProperties(double? agr = null,
                                             double? ceMajor = null,
                                             double? ceMinor = null,
                                             double? iMajor = null,
                                             double? iMinor = null,
                                             double? j = null,
                                             double? aMajor = null,
                                             double? aMinor = null)
    {
        SetAgr(agr);
        SetCeMajor(ceMajor);
        SetCeMinor(ceMinor);
        SetAMajor(aMajor);
        SetAMinor(aMinor);
        SetIXX();
        SetIYY();
        SetIXY();
        SetAlpha();

        SetIMajor(iMajor);
        SetIMinor(iMinor);
        SetRMajor();
        SetRMinor();
        SetZeXX();
        SetZeYY();

        SetV1();
        SetV2();
        SetU2();
        SetU1();

        SetZeMajor();
        SetZeMinor();
        SetJ(j);
        SetCw();
    }

    private void SetAgr(double? value = null)
    {
        Agr = value != null ? (double)value : B1 * T1 + (D - T1) * T1;
    }
    private void SetCeMajor(double? value = null)
    {
        CeMajor = value != null ? (double)value : (T1 / 2) * (Math.Pow(D, 2) + B1 * T1 - Math.Pow(T1, 2)) / Agr;
    }
    private void SetCeMinor(double? value = null)
    {
        CeMinor = value != null ? (double)value : (T1 / 2) * (Math.Pow(B1, 2) + D * T1 - Math.Pow(T1, 2)) / Agr;
    }
    private void SetAMajor(double? value = null)
    {
        AMajor = value != null ? (double)value : CeMajor - T1 / 2;
    }
    private void SetAMinor(double? value = null)
    {
        AMinor = value != null ? (double)value : CeMinor - T1 / 2;
    }
    private void SetIXX(double? value = null)
    {
        Ixx = value != null ? (double)value : T1 / 3 * (B1 * Math.Pow(T1, 2) + Math.Pow(D, 3) - Math.Pow(T1, 3)) - Agr * Math.Pow(CeMajor, 2);
    }
    private void SetIYY(double? value = null)
    {
        Iyy = value != null ? (double)value : T1 / 3 * (D * Math.Pow(T1, 2) + Math.Pow(B1, 3) - Math.Pow(T1, 3)) - Agr * Math.Pow(CeMinor, 2);
    }
    private void SetIXY(double? value = null)
    {
        Ixy = value != null ? (double)value : Math.Pow(T1, 2) / 4 * (Math.Pow(B1, 2) + Math.Pow(D, 2) - Math.Pow(T1, 2)) - Agr * CeMinor * CeMajor;
    }
    private void SetAlpha(double? value = null)
    {
        Alpha = value != null ? (double)value : (Math.Atan(-2 * Ixy / (Ixx - Iyy)) / 2).RadiansToDegrees();
    }
    private void SetIMajor(double? value = null)
    {
        IMajor = value != null ? (double)value : (Ixx + Iyy) / 2 + Math.Sqrt(Math.Pow((Ixx - Iyy) / 2, 2) + Math.Pow(Ixy, 2));
    }
    private void SetIMinor(double? value = null)
    {
        IMinor = value != null ? (double)value : (Ixx + Iyy) / 2 - Math.Sqrt(Math.Pow((Ixx - Iyy) / 2, 2) + Math.Pow(Ixy, 2));
    }
    private void SetRMajor(double? value = null)
    {
        RMajor = value != null ? (double)value : Math.Sqrt(IMajor / Agr);
    }
    private void SetRMinor(double? value = null)
    {
        RMinor = value != null ? (double)value : Math.Sqrt(IMinor / Agr);
    }
    private void SetZeXX(double? value = null)
    {
        Zxx = value != null ? (double)value : Ixx / Math.Max(D - CeMajor, CeMajor);
    }
    private void SetZeYY(double? value = null)
    {
        Zyy = value != null ? (double)value : Iyy / Math.Max(B1 - CeMinor, CeMinor);
    }
    private void SetV1(double? value = null)
    {
        V1 = value != null ? (double)value : CalculateV1();
    }
    private void SetV2(double? value = null)
    {
        V2 = value != null ? (double)value : CalculateV2();
    }
    private void SetU1(double? value = null)
    {
        U1 = value != null ? (double)value : CalculateU1();
    }
    private void SetU2(double? value = null)
    {
        U2 = value != null ? (double)value : CalculateU2();
    }
    private void SetZeMajor(double? value = null)
    {
        ZeMajor = value != null ? (double)value : IMajor / Math.Max(U1, U2);
    }
    private void SetZeMinor(double? value = null)
    {
        ZeMinor = value != null ? (double)value : IMinor / Math.Max(V1, V2);
    }
    private void SetJ(double? value = null)
    {
        J = value != null ? (double)value : (D - T1 / 2 + B1 - T1 / 2) * Math.Pow(T1, 3) / 3;
    }
    private void SetCw(double? value = null)
    {
        Cw = value != null ? (double)value : Math.Pow(T1, 3) / 36 * (Math.Pow(D - T1 / 2, 3) + Math.Pow(B1 - T1 / 2, 3));
    }

    private double CalculateAMajor()
    {
        var i1 = Math.Pow(T1, 3) * (D - 0.5 * T1) / 12;
        var i2 = T1 * Math.Pow(B1 - 0.5 * T1, 3) / 12;
        return CeMajor - T1 / 2 - i1 / (i1 + i2) * (D - 0.5 * T1) / 2;
    }
    private double CalculateAMinor()
    {
        var i1 = T1 * Math.Pow(D - 0.5 * T1, 3) / 12;
        var i2 = Math.Pow(T1, 3) * (B1 - 0.5 * T1) / 12;
        return CeMinor - T1 / 2 - i2 / (i1 + i2) * (B1 - 0.5 * T1) / 2;
    }
    private double CalculateU2()
    {
        var axy = Math.Sqrt(Math.Pow(B1 - CeMinor, 2) + Math.Pow(CeMajor - T1, 2));
        var theta = Math.Acos((CeMajor - T1) / axy).RadiansToDegrees();
        var beta = 90 - theta;
        return axy * Math.Sin((beta + Alpha).DegreesToRadians()) + T1 * Math.Cos(Alpha.DegreesToRadians());
    }

    private double CalculateU1()
    {
        return B1 * Math.Sin(Alpha.DegreesToRadians()) + D * Math.Cos(Alpha.DegreesToRadians()) - U2;
    }

    private double CalculateV2()
    {
        var axy = Math.Sqrt(Math.Pow(B1 - CeMinor, 2) + Math.Pow(CeMajor - T1, 2));
        var theta = Math.Acos((CeMajor - T1) / axy).RadiansToDegrees();
        var beta = 90 - theta;
        var v1Bottom = axy * Math.Cos((beta + Alpha).DegreesToRadians());
        var v1Top = D * Math.Cos((90 - Alpha).DegreesToRadians()) - V1 + T1 * Math.Cos(Alpha.DegreesToRadians());
        return Math.Max(v1Bottom, v1Top);
    }

    private double CalculateV1()
    {
        var axy = Math.Sqrt(Math.Pow(CeMinor, 2) + Math.Pow(CeMajor, 2));
        var theta = Math.Asin(CeMinor / axy).RadiansToDegrees();
        var beta = 90 - theta - Alpha;
        return axy * Math.Cos(beta.DegreesToRadians());
    }

    protected override void SetDefaultRestraints()
    {
        IsBottomFlangeRestraint = false;
        IsTopFlangeRestraint = true;
        IsTorsionalRestraint = false;
    }

    protected override string GetDisplayName() => $"{D.Cut(1)}x{B1.Cut(1)}x{T1.Cut(1)} {TypeDisplay}";
    protected override string GetTypeDisplay() => D == B1 ? "EA" : "UA";
}
