using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class BeamChainService : IBeamChainService
{
    public void GenerateBeamChains(List<Beam> beams)
    {
        foreach (var beam in beams)
            GenerateBeamChain(beams, beam);
    }

    private static void GenerateBeamChain(List<Beam> beams, Beam beam)
    {
        var axes = new[] { BeamAxis.Principal1, BeamAxis.Principal2, BeamAxis.PrincipalZ, BeamAxis.PrincipalETop, BeamAxis.PrincipalEBottom };
        foreach (var axis in axes)
        {
            if (beam.BeamChain.GetChainBeamsByIdForAxis(axis).Count == 0)
            {
                var chainBeams = beam.BeamChain.GetChainBeamsForAxis(axis);
                var chainBeamsById = GetAllInChain(beam, axis, chainBeams);
                switch (axis)
                {
                    case BeamAxis.Principal1:
                        beam.BeamChain.L1BeamsById = chainBeamsById;
                        break;
                    case BeamAxis.Principal2:
                        beam.BeamChain.L2BeamsById = chainBeamsById;
                        break;
                    case BeamAxis.PrincipalZ:
                        beam.BeamChain.LzBeamsById = chainBeamsById;
                        break;
                    case BeamAxis.PrincipalETop:
                        beam.BeamChain.LeTopBeamsById = chainBeamsById;
                        break;
                    case BeamAxis.PrincipalEBottom:
                        beam.BeamChain.LeBottomBeamsById = chainBeamsById;
                        break;
                }
            }
            SetAllInChain(beam, axis, beams);
        }
    }

    private static List<int> GetAllInChain(Beam beam, BeamAxis beamAxis, List<Beam>? chainBeams)
    {
        if (chainBeams == null || chainBeams.Count <= 0)
            return [];

        var connectedBeams = new List<int> { beam.Number };
        foreach (var childBeam in chainBeams)
            GetChildChain(beam, beamAxis, childBeam, connectedBeams, chainBeams);

        return connectedBeams.Distinct().ToList();
    }

    private static void GetChildChain(Beam parentBeam, BeamAxis beamAxis, Beam childBeam, List<int> connectedBeams, List<Beam>? chainBeams)
    {
        if (chainBeams == null || chainBeams.Count <= 0)
            return;
        connectedBeams.Add(childBeam.Number);
        foreach (var subChildBeam in childBeam.BeamChain.GetChainBeamsForAxis(beamAxis))
        {
            if (subChildBeam.Number == parentBeam.Number || connectedBeams.Contains(subChildBeam.Number))
                continue;

            connectedBeams.Add(subChildBeam.Number);
            GetChildChain(parentBeam, beamAxis, subChildBeam, connectedBeams, chainBeams);
        }
    }

    private static void SetAllInChain(Beam beam, BeamAxis beamAxis, List<Beam> beams)
    {
        var chainBeamsById = beam.BeamChain.GetChainBeamsByIdForAxis(beamAxis);
        if (chainBeamsById == null || chainBeamsById.Count == 0)
        {
            // If there is no chain detected then set the chain to the beam itself
            beam.BeamChain.GetChainBeamsForAxis(beamAxis)?.Add(beam);
            chainBeamsById?.Add(beam.Number);
            return;
        }

        var effChainBeams = beams.Where(bm => chainBeamsById.Contains(bm.Number)).ToList();
        if (effChainBeams.Count == 0)
            return;

        // Set beam chain properties for all beams in the chain
        foreach (var chainBeam in effChainBeams)
        {
            chainBeam.BeamChain.SetBeamChainsForAxis(beamAxis, chainBeamsById, effChainBeams);
        }
    }
}
