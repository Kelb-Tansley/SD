namespace SD.Core.Shared.Models.BeamModels;

public class BeamChainEnd
{
    public int Node { get; set; }
    public required Beam Beam { get; set; }

    public bool Released2 { get; set; }
    public bool Released1 { get; set; }
    public bool ReleasedZ { get; set; }

    public bool Unrestrained { get; set; }
}
