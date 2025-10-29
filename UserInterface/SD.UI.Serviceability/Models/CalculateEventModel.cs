using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;

namespace SD.UI.Serviceability.Models;
public class CalculateEventModel
{
    public DeflectionAxis DeflectionAxis { get; set; }
    public List<DeflectionResult>? DeflectionResults { get; set; }
    public double MinDeflectionRatio { get; set; }
}
