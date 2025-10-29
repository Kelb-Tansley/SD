namespace SD.Data.Entities;
public class BeamPropertySettings : EntityBase
{
    public required string FileName { get; set; }
    public int PropertyNumber { get; set; }
    public bool IsLateralRestraint { get; set; }
}