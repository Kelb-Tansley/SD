using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Shared.Extensions;
public static class BeamExtensions
{
    public static bool HasCommonNode(this Beam beam, Beam connectedBeam)
    {
        return beam.Node1 == connectedBeam.Node1 || beam.Node1 == connectedBeam.Node2 || beam.Node2 == connectedBeam.Node1 || beam.Node2 == connectedBeam.Node2;
    }
    public static bool HasCommonNode(this Beam beam, List<int> nodes)
    {
        return beam.Node1 == nodes[0] || beam.Node1 == nodes[1] || beam.Node2 == nodes[0] || beam.Node2 == nodes[1];
    }
}