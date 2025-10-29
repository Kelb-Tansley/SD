namespace SD.Core.Shared.Contracts;
public interface IFemModel
{
    public nint ModelHandle { get; set; }
    public nint DesignModelHandle { get; set; }
    public string FileName { get; set; }
    public bool FileExists { get; set; }

    public void ClearFile();
}
