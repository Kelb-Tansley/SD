using SD.Element.Design.AS.Enums;

namespace SD.Element.Design.AS.Services
{
    internal class ASCompressionService
    {
        private const double phi = 0.9D;

        /// <summary>
        /// Calculates element slenderness for compression
        /// </summary>
        private static void GetElementSlenderness(Section section, out double[] lambda_e, out double[] lambda_ey)
        {
            // Relevant Indices
            const int flange1 = 0;
            const int flange2 = 1;
            const int web = 2;

            lambda_e = [0, 0, 0];
            lambda_ey = [0, 0, 0];

            switch (section.SectionType)
            {
                case SectionType.IorH: //I and H sections
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B2 - section.T3) / 2D / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[flange2] = ((section.B1 - section.T3) / 2D / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[web] = ((section.D - section.T2 - section.T1) / section.T3) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        lambda_ey[flange1] = 16D;
                        lambda_ey[flange2] = 16D;
                        lambda_ey[web] = 45D;

                        break;
                    }
                case SectionType.LipChannel: // Sections PFC and TFC
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B1 - section.T2) / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[flange2] = lambda_e[flange1];
                        lambda_e[web] = ((section.D - section.T1 * 2D) / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        lambda_ey[flange1] = 16D;
                        lambda_ey[flange2] = 16D;
                        lambda_ey[web] = 45D;

                        break;
                    }
                case SectionType.Angle: // Sections EA and UA - TODO
                    {
                        break;
                    }
                case SectionType.RectangularHollow: // Sections SHS and RHS
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B1 - section.T2 * 2D) / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[flange2] = lambda_e[flange1];
                        lambda_e[web] = ((section.D - section.T1 * 2D) / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        lambda_ey[flange1] = 40D;
                        lambda_ey[flange2] = 40D;
                        lambda_ey[web] = 40D;

                        break;
                    }
                case SectionType.CircularHollow: // Section CHS
                    {
                        // Element slenderness
                        lambda_e[flange1] = (section.D / section.T1) * (section.Material.MinFy / 250D);

                        // Set Limits
                        lambda_ey[flange1] = 82D;

                        break;
                    }
                case SectionType.T: // Sections T
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B2 - section.T3) / 2D / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[web] = ((section.D - section.T1) / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        lambda_ey[flange1] = 16D;
                        lambda_ey[web] = 16D;

                        break;
                    }
            }
        }

        /// <summary>
        /// Calculates form factor (kf) according to section 6.2.2
        /// </summary>
        public static double CalculateFormFactor(Section section)
        {
            // Relevant Indices
            const int flange1 = 0;
            const int flange2 = 1;
            const int web = 2;

            GetElementSlenderness(section, out double[] lambda_e, out double[] lambda_ey);

            // Effective width
            var beFlange1 = 0D;
            var beFlange2 = 0D;
            var beWeb = 0D;
            var webWidth = 0D;
            var Ae = 0D;
            switch (section.SectionType)
            {
                case SectionType.IorH:
                    beFlange1 = Math.Min(section.B1 * (lambda_ey[flange1] / lambda_e[flange1]), section.B1);
                    beFlange2 = Math.Min(section.B2 * (lambda_ey[flange2] / lambda_e[flange2]), section.B2);
                    webWidth = section.D - section.T2 - section.T1;
                    beWeb = Math.Min(webWidth * (lambda_ey[web] / lambda_e[web]), webWidth);

                    Ae = section.Agr;
                    if (beFlange1 < section.B1)
                    {
                        Ae -= (section.B1 - beFlange1) * section.T1;
                    }
                    if (beFlange2 < section.B2)
                    {
                        Ae -= (section.B2 - beFlange2) * section.T2;
                    }
                    if (beWeb < webWidth)
                    {
                        Ae -= (webWidth - beWeb) * section.T3;
                    }
                    break;
                case SectionType.LipChannel:
                    beFlange1 = Math.Min(section.B1 * (lambda_ey[flange1] / lambda_e[flange1]), section.B1);
                    beFlange2 = beFlange1; // TODO: Check use of flange 2
                    webWidth = section.D - section.T1 * 2D;
                    beWeb = Math.Min(webWidth * (lambda_ey[web] / lambda_e[web]), webWidth);

                    Ae = section.Agr;
                    if (beFlange1 < section.B1)
                    {
                        Ae -= (section.B1 - beFlange1) * section.T1 * 2D;
                    }
                    if (beWeb < webWidth)
                    {
                        Ae -= (webWidth - beWeb) * section.T2;
                    }
                    break;
                case SectionType.RectangularHollow:
                    beFlange1 = Math.Min(section.B1 * (lambda_ey[flange1] / lambda_e[flange1]), section.B1);
                    beFlange2 = beFlange1;
                    webWidth = section.D - section.T1 * 2D;
                    beWeb = Math.Min(webWidth * (lambda_ey[web] / lambda_e[web]), webWidth);

                    Ae = section.Agr;
                    if (beFlange1 < section.B1)
                    {
                        Ae -= (section.B1 - beFlange1) * section.T1 * 2D;
                    }
                    if (beWeb < webWidth)
                    {
                        Ae -= (webWidth - beWeb) * section.T2 * 2D;
                    }
                    break;
                case SectionType.Angle: // TODO:
                    break;
                case SectionType.CircularHollow:
                    beFlange1 = Math.Min(
                        section.D * Math.Sqrt(lambda_ey[flange1] / lambda_e[flange1]),
                        section.D * Math.Pow((3D * lambda_ey[flange1]) / lambda_e[flange1], 2)
                    );
                    beFlange1 = Math.Min(beFlange1, section.D);

                    Ae = section.Agr;
                    if (beFlange1 < section.D)
                    {
                        Ae = Math.PI * Math.Pow(beFlange1 / 2D, 2) - Math.PI * Math.Pow((section.D - section.T1) / 2D, 2);
                    }
                    break;
                case SectionType.T:
                    beFlange1 = Math.Min(section.B1 * (lambda_ey[flange1] / lambda_e[flange1]), section.B1);
                    webWidth = section.D - section.T1;
                    beWeb = Math.Min(webWidth * (lambda_ey[web] / lambda_e[web]), webWidth);

                    Ae = section.Agr;
                    if (beFlange1 < section.B1)
                    {
                        Ae -= (section.B1 - beFlange1) * section.T1;
                    }
                    if (beWeb < webWidth)
                    {
                        Ae -= (webWidth - beWeb) * section.T2;
                    }
                    break;
            }

            return Ae / section.Agr;
        }

        /// <summary>
        /// Determines the compressive properties
        /// </summary>
        private static double CalculateCompressiveProperties(Beam beam, Section bp, BeamAxisPart axis, double kf, bool forceKe)
        {
            var Ke = forceKe ? 1 : (int)axis <= (int)BeamAxisPart.MajorBottom ? beam.BeamChain.K2 : beam.BeamChain.K1;
            var effectiveRatio = (int)axis <= (int)BeamAxisPart.MajorBottom ? (Ke * beam.BeamChain.L2 / bp.RMajor) : (Ke * beam.BeamChain.L1 / bp.RMinor); // TODO: Fix comparison

            var lambda_n = effectiveRatio * Math.Sqrt(kf) * Math.Sqrt(bp.Material.FyElement1 / 250D); // Modified compression member slenderness - Check L_e value TODO
            var alpha_a = (2100D * (lambda_n - 13.5D)) / (Math.Pow(lambda_n, 2) - 15.3D * lambda_n + 2050D); // Compression member factor
            var alpha_b = 0D; // Compression member section constant - Should not be fixed AS Code Table 6.3.3 TODO: Assign

            switch (bp.SectionType)
            {
                case SectionType.IorH: //I and H sections
                    {
                        alpha_b = 0D;
                        break;
                    }
                case SectionType.LipChannel: // Sections PFC and TFC
                    {
                        alpha_b = 0.5D;
                        break;
                    }
                case SectionType.Angle: // Sections EA and UA
                    {
                        alpha_b = 0.5D;
                        break;
                    }
                case SectionType.CircularHollow:
                    {
                        alpha_b = -0.5D;
                        break;
                    }
                case SectionType.RectangularHollow: // Sections SHS and RHS
                    {
                        alpha_b = -0.5D;
                        break;
                    }
                case SectionType.T: // Sections T
                    {
                        alpha_b = 0.5D;
                        break;
                    }
                case SectionType.Unknown:
                    {
                        alpha_b = 0.5D;
                        break;
                    }
            }

            var lambda = lambda_n + alpha_a * alpha_b; // Slenderness Ratio
            var eta = Math.Max(0.00326D * (lambda - 13.5D), 0); // Compression member imperfection factor
            var xi = (Math.Pow(lambda / 90, 2) + 1 + eta) / (2 * Math.Pow(lambda / 90, 2)); // Compression member factor
            var alpha_c = xi * (1 - Math.Sqrt(1 - Math.Pow(90 / (xi * lambda), 2))); // Member slenderness reduction factor
            return alpha_c;
        }

        /// <summary>
        /// Determines the compressive resistance to section 6.1 of AS 4100-2020
        /// </summary>
        public static double CompressiveResistance(Beam beam, double kf, out double Ns, out double Ncx, out double Ncy, bool forceKe = false)
        {
            var bp = beam.Section;
            Ns = phi * kf * bp.Agr * bp.Material.MinFy; // Nominal Section Capacity - TODO check An (Net Area) value

            var alpha_c_Major = CalculateCompressiveProperties(beam, bp, BeamAxisPart.MajorTop, kf, forceKe);
            var alpha_c_Minor = CalculateCompressiveProperties(beam, bp, BeamAxisPart.MinorTop, kf, forceKe);

            Ncx = Math.Min(alpha_c_Major * Ns, Ns); // Nominal member capacity in major axis
            Ncy = Math.Min(alpha_c_Minor * Ns, Ns); // Nominal member capacity in minor axis

            return Math.Min(Ncx, Ncy);
        }
    }
}
