namespace SD.Element.Design.Sans.Models;

/// <summary>
/// This should not be confused with the local co-ordinate system in Strand7. Unique to the SANS coordinate system.
/// </summary>
public class SansLocalCompressionResistance()
{
    public double Fex { get; set; }
    public double Fey { get; set; }
    public double FexK1 { get; set; }
    public double FeyK1 { get; set; }
    public double Crx { get; set; }
    public double Cry { get; set; }
    public double Crz { get; set; }
    public double Cryz { get; set; }
}