namespace SD.Element.Design.AS.Services
{
    internal class ASTensionService
    {
        /// <summary>
        /// Determines the tensile resistance of a beam to section 7.1 of AS 4100-2020
        /// </summary>
        /// 
        public static double TensileResistance(Beam Beam)
        {
            var bp = Beam.Section;
            const double phi = 0.9D;
            return phi * Math.Min(bp.Agr * bp.Material.MinFy, 0.85D * Beam.Kt_Tension * bp.NetAreaFactor * bp.Agr * bp.Material.FuElement1);

            // Add min of all differnt Fy values per section
            // Second Agr should be An
        }
    }
}
