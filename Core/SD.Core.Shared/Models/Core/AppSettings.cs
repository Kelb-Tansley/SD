using System.IO;
using SD.Core.Shared.Contracts;

namespace SD.Core.Shared.Models.Core;
public class AppSettings : IAppSettings
{
    public void Initialize()
    {
        Directory.CreateDirectory(AuthLocation);
        Directory.CreateDirectory(ContourLocation);
        Directory.CreateDirectory(MathcadLocation);
    }

    public string? AppDataLocation { get; set; }
    public Strand7Api? Strand7Api { get; set; }

    public string AuthLocation => Environment.ExpandEnvironmentVariables(AppDataLocation) + "Auth";
    public string ContourLocation => Environment.ExpandEnvironmentVariables(AppDataLocation) + "Contour";
    public string MathcadLocation => Environment.ExpandEnvironmentVariables(AppDataLocation) + "Mathcad";
}