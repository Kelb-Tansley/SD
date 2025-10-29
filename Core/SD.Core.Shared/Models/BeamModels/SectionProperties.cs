namespace SD.Core.Shared.Models.BeamModels;
public class SectionProperties : Identity
{
    /// <summary>
    /// Breadth of section part 1. Also seen as the width (W) of the section. Most commonly the bottom-most flange of the cross-section.
    /// </summary>
    public double B1 { get; protected set; }
    /// <summary>
    /// Breadth of section part 2. Also seen as the width (W) of the section. Most commonly the top-most flange of the cross-section.
    /// </summary>
    public double B2 { get; protected set; }
    /// <summary>
    /// Overall depth or outer diameter of the section. Measured from top-most edge to bottom-most edge of the section. Also seen as the height (H) of the section.
    /// </summary>
    public double D { get; protected set; }
    /// <summary>
    /// Thickness of section part 1. Alligns with B1. The primary thickness for element with uniform thickness.
    /// </summary>
    public double T1 { get; protected set; }
    /// <summary>
    /// Thickness of section part 2. Alligns with B2 or D, depending on the section type. Also seen as the web thickness for PFC sections.
    /// </summary>
    public double T2 { get; protected set; }
    /// <summary>
    /// Thickness of section part 3. Alligns with D, depending on the section type. Also seen as the web thickness for I and Angles.
    /// </summary>
    public double T3 { get; protected set; }
    /// <summary>
    /// Gross sectional area.
    /// </summary>
    public double Agr { get; protected set; }
    /// <summary>
    /// Moment of inertia about the major axis (x).
    /// </summary>
    public double IMajor { get; protected set; }
    /// <summary>
    /// Moment of inertia about the minor axis (y).
    /// </summary>
    public double IMinor { get; protected set; }
    /// <summary>
    /// Torsional constant (St. Venant's constant)
    /// </summary>
    public double J { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the major axis (x)
    /// </summary>
    public double ZeMajor { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the major axis (x), for the top portion of the section
    /// </summary>
    public double ZeMajorTop { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the major axis (x), for the bottom portion of the section
    /// </summary>
    public double ZeMajorBottom { get; protected set; }
    /// <summary>
    /// Plastic section modulus about the major axis (x)
    /// </summary>
    public double ZplMajor { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the major axis (x)
    /// </summary>
    public double ZeMinor { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the minor axis (y), for the top portion of the section
    /// </summary>
    public double ZeMinorTop { get; protected set; }
    /// <summary>
    /// Elastic section modulus about the minor axis (y), for the bottom portion of the section
    /// </summary>
    public double ZeMinorBottom { get; protected set; }
    /// Plastic section modulus about the minor axis (y)
    /// </summary>
    public double ZplMinor { get; protected set; }
    /// <summary>
    /// Radius of gyration about the Major axis (x)
    /// </summary>
    public double RMajor { get; protected set; }
    /// <summary>
    /// Radius of gyration about the minor axis (y)
    /// </summary>
    public double RMinor { get; protected set; }
    /// <summary>
    /// Warping constant
    /// </summary>
    public double Cw { get; protected set; }
    /// <summary>
    /// The distance from the bottom-most position of the section to the elastic centroid axis, which is the major axis.
    /// For an angle section the major axis is about the u-u axis. However, the section centroid is read in the x-y plane. So CeMajor is from the bottom-most position of the section to the local x-x axis.
    /// T sections are always symmetric about the minor axis. Their major axis is the local y-y axis and so CeMajor is read from the left-most position of the section to the elastic centroid. 
    /// </summary>
    public double CeMajor { get; protected set; }
    /// <summary>
    /// The distance from the left-most position of the section to the elastic centroid axis, which is the minor axis.
    /// For an angle section the minor axis is about the v-v axis. However, the section centroid is read in the x-y plane. So CeMinor is from the left-most position of the section to the local y-y axis.
    /// T sections are always symmetric about the minor axis. Their minor axis is the local x-x axis and so CeMinor is read from the bottom-most position of the section to the elastic centroid. 
    /// </summary>
    public double CeMinor { get; protected set; }
    /// <summary>
    /// Distance from elastic centroid to shear center along major axis
    /// </summary>
    public double AMajor { get; protected set; }
    /// <summary>
    /// Distance from elastic centroid to shear center along minor axis
    /// </summary>
    public double AMinor { get; protected set; }
}
