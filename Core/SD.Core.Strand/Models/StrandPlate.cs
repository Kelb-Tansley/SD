namespace SD.Core.Strand.Models;

public class StrandPlate(StrandPlateProperty poperty, List<StrandNode> nodes)
{
    public StrandPlateProperty Poperty { get; private set; } = poperty;
    public List<StrandNode> Nodes { get; private set; } = nodes;
}