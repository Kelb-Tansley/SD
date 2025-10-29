using SD.Core.Shared.Contracts;

namespace SD.Core.Shared.Models.Core;
public class RuntimeAppSettings : IRuntimeAppSettings
{
    public bool RequiresRestart { get; set; }
}
