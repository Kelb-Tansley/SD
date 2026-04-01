namespace SD.Data.Entities;
public class BeamPropertySettings : EntityBase
{
    public required FemFileEntity FemFile { get; set; }
    public int PropertyNumber { get; set; }
    public bool IsLateralRestraint { get; set; }
}