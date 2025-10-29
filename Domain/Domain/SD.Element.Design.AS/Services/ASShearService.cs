using SD.Element.Design.Models;

namespace SD.Element.Design.AS.Services
{
    internal class ASShearService
    {
        const double phi = 0.9D;

        /// <summary>
        /// Determines nominal shear capacity with a given depth and area according to section 5.11 of AS 4100-2020
        /// </summary>
        private static double GetNominalShearCapacity(double tw, double dp, double area, double fy)
        {
            var Vv = 0D;
            var slendernessRatio = dp / tw; // Web Depth to Thickness Ratio
            var Vw = 0.6D * fy * area; // Shear Yield Capacity
            var stressRatio = 82D / Math.Sqrt(fy / 250D);

            if (slendernessRatio <= stressRatio)
            {
                Vv = Vw;
            }
            else
            {
                // Assume unstiffened web TODO
                var alpha_v = Math.Pow(stressRatio / slendernessRatio, 2);
                Vv = Math.Min(Vw, alpha_v * Vw);
            }

            return Vv;
        }

        /// <summary>
        /// Determines the design shear resistance of a beam to section 5.11 of AS 4100-2020
        /// </summary>
        public static ShearResistance ShearResistance(Section bp)
        {
            // Variables
            var dp = 0D; // Web Depth
            var Af = 0D; // Gross Area of flange
            var Aw = 0D; // Area of the web
            var fyw = 0D; // Web Fy
            var fyf = 0D; // Flange Fy
            var twMajor = 0D; // Web Thickness
            var twMinor = 0D; // Web Thickness

            switch (bp.SectionType)
            {
                case SectionType.IorH: //I and H sections
                    {
                        dp = bp.D - bp.T1 - bp.T2;
                        Aw = bp.D * bp.T3;
                        Af = bp.B1 * bp.T1 + bp.B2 * bp.T2;
                        fyw = bp.Material.FyElement3;
                        fyf = Math.Min(bp.Material.FyElement1, bp.Material.FyElement2);
                        twMajor = bp.T3;
                        twMinor = bp.T1 + bp.T2;

                        break;
                    }
                case SectionType.LipChannel: // Sections PFC and TFC
                    {
                        dp = bp.D - 2D * bp.T1;
                        Aw = bp.T2 * bp.D;
                        Af = bp.T1 * bp.B1 * 2D;
                        fyw = bp.Material.FyElement2;
                        fyf = bp.Material.FyElement1;
                        twMajor = bp.T2;
                        twMinor = bp.T1 * 2D;

                        break;
                    }
                case SectionType.Angle: // Sections EA and UA - Check TODO - Store leg 1 and leg 2 instead major and minor
                    {
                        // TODO:

                        //return new ShearResistance(vrMajor: phi * GetNominalShearCapacity(twMajor, dp, Aw, fyw),
                        //                           vrMinor: phi * GetNominalShearCapacity(twMajor, dpMinor, Af, fyw));
                        break;
                    }
                case SectionType.CircularHollow:
                    {
                        var Vw = 0.36D * bp.Material.FyElement1 * bp.Agr; // Shear Yield Capacity

                        return new ShearResistance(vrMajor: phi * Vw, vrMinor: phi * Vw);
                    }
                case SectionType.RectangularHollow: // Sections SHS and RHS
                    {
                        // TODO: check for curved corners this is wrong
                        dp = bp.D - bp.T1 * 2D;
                        Aw = bp.D * bp.T2 * 2D;
                        Af = 2D * bp.T1 * bp.B1;
                        fyw = bp.Material.FyElement2;
                        fyf = bp.Material.FyElement1;
                        twMajor = bp.T2;
                        twMinor = bp.T1 * 2D;

                        break;
                    }
                case SectionType.T: // Sections T
                    {
                        dp = bp.D - bp.T1;
                        Aw = bp.D * bp.T2;
                        Af = bp.T1 * bp.B1;
                        fyw = bp.Material.FyElement2;
                        fyf = bp.Material.FyElement1;
                        twMajor = bp.T2;
                        twMinor = bp.T1;

                        break;
                    }
                case SectionType.Unknown:
                    {
                        return new ShearResistance(vrMajor: 0D, vrMinor: 0D);
                    }
            }

            // Major axis: Web Fy
            // Minor axis: Flange Fy (calculate for both flanges the sum)
            return new ShearResistance(vrMajor: phi * GetNominalShearCapacity(twMajor, dp, Aw, fyw),
                                       vrMinor: phi * GetNominalShearCapacity(twMinor, bp.B1, Af, fyf)); // TODO: Implement minor axis
        }
    }
}
