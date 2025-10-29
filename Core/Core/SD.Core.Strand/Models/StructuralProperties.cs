namespace SD.Core.Strand.Models;
public class StructuralProperties
{
    public double B { get; set; } // Width
    public double B2 { get; set; } // Width 2
    public double H { get; set; } // Height
    public double Tf { get; set; } // Flange thickness
    public double Tf2 { get; set; } // Flange thickness 2
    public double Tw { get; set; } // Web thickness
    public double L { get; set; } // Lip length
    public double Tl { get; set; } // Lip thickness
    public double Agr { get; set; } // Gross area
    public double Ix { get; set; } // Moment of inertia about x-axis
    public double Iy { get; set; } // Moment of inertia about y-axis
    public double Jsec { get; set; } // Torsional constant (St. Venant's constant)
    public double Zx { get; set; } // Plastic section modulus about x-axis
    public double Zplx { get; set; } // Plastic section modulus (local) about x-axis
    public double Zy { get; set; } // Plastic section modulus about y-axis
    public double Zply { get; set; } // Plastic section modulus (local) about y-axis
    public double Rx { get; set; } // Radius of gyration about x-axis
    public double Ry { get; set; } // Radius of gyration about y-axis
    public double Cw { get; set; } // Warping constant
    public double Ac { get; set; } // Centroidal distance from bottom flange to centroid
    public double Ax { get; set; } // Distance from centroid to shear center along x-axis
    public double Ay { get; set; } // Distance from centroid to shear center along y-axis
    public double Alpha { get; set; } // Shear coefficient
    public double Ru { get; set; } // Ultimate axial load resistance
    public double Rv { get; set; } // Ultimate shear load resistance
    public double Cx { get; set; } // Shear centre offset in the principal 1 axis direction
    public double Cy { get; set; } // Shear centre offset in the principal 2 axis direction
    public double df { get; set; } // Lip depth
}
