using SD.Element.Design.AS.Enums;

namespace SD.Element.Design.AS.Services
{
    public class ASMomentService
    {
        private const double phi = 0.9D;

        /// <summary> 
        /// Calculaties the Element Slenderness Limits (CHS only for compression calcs)
        /// </summary>  
        private static void getElementSlenderness(BeamAxisPart axis, Section section, out double[] lambda_e, out double[] lambda_ep, out double[] lambda_ey)
        {
            // Relevant Indices
            const int flange1 = 0;
            const int flange2 = 1;
            const int web = 2;

            lambda_e = [0,0,0];
            lambda_ep = [0,0,0];
            lambda_ey = [0,0,0];

            switch (section.SectionType)
            {
                case SectionType.IorH: //I and H sections - TODO Check different flange lengths
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B2 - section.T3) / 2D / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[flange2] = ((section.B1 - section.T3) / 2D / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[web] = ((section.D - section.T2 - section.T1) / section.T3) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        switch (axis)
                        {
                            case BeamAxisPart.MajorTop:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 16D;

                                    // Web 
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MajorBottom:
                                {
                                    // Flange 2
                                    lambda_ep[flange2] = 9D;
                                    lambda_ey[flange2] = 16D;

                                    // Web
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MinorTop:
                            case BeamAxisPart.MinorBottom:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 25D;

                                    // Flange 2
                                    lambda_ep[flange2] = 9D;
                                    lambda_ey[flange2] = 25D;

                                    break;
                                }
                        }

                        break;
                    }
                case SectionType.LipChannel: // Sections PFC and TFC
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B1 - section.T2) / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[flange2] = lambda_e[flange1];
                        lambda_e[web] = ((section.D - section.T1 * 2D) / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        switch (axis)
                        {
                            case BeamAxisPart.MajorTop:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 16D;

                                    // Web 
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MajorBottom:
                                {
                                    // Flange 2
                                    lambda_ep[flange2] = 9D;
                                    lambda_ey[flange2] = 16D;

                                    // Web
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MinorTop:
                                {
                                    // Web
                                    lambda_ep[web] = 30D;
                                    lambda_ey[web] = 45D;

                                    break;
                                }
                            case BeamAxisPart.MinorBottom:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 25D;

                                    // Flange 2
                                    lambda_ep[flange2] = 9D;
                                    lambda_ey[flange2] = 25D;

                                    break;
                                }
                        }

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
                        switch (axis)
                        {
                            case BeamAxisPart.MajorTop:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 30D;
                                    lambda_ey[flange1] = 40D;

                                    // Web 
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MajorBottom:
                                {
                                    // Flange 2
                                    lambda_ep[flange2] = 30D;
                                    lambda_ey[flange2] = 40D;

                                    // Web 
                                    lambda_ep[web] = 82D;
                                    lambda_ey[web] = 115D;

                                    break;
                                }
                            case BeamAxisPart.MinorTop:
                            case BeamAxisPart.MinorBottom:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 82D;
                                    lambda_ey[flange1] = 115D;

                                    // Flange 2
                                    lambda_ep[flange2] = 82D;
                                    lambda_ey[flange2] = 115D;

                                    // Web 
                                    lambda_ep[web] = 30D;
                                    lambda_ey[web] = 40D;

                                    break;
                                }
                        }

                        break;
                    }
                case SectionType.T: // Sections T
                    {
                        // Element slenderness
                        lambda_e[flange1] = ((section.B2 - section.T3) / 2D / section.T1) * Math.Sqrt(section.Material.MinFy / 250D);
                        lambda_e[web] = ((section.D - section.T1) / section.T2) * Math.Sqrt(section.Material.MinFy / 250D);

                        // Set Limits
                        switch (axis)
                        {
                            case BeamAxisPart.MajorTop:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 16D;

                                    break;
                                }
                            case BeamAxisPart.MajorBottom:
                                {
                                    // Web 
                                    lambda_ep[web] = 9D;
                                    lambda_ey[web] = 25D;

                                    break;
                                }
                            case BeamAxisPart.MinorTop:
                            case BeamAxisPart.MinorBottom:
                                {
                                    // Flange 1
                                    lambda_ep[flange1] = 9D;
                                    lambda_ey[flange1] = 25D;

                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Determines if a beam section is compact, non-compact, or slender
        /// </summary>
        public static Slenderness GetSectionSlenderness(Section section, BeamAxisPart axis, out bool specialCase, out double lambda_s, out double lambda_sp, out double lambda_sy)
        {

            // Total Variables
            lambda_s = 0D; // Section slenderness
            lambda_sp = 0D; // Plasticity limit
            lambda_sy = 0D; // Yield slenderness limit

            // Relevant Indices
            const int flange1 = 0;
            const int flange2 = 1;
            const int web = 2;

            // Flags
            specialCase = false;

            getElementSlenderness(axis, section, out double[] lambda_e, out double[] lambda_ep, out double[] lambda_ey);

            // === Slenderness Limits ===
            if (section.SectionType != SectionType.CircularHollow)
            {
                int[] relevantSection = [];

                // Select relevant sections
                switch (section.SectionType)
                {
                    case SectionType.IorH: //I and H sections
                        {
                            switch (axis)
                            {
                                case BeamAxisPart.MajorTop:
                                    {
                                        relevantSection = [flange1, web];
                                        break;
                                    }
                                case BeamAxisPart.MajorBottom:
                                    {
                                        relevantSection = [flange2, web];
                                        break;
                                    }
                                case BeamAxisPart.MinorTop:
                                case BeamAxisPart.MinorBottom:
                                    {
                                        relevantSection = [flange1, flange2];
                                        break;
                                    }

                            }

                            break;
                        }
                    case SectionType.LipChannel: // Sections PFC and TFC
                        {
                            switch (axis)
                            {
                                case BeamAxisPart.MajorTop:
                                    {
                                        relevantSection = [flange1, web];
                                        break;
                                    }
                                case BeamAxisPart.MajorBottom:
                                    {
                                        relevantSection = [flange2, web];
                                        break;
                                    }
                                case BeamAxisPart.MinorTop:
                                    {
                                        relevantSection = [web];
                                        break;
                                    }
                                case BeamAxisPart.MinorBottom:
                                    {
                                        relevantSection = [flange1, flange2];
                                        break;
                                    }

                            }

                            break;
                        }
                    case SectionType.Angle: // Sections EA and UA - TODO
                        {
                            break;
                        }
                    case SectionType.RectangularHollow: // Sections SHS and RHS
                        {
                            switch (axis)
                            {
                                case BeamAxisPart.MajorTop:
                                    {
                                        relevantSection = [flange1, web];
                                        break;
                                    }
                                case BeamAxisPart.MajorBottom:
                                    {
                                        relevantSection = [flange2, web];
                                        break;
                                    }
                                case BeamAxisPart.MinorTop:
                                case BeamAxisPart.MinorBottom:
                                    {
                                        relevantSection = [flange1, flange2, web];
                                        break;
                                    }
                            }

                            break;
                        }
                    case SectionType.T: // Sections T
                        {
                            switch (axis)
                            {
                                case BeamAxisPart.MajorTop:
                                case BeamAxisPart.MinorTop:
                                case BeamAxisPart.MinorBottom:
                                    {
                                        relevantSection = [flange1];
                                        break;
                                    }
                                case BeamAxisPart.MajorBottom:
                                    {
                                        relevantSection = [web];
                                        break;
                                    }
                            }

                            break;
                        }
                }

                int maxIndex = relevantSection[0];

                foreach (int currentIndex in relevantSection)
                {
                    if (maxIndex == currentIndex) continue;

                    // Ratios
                    var ratio = lambda_e[currentIndex] / lambda_ey[currentIndex];
                    var compareRatio = lambda_e[maxIndex] / lambda_ey[maxIndex];

                    if (ratio > compareRatio)
                    {
                        maxIndex = currentIndex;
                    }
                }

                lambda_s = lambda_e[maxIndex];
                lambda_sp = lambda_ep[maxIndex];
                lambda_sy = lambda_ey[maxIndex];

                // Set special case - TODO: Might include angles
                if ((section.SectionType == SectionType.IorH && (axis == BeamAxisPart.MinorTop || axis == BeamAxisPart.MinorBottom)) ||
                    (section.SectionType == SectionType.T && axis != BeamAxisPart.MajorTop) ||
                    (section.SectionType == SectionType.LipChannel && axis == BeamAxisPart.MinorBottom))
                {
                    specialCase = true;
                }
            }
            else
            {
                lambda_s = (section.B1 / section.T1) * (section.Material.FyElement1 / 250D);
                lambda_sp = 50D;
                lambda_sy = 120D;
            }

            // Compact
            if (lambda_s <= lambda_sp)
            {
                return Slenderness.Compact;
            }
            else if (lambda_s > lambda_sp && lambda_s <= lambda_sy)
            {
                return Slenderness.NonCompact;
            }
            else
            {
                return Slenderness.Slender;
            }
        }

        /// <summary> 
        /// Determines if a beam section is compact, non-compact, or slender without outputs
        /// </summary>
        public static Slenderness GetSectionSlenderness(Section bp, BeamAxisPart axis)
        {
            return GetSectionSlenderness(bp, axis, out bool specialCase, out double lambda_s, out double lambda_sp, out double lambda_sy);
        }

        /// <summary>
        /// Determines the effective section modulus to section 5.2.1 of AS 4100-2020
        /// </summary>
        public static double GetSectionModulus(Section section, BeamAxisPart axis)
        {
            var Ze = 0D; // Effective section modulus
            var slenderness = GetSectionSlenderness(section, axis, out bool specialCase, out double lambda_s, out double lambda_sp, out double lambda_sy);

            // === Final Calculations ===

            // Plastic Modulus
            var S = axis == BeamAxisPart.MajorTop || axis == BeamAxisPart.MajorBottom ? section.ZplMajor : section.ZplMinor;

            // Elastic Modulus
            //var Z = axis == BeamAxis.MajorTop ? section.ZeMajorTop : axis == BeamAxis.MajorBottom ? section.ZeMajorBottom : axis == BeamAxis.MinorTop ? section.ZeMinorTop : section.ZeMinorBottom;
            var Z = axis == BeamAxisPart.MajorTop || axis == BeamAxisPart.MajorBottom ? section.ZeMajor: section.ZeMinor; // TODO: Get Z values from strand? Or account for rounded corners

            // Compact Sections
            if (slenderness == Slenderness.Compact)
            {
                Ze = Math.Min(S, 1.5D * Z);
            }

            // Non-Compact Sections
            else if (slenderness == Slenderness.NonCompact)
            {
                var Zc = Math.Min(S, 1.5D * Z); // Compact effective section modulus

                Ze = Z + (((lambda_sy - lambda_s) / (lambda_sy - lambda_sp)) * (Zc - Z));
            }

            // Slender Sections
            else
            {
                if (section.SectionType == SectionType.CircularHollow)
                {
                    Ze = Z * Math.Min(Math.Sqrt(lambda_sy / lambda_s), Math.Pow((2D * lambda_sy) / lambda_s, 2D));
                }
                else if (specialCase)
                {
                    Ze = Z * Math.Pow((lambda_sy / lambda_s), 2);
                }
                else
                {
                    Ze = Z * (lambda_sy / lambda_s); // TODO - Ignoring flat plate element AS 4100-2020 section 5.2.5
                }
            }

            return Ze;
        }

        /// <summary> 
        /// Determines if the segment has full lateral restraints
        /// </summary>
        public static bool HasFullLateralRestraint(Beam beam, BeamAxisPart axis, double Ze)
        {
            var section = beam.Section;

            // Variables
            var beta_m = -1D; // TODO -1 for now Section 5.3.2.4
            var lengthRadiusRatio = beam.BeamChain.L2 / section.RMinor;

            switch (section.SectionType)
            {
                case SectionType.IorH:
                    {
                        // Equal flanged
                        if (section.B1 == section.B2)
                        {
                            return lengthRadiusRatio <= (80D + 50D * beta_m) * Math.Sqrt(250D / section.Material.MinFy);
                        }
                        // Unequal flanged
                        else
                        {
                            var df = section.D - section.T1 / 2 - section.T2 / 2;
                            var Icy = axis == BeamAxisPart.MajorTop ? (section.T2 - Math.Pow(section.B2, 3)) / 12D : (section.T1 - Math.Pow(section.B1, 3)) / 12D;
                            var rho = Icy / section.IMinor;
                            return lengthRadiusRatio <= (80D + 50D * beta_m) * Math.Sqrt((2D * rho * section.Agr * df) / (2.5D * Ze)) * Math.Sqrt(250D / section.Material.MinFy);
                        }
                    }
                case SectionType.LipChannel:
                    {
                        return lengthRadiusRatio <= (60D + 40D * beta_m) * Math.Sqrt(250D / section.Material.MinFy);
                    }
                case SectionType.RectangularHollow:
                    {
                        var width = axis == BeamAxisPart.MajorTop ? section.B2 : section.B1;
                        return lengthRadiusRatio <= (1800D + 1500D * beta_m) * (width / section.D) * (250D / section.Material.MinFy);
                    }
                case SectionType.Angle:
                    {
                        return true; // TODO
                    }
            }

            return false;
        }

        /// <summary> 
        /// Calculates the member capacity of segments without full lateral restratin as in AS 4100-2020 Section 5.6
        /// </summary>
        private static double getMemberCapacity(Beam beam, BendingConstants bendingConstants, BeamAxisPart axis, double Mm, double Ms)
        {
            // Beam Section
            var section = beam.Section;

            var M2 = (int)axis < (int)BeamAxisPart.MinorTop ? bendingConstants.MuMajorQuarter : bendingConstants.MuMinorQuarter;
            var M3 = (int)axis < (int)BeamAxisPart.MinorTop ? bendingConstants.MuMajorHalf : bendingConstants.MuMinorHalf;
            var M4 = (int)axis < (int)BeamAxisPart.MinorTop ? bendingConstants.MuMajorThreeQuarter : bendingConstants.MuMinorThreeQuarter;

            // Variables
            var alpha_s = 0D;
            var alpha_m = Math.Min((1.7D * Mm) / Math.Sqrt(Math.Pow(M2, 2) + Math.Pow(M3, 2) + Math.Pow(M4, 2)), 2.5D); // Moment Modification Factor

            var kTop = beam.Kt_Bending * beam.Kl * beam.Kr; // TODO: Check for top and bottom
            var kBottom = beam.Kt_Bending * beam.Kl * beam.Kr;

            // Effective Length
            var le = axis == BeamAxisPart.MajorTop ? beam.BeamChain.L2 * kTop : beam.BeamChain.L1 * kBottom; // TODO: Double check k values and formula 5.6.3

            // Segment unrestrained at one end
            if (axis == BeamAxisPart.MajorTop ? beam.BeamChain.L2Ends.Any(a => a.Unrestrained) : beam.BeamChain.L1Ends.Any(a => a.Unrestrained))
            {
                // Check if quarter and max moments (M2, M3, M4, Mm) are within 0.5
                var range = 0.5;
                if (Math.Abs(M2 - M3) <= range &&
                    Math.Abs(M2 - M4) <= range &&
                    Math.Abs(M2 - Mm) <= range &&
                    Math.Abs(M3 - M4) <= range &&
                    Math.Abs(M3 - Mm) <= range &&
                    Math.Abs(M4 - Mm) <= range)
                {
                    alpha_m = 0.25D;
                }

                // Check if slope is linear
                else if (M3 - M2 == M4 - M3)
                {
                    alpha_m = 1.25D;
                }

                // Assume parabolic
                else
                {
                    alpha_m = 2.25D;
                }
            }

            // Segment fully or partially restrained at both ends
            var Moa = 0D;

            // Unequal Flanged I section
            if (section.SectionType == SectionType.IorH && section.B1 != section.B2)
            {
                var df = section.D - section.T1 / 2 - section.T2 / 2; // Distance between flange centroids
                var Icy = axis == BeamAxisPart.MajorTop ? (section.T2 - Math.Pow(section.B2, 3)) / 12D : (section.T1 - Math.Pow(section.B1, 3)) / 12D; // Second moment of area of the compression flange about the section minor axis
                var Beta_x = 0.8D * df * (((2 * Icy) / (section.IMinor)) - 1D); // Monosymmetry section constant

                Moa = Math.Sqrt((Math.Pow(Math.PI, 2) * section.Material.Es * section.IMinor) / Math.Pow(le, 2)) * (Math.Sqrt((section.Material.Gs * section.J) + ((Math.Pow(Math.PI, 2) * section.Material.Es * section.Cw) / Math.Pow(le, 2)) + (((Math.Pow(Beta_x, 2)) / 4D) * ((section.Material.Es * section.IMinor) / (Math.Pow(le, 2)))))); // Reference Buckling Moment
            }

            // The Rest - TODO: Assuming Cw is 0 for Angle and Hollow sections
            else
            {
                Moa = Math.Sqrt(((Math.Pow(Math.PI, 2) * section.Material.Es * section.IMinor) / Math.Pow(le, 2)) * (section.Material.Gs * section.J + ((Math.Pow(Math.PI, 2) * section.Material.Es * section.Cw) / Math.Pow(le, 2)))); // Reference Buckling Moment - TODO: check Cw
            }

            alpha_s = 0.6D * (Math.Sqrt(Math.Pow(Ms / Moa, 2) + 3) - (Ms / Moa)); // Slenderness Reduction Factor
            return Math.Min(alpha_m * alpha_s * Ms, Ms); // Member Moment Capacity
        }

        /// <summary>
        /// Determines the bending resistance to section 5.1 of AS 4100-2020 // TODO: Check for platic method section 5.1
        /// </summary>
        public static double MomentResistance(Beam beam, BeamForces beamForces, BendingConstants bendingConstants, out double Msx, out double Msy)
        {
            var section = beam.Section;

            // === Section ===

            // MajorTop Top
            var ZeMajorTop = GetSectionModulus(section, BeamAxisPart.MajorTop); // Effective section modulus major 1
            var Msx_Top = section.Material.MinFy * ZeMajorTop;

            // MajorTop Bottom
            var ZeMajorBottom = GetSectionModulus(section, BeamAxisPart.MajorBottom); // Effective section modulus major 2
            var Msx_Bottom = section.Material.MinFy * ZeMajorBottom;


            // MinorTop Top
            var ZeMinorTop = GetSectionModulus(section, BeamAxisPart.MinorTop); // Effective section modulus minor 1
            var Msy_Top = section.Material.MinFy * ZeMinorTop;

            // MinorTop Bottom
            var ZeMinorBottom = GetSectionModulus(section, BeamAxisPart.MinorBottom); // Effective section modulus minor 2
            var Msy_Bottom = section.Material.MinFy * ZeMinorBottom;

            // Output variables
            Msx = phi * Math.Min(Msx_Bottom, Msx_Top);
            Msy = phi * Math.Min(Msy_Top, Msy_Bottom);


            // === Member ===

            var Mbx_Top = 0D;
            var Mbx_Bottom = 0D;

            // Check MajorTop

            // Member capacity with full support
            if (section.SectionType == SectionType.CircularHollow ||
                HasFullLateralRestraint(beam, BeamAxisPart.MajorTop, ZeMajorTop))
            {
                Mbx_Top = Msx_Top;
            }

            // Member capacity without full support
            else
            {
                Mbx_Top = getMemberCapacity(beam, bendingConstants, BeamAxisPart.MajorTop, beamForces.MaxAbsMuMajor, Msx_Top);
            }


            // Check MajorBottom

            // Member capacity with full support
            if (section.SectionType == SectionType.CircularHollow ||
                HasFullLateralRestraint(beam, BeamAxisPart.MajorBottom, ZeMajorBottom))
            {
                // MajorTop Bottom
                Mbx_Bottom = Msx_Bottom;
            }

            // Member capacity without full support
            else
            {
                // MajorTop Bottom
                Mbx_Bottom = getMemberCapacity(beam, bendingConstants, BeamAxisPart.MajorBottom, beamForces.MaxAbsMuMajor, Msx_Bottom);
            }

            // Output variables
            return phi * Math.Min(Mbx_Top, Mbx_Bottom);
        }
    }
}
