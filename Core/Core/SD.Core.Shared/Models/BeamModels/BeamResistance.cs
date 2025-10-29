
namespace SD.Core.Shared.Models.BeamModels;
public class BeamResistance
{
    /// <summary>
    /// Tensile resistance
    /// </summary>
    public double Tr { get; protected set; }
    /// <summary>
    /// Shear resistance parallel to the major axis
    /// </summary>
    public double VrMajor { get; protected set; }
    /// <summary>
    /// Shear resistance parallel to the minor axis
    /// </summary>
    public double VrMinor { get; protected set; }
    /// <summary>
    /// Bending slenderness about the major axis. KL/r
    /// </summary>
    public double SlendernessMajor { get; protected set; }
    /// <summary>
    /// Bending slenderness about the minor axis. KL/r
    /// </summary>
    public double SlendernessMinor { get; protected set; }
}
