namespace SD.Element.Design.Sans.Models;

public class CompressionResistance(double crMajor, double crMinor, double crTF, double feMajor, double feMinor, double feMajorK1, double feMinorK1)
{
    /// <summary>
    /// Compressive resistance in the major axis of the beam.
    /// </summary>
    public double CrMajor { get; private set; } = crMajor;
    /// <summary>
    /// Compressive resistance in the minor axis of the beam.
    /// </summary>
    public double CrMinor { get; private set; } = crMinor;
    /// <summary>
    /// Torsional or torsional-flexural buckling compressive resistance.
    /// </summary>
    public double CrTF { get; private set; } = crTF;
    /// <summary>
    /// Overall compressive resistance of the beam.
    /// </summary>
    public double Cr { get => DoubleExtensions.GetMinimumNonZero(CrMajor, CrMinor, CrTF); }

    public double FeMajor { get; private set; } = feMajor;
    public double FeMinor { get; private set; } = feMinor;
    public double FeMajorK1 { get; private set; } = feMajorK1;
    public double FeMinorK1 { get; private set; } = feMinorK1;
}