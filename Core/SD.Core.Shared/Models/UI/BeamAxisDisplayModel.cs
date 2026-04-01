using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models.UI;

public class BeamAxisDisplayModel
{
    public required string DisplayName { get; set; }
    public required BeamAxis BeamAxis { get; set; }
    public required ResultType ResultType { get; set; }
    public SansUtilizationType SansUtilizationType { get; set; } = SansUtilizationType.All;
}