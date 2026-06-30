namespace SD.Data.Entities;
public class SectionDesignSetting : EntityBase
{
    public required Guid FemFileStableId { get; set; }
    public int PropertyNumber { get; set; }
    public double WebStiffenerSpacing { get; set; }
    public double NetAreaFactor { get; set; }
    public bool IsLateralRestraint { get; set; }
    public bool IsTorsionalRestraint { get; set; }
    public bool IsTopFlangeRestraint { get; set; }
    public bool IsBottomFlangeRestraint { get; set; }
    public bool IsPlateGirder { get; set; }
    public bool IsBracedFrame { get; set; }
}