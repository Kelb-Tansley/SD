namespace SD.Core.Shared.Models;
public class BendingConstants
{
    /// <summary>
    /// Critical major moment resistance constant: ω2 is the coefficient to account for increased moment resistance 
    /// of a laterally unsupported beam segment when subject to a moment gradient. 
    /// Defined in section 13.6(a) of the SANS code.
    /// </summary>
    public double McrMajorω { get; set; }

    /// <summary>
    /// Critical minor moment resistance constant: ω2 is the coefficient to account for increased moment resistance 
    /// of a laterally unsupported beam segment when subject to a moment gradient. 
    /// Defined in section 13.6(a) of the SANS code.
    /// </summary>
    public double McrMinorω { get; set; }

    /// <summary>
    /// Ultimate major axis bending moment at first quarter point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMajorQuarter { get; set; }
    /// <summary>
    /// Ultimate major axis bending moment at the halfway point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMajorHalf { get; set; }
    /// <summary>
    /// Ultimate major axis bending moment at the three quarter point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMajorThreeQuarter { get; set; }

    /// <summary>
    /// Ultimate minor axis bending moment at first quarter point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMinorQuarter { get; set; }
    /// <summary>
    /// Ultimate minor axis bending moment at the halfway point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMinorHalf { get; set; }
    /// <summary>
    /// Ultimate minor axis bending moment at the three quarter point along the beam length, for the determination of w2.
    /// </summary>
    public double MuMinorThreeQuarter { get; set; }

    /// <summary>
    /// Accounting for the second-order effects due to the deformation of a member between its ends. 
    /// ω1 factor for the major axis taken from 13.8.5 from SANS 10162:1
    /// </summary>
    public double ω1Major { get; set; }
    /// <summary>
    /// Accounting for the second-order effects due to the deformation of a member between its ends. 
    /// ω1 factor for the minor axis taken from 13.8.5 from SANS 10162:1
    /// </summary>
    public double ω1Minor { get; set; }


    /// <summary>
    ///1.The unbraced length of the member is subject to end moments only.
    ///2.The member has a bending moment at any point within its unbraced length that is larger than the end moments, or when there is no effective 
    ///lateral support for the compression flange at one of the ends of the unsupported length.
    /// </summary>
    public int Loadω2Case { get; set; }
    /// <summary>
    /// 1.Member not subject to transverse loads between supports.
    /// 2.Member subject to distributed loads or a series of point loads between supports.
    /// 3.Member subject to a concentrated load or moment between supports.
    /// </summary>
    public int Loadω1Case { get; set; }
    /// <summary>
    /// 1.Single curvature
    /// 2.Double curvature
    /// </summary>
    public int Curvature { get; set; }
}
