namespace SD.Data.Entities;

public class DesignSettings : EntityBase
{
    public double BeamAllignmentAngleTolerance { get; set; }
    public double BeamRotationAngleTolerance { get; set; }
    public double BeamRestraintAngleTolerance { get; set; }
    public int BeamMinStations { get; set; }
}