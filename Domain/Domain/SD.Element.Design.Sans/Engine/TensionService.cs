namespace SD.Element.Design.Sans.Engine;
public class TensionService : SansService
{
    /// <summary>
    /// Determines the tensile resistance of a beam to section 13.2 of SANS 10162-1
    /// </summary>
    public static double TensileResistance(Section bp)
    {
        return Φ * Math.Min(bp.Agr * bp.Material.MinFy, 0.85D * bp.NetAreaFactor * bp.Agr * bp.Material.FuElement1);
    }
}
