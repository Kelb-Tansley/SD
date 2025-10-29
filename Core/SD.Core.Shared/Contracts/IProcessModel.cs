namespace SD.Core.Shared.Contracts;
public interface IProcessModel
{
    public bool IsPrimaryProcessRunning { get; set; }
    public bool IsDesignWindowOpen { get; set; }
    public bool IsFemModelLoaded { get; set; }
}
