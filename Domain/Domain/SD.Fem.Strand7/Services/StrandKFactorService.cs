namespace SD.Fem.Strand7.Services;
public class StrandKFactorService : IKFactorService
{
    public async Task CalculateKFactors(int modelId, IFemModelParameters femModelParameters, BeamDesignSettings sansDesignSettings, BeamAxis beamAxisEnum)
    {
        var beamChains = femModelParameters?.Beams?.Select(b => b.BeamChain)?.Distinct()?.ToList();
        if (beamChains == null || beamChains.Count == 0)
            return;
        foreach (var beamChain in beamChains)
            BeamChainHelper.SetBeamChainEndDetails(modelId, beamChain);
    }

    //private static IEnumerable<Beam> GetInPlaneBeams(IEnumerable<Beam> beams, BeamChain beamChain, int axis)
    //{
    //    var nodes2 = beamChain.L2Ends.Where(end => end.Released2).Select(e => e.Node);
    //    if (nodes2 == null || !nodes2.Any()) 
    //        return beams; 


    //    var connectedBeams = beams.Where(b => nodes2.Contains(b.Node1) || nodes2.Contains(b.Node2));

    //}
    private static void GetFrameStiffness(IEnumerable<Beam> connectedBeams, BeamChain beamChain)
    {

    }
}
