namespace SD.Services;

public sealed class LicenseValidationResult
{
    public bool IsLicensed { get; init; }
    public string? InstallKey { get; init; }
}