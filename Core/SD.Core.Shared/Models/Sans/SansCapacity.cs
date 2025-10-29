namespace SD.Core.Shared.Models.Sans;
public class SansCapacity : SectionCapacity
{
    public double ω1Major { get; set; }
    public double ω1Minor { get; set; }
    public double ω2Major { get; set; }
    public double ω2Minor { get; set; }
    public double SlendernessMajor { get; set; }
    public double SlendernessMinor { get; set; }
    public BendingConstants BendingConstants { get; set; }
}
