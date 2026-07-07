namespace SD.Services;

public interface IInstallKeyFileService
{
    public bool TryReadInstallKey(out string? installKey);
    public void SaveInstallKey(string installKey);
}
