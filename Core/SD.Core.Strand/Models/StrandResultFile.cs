namespace SD.Core.Strand.Models;
public class StrandResultFile : Strand7Rules
{
    public int INumPrimary { get; set; }
    public int INumSecondary { get; set; }
    public int KStart { get; set; }
    public int KEnd { get; set; }
    public int NumberOfLoadCases { get; set; }
}