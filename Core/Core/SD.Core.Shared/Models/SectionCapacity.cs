namespace SD.Core.Shared.Models;

public class SectionCapacity
{
    /// <summary>
    /// Overall compressive resistance of the beam, including buckling and second order effects.
    /// </summary>
    public double Cr { get; set; }
    /// <summary>
    /// Compressive resistance of the beam about the major axis, including buckling and second order effects.
    /// </summary>
    public double CrMajor { get; set; }
    /// <summary>
    /// Compressive resistance of the beam about the minor axis, including buckling and second order effects.
    /// </summary>
    public double CrMinor { get; set; }

    /// <summary>
    /// Tension resistance of the beam, including reductions due to holes and welds at connections.
    /// </summary>
    public double Tr { get; set; }

    /// <summary>
    /// Bending moment resistance of the beam in the major (2) axis, including lateral and lateral torsional buckling effects.
    /// </summary>
    public double MrMajor { get; set; }

    /// <summary>
    /// Bending moment resistance of the beam in the minor (1) axis, including lateral and lateral torsional buckling effects.
    /// </summary>
    public double MrMinor { get; set; }

    /// <summary>
    /// Shear resistance of the beam, where the shear load is applied parallel to the major (2) axis.
    /// </summary>
    public double VrMajor { get; set; }

    /// <summary>
    /// Shear resistance of the beam, where the shear load is applied parallel to the major (1) axis.
    /// </summary>
    public double VrMinor { get; set; }

    /// <summary>
    /// Allowable stress of the section before yielding occurs.
    /// </summary>
    public double AllowableStress { get; set; }
}
