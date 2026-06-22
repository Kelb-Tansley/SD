namespace SD.Data.Entities;

public class BeamKValueEntity : EntityBase
{
    public required Guid FemFileStableId { get; set; }
    public int BeamNumber { get; set; }
    public double K2 { get; set; }
    public double K1 { get; set; }
    public double Kz { get; set; }
    public double KeTop { get; set; }
    public double KeBottom { get; set; }
}
