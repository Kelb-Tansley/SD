namespace SD.Fem.Strand7.Helpers;

public static class BeamChainHelper
{
    public static void SetBeamChainEndDetails(int modelId, BeamChain beamChain)
    {
        beamChain.L2Ends = GetFreeChainEnds(modelId, beamChain.L2Beams, beamChain.EndL2Nodes, 2);

        beamChain.L1Ends = GetFreeChainEnds(modelId, beamChain.L1Beams, beamChain.EndL1Nodes, 1);

        beamChain.LzEnds = GetFreeChainEnds(modelId, beamChain.LzBeams, beamChain.EndLzNodes, 0);

        beamChain.LeTopEnds = GetFreeChainEnds(modelId, beamChain.LeTopBeams, beamChain.EndLeTopNodes, 0);

        beamChain.LeBottomEnds = GetFreeChainEnds(modelId, beamChain.LeBottomBeams, beamChain.EndLeBottomNodes, 0);
    }

    private static List<BeamChainEnd> GetFreeChainEnds(int modelId, List<Beam> beams, List<int> freeNodes, int axis)
    {
        var ends = new List<BeamChainEnd>();
        var endReleasePrincipalAxis = axis == 2 ? 0 : axis == 1 ? 1 : 2;

        if (freeNodes == null || freeNodes.Count == 0)
            return ends;

        var endBeams = beams?.Where(b => b.HasCommonNode(freeNodes));
        if (endBeams == null)
            return ends;

        foreach (var beam in endBeams)
        {
            var endReleases = new int[3];
            var releaseStiffness = new double[3];
            var releasedEnds = new List<int>();

            //Node i is on end i of the beam
            var i = freeNodes.Contains(beam.Node1) ? 1 : 2;

            var beamChainEnd = new BeamChainEnd()
            {
                Beam = beam,
                Node = i == 1 ? beam.Node1 : beam.Node2
            };

            //Check if there is a release on end i
            var result = St7.St7GetBeamRRelease3(modelId, beam.Number, i, endReleases, releaseStiffness).HandleApiError();
            if (!result.IsValid)
            {
                ends.Add(beamChainEnd);
                continue;
            }

            beamChainEnd.Released2 = endReleases[0] == St7.brReleased || endReleases[0] == St7.brPartial;
            beamChainEnd.Released1 = endReleases[1] == St7.brReleased || endReleases[1] == St7.brPartial;
            beamChainEnd.ReleasedZ = endReleases[2] == St7.brReleased || endReleases[2] == St7.brPartial;

            ends.Add(beamChainEnd);
        }
        return ends;
    }
}
