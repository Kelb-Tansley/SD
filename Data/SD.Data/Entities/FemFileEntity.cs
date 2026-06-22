namespace SD.Data.Entities;

public class FemFileEntity : EntityBase
{
    // Stable identifier for the file that survives moves/renames
    public Guid StableId { get; set; } = Guid.NewGuid();

    // Original file name or relative path for convenience (do not rely on this as identity)
    public required string FileName { get; set; }
    public required string FileNameOnly { get; set; }
}