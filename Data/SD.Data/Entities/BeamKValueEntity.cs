using System;

namespace SD.Data.Entities;

public class BeamKValueEntity : EntityBase
{
    // FK to FemFile via stable GUID to survive file moves/renames
    public required Guid FemFileStableId { get; set; }
    public FemFileEntity? FemFile { get; set; }
    public int BeamNumber { get; set; }
    public double K2 { get; set; }
    public double K1 { get; set; }
    public double Kz { get; set; }
    public double KeTop { get; set; }
    public double KeBottom { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
