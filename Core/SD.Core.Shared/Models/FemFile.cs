namespace SD.Core.Shared.Models;
public class FemFile
{
    public Guid? FileId { get; set; }
    public string FemModelFilePath { get; set; } = string.Empty;
    public bool ExactMatchFound { get; set; } = false;
}
