using SD.Core.Shared.Models;

namespace SD.Core.Shared.Contracts;
public interface IAppSettings
{
    void Initialize();
    public string? AppDataLocation { get; set; }
    public Strand7Api? Strand7Api { get; set; }
    public string AuthLocation { get; }
    public string ContourLocation { get; }
    public string MathcadLocation { get; }
}