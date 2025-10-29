namespace SD.Element.Design.AS.Services
{
    internal class ASCombinedService
    {
        private const double phi = 0.9D;

        /// <summary> 
        /// Returns uniaxial bending about the major principal x-axis (Mrx) as per section 8.3.2 in AS 4100-2020
        /// </summary>
        public static double MajorSectionBendingAxial(double Msx, double kf, bool alternativeCase, Beam beam, BeamForces beamForces, bool isTension, double Ns)
        {
            //var Mrx = phi;
            var section = beam.Section;

            // Calculate axial force and capacity
            var axialForce = isTension ? beamForces.MaxAxialForce : beamForces.MinAxialForce;
            var axialCapacity = isTension ? beam.Resistance!.Tr : Ns;

            // Calculate Mrx
            if (alternativeCase && (isTension || kf == 1D))
            {
                return phi * Math.Min(1.18D * Msx * (1D - axialForce / axialCapacity), Msx);
            }
            else if (alternativeCase && (kf < 1D))
            {
                var dw = section.D - section.T1 - section.T2; // Depth of the web
                var tw = section.SectionType == SectionType.IorH ? section.T3 : section.T2; // Thickness of the web
                var fyw = section.SectionType == SectionType.IorH ? section.Material.FyElement3 : section.Material.FyElement2; // Fy of the web

                var lambda_w = (dw / tw) * Math.Sqrt(fyw / 250D);
                var lambda_wy = section.SectionType == SectionType.IorH ? 45D : 40D; // Yield slenderness limit of the web //A.C.// TODO: Add 35 for HW sections.

                return phi * Math.Min(Msx * (1D - axialForce / axialCapacity) * (1D + 0.18D * ((82D - lambda_w) / (82D - lambda_wy))), Msx);
            }
            else
            {
                return phi * Msx * (1 - axialForce / axialCapacity);
            }
        }

        /// <summary> 
        /// Returns uniaxial bending about the minor principal y-axis (Mry) as per section 8.3.3 in AS 4100-2020
        /// </summary>
        public static double MinorSectionBendingAxial(double Msy, bool alternativeCase, Beam beam, BeamForces beamForces, bool isTension, Slenderness minorSlenderness, double Ns)
        {
            var section = beam.Section;

            // Calculate axial force and capacity
            var axialForce = 0D;
            if (isTension)
            {
                axialForce = beamForces.MaxAxialForce > 0 ? beamForces.MaxAxialForce : 0;
            }
            else
            {
                axialForce = Math.Abs(beamForces.MinAxialForce < 0 ? beamForces.MinAxialForce : 0);
            }
            var axialCapacity = isTension ? beam.Resistance!.Tr : Ns;

            // Doubly semmetric I or H compact section
            if (section.SectionType == SectionType.IorH &&
                section.B1 == section.B2 &&
                section.T1 == section.T2 &&
                minorSlenderness == Slenderness.Compact)
            {
                return phi * Math.Min(1.19D * Msy * (1D - Math.Pow(axialForce / axialCapacity, 2)), Msy);
            }

            // Rectangular or square compact section
            else if (section.SectionType == SectionType.RectangularHollow && minorSlenderness == Slenderness.Compact)
            {
                return phi * Math.Min(1.18D * Msy * (1D - axialForce / axialCapacity), Msy);
            }

            // Default
            else
            {
                return phi * Msy * (1D - axialForce / axialCapacity);
            }
        }
    }
}
