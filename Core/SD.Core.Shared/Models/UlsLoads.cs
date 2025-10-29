namespace SD.Core.Shared.Models;
public class UlsLoads
{
    /// <summary>
    /// Ultimate applied compresive force on the beam.
    /// </summary>
    public double Cu { get; set; }

    /// <summary>
    /// Ultimate applied tension force on the beam.
    /// </summary>
    public double Tu { get; set; }

    /// <summary>
    /// Ultimate applied bending moment about the major (2) axis of the beam.
    /// </summary>
    public double MuMajor { get; set; }

    /// <summary>
    /// Ultimate applied bending moment about the minor (1) axis of the beam.
    /// </summary>
    public double MuMinor { get; set; }

    /// <summary>
    /// Ultimate applied shear force on the beam, in a direction parallel to the major (2) axis of the beam.
    /// </summary>
    public double VuMajor { get; set; }

    /// <summary>
    /// Ultimate applied shear force on the beam, in a direction parallel to the minor (1) axis of the beam.
    /// </summary>
    public double VuMinor { get; set; }

    /// <summary>
    /// Ultimate Von Misses applied stress found in the XY plane of the beam, along the entire length of the beam.
    /// </summary>
    public double VonMisses { get; set; }
}
