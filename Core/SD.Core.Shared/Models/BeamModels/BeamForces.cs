namespace SD.Core.Shared.Models.BeamModels;
public class BeamForces
{
    /// <summary>
    /// Maximum VonMisses stresses found in the XY plane of the beam, along the entire length of the beam.
    /// </summary>
    public double VonMises { get; set; }
    /// <summary>
    /// Maximum axial force found along the entire length of the beam.
    /// </summary>
    public double MaxAxialForce { get; set; }

    /// <summary>
    /// Minimum axial force found along the entire length of the beam.
    /// </summary>
    public double MinAxialForce { get; set; }

    /// <summary>
    /// Maximum ultimate shear force parallel to the minor (1) axis.
    /// </summary>
    public double MaxVuMinor { get; set; }

    /// <summary>
    /// Maximum ultimate shear force parallel to the major (2) axis.
    /// </summary>
    public double MaxVuMajor { get; set; }

    /// <summary>
    /// Minimum ultimate shear force parallel to the minor (1) axis.
    /// </summary>
    public double MinVuMinor { get; set; }

    /// <summary>
    /// Minimum ultimate shear force parallel to the major (2) axis.
    /// </summary>
    public double MinVuMajor { get; set; }

    /// <summary>
    /// Maximum ultimate bending moment about the major (2) axis.
    /// </summary>
    public double MaxMuMajor { get; set; }

    /// <summary>
    /// Maximum ultimate bending moment about the minor (1) axis.
    /// </summary>
    public double MaxMuMinor { get; set; }

    /// <summary>
    /// Minimum ultimate bending moment about the major (2) axis.
    /// </summary>
    public double MinMuMajor { get; set; }

    /// <summary>
    /// Minimum ultimate bending moment about the minor (1) axis.
    /// </summary>
    public double MinMuMinor { get; set; }

    /// <summary>
    /// Ultimate bending moment about the minor (1) axis at the start of the beam.
    /// </summary>
    public double StartMuMinor { get; set; }

    /// <summary>
    /// Ultimate bending moment about the minor (1) axis at the end of the beam.
    /// </summary>
    public double EndMuMinor { get; set; }

    /// <summary>
    /// Ultimate bending moment about the major (2) axis at the start of the beam.
    /// </summary>
    public double StartMuMajor { get; set; }

    /// <summary>
    /// Ultimate bending moment about the major (2) axis at the end of the beam.
    /// </summary>
    public double EndMuMajor { get; set; }

    public double MaxAbsVuMinor { get => GetMaxAbs(MaxVuMinor, MinVuMinor); }

    public double MaxAbsVuMajor { get => GetMaxAbs(MaxVuMajor, MinVuMajor); }

    public double MaxAbsMuMinor { get => GetMaxAbs(MaxMuMinor, MinMuMinor); }

    public double MaxAbsMuMajor { get => GetMaxAbs(MaxMuMajor, MinMuMajor); }

    public double Tension => MaxAxialForce > 0 ? MaxAxialForce : 0;

    public double Compression => MinAxialForce < 0 ? -MinAxialForce : 0;

    private static double GetMaxAbs(double min, double max)
    {
        return Math.Max(Math.Abs(min), Math.Abs(max));
    }

    public double SmallerStartOrEndMuMinor()
    {
        var min = Math.Min(Math.Abs(EndMuMinor), Math.Abs(StartMuMinor));

        return min == Math.Abs(StartMuMinor) ? StartMuMinor : EndMuMinor;
    }
    public double LargerStartOrEndMuMinor()
    {
        var max = Math.Max(Math.Abs(EndMuMinor), Math.Abs(StartMuMinor));

        return max == Math.Abs(StartMuMinor) ? StartMuMinor : EndMuMinor;
    }
    public double SmallerStartOrEndMuMajor()
    {
        var min = Math.Min(Math.Abs(EndMuMajor), Math.Abs(StartMuMajor));

        return min == Math.Abs(StartMuMajor) ? StartMuMajor : EndMuMajor;
    }
    public double LargerStartOrEndMuMajor()
    {
        var max = Math.Max(Math.Abs(EndMuMajor), Math.Abs(StartMuMajor));

        return max == Math.Abs(StartMuMajor) ? StartMuMajor : EndMuMajor;
    }
    public bool HasBending()
    {
        const double tolerance = 0.01D;
        return Math.Abs(MaxAbsMuMinor) > tolerance || Math.Abs(MaxAbsMuMajor) > tolerance;
    }
}
