using SD.Core.Shared.Enum;

namespace SD.Core.Strand.Models;
public class BeamReleasedEnds
{
    public required int ReleasedEnd { get; set; }
    public required BeamAxis ReleasedAxis { get; set; }
}
