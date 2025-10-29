using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Core.Shared.Models.BeamModels;
public partial class BeamChain : BeamChainLength
{
    public List<int> LeBottomBeamsById { get; set; } = [];
    public List<Beam> LeBottomBeams { get; set; } = [];
    public List<BeamChainEnd> LeBottomEnds { get; set; } = [];

    public List<int> LeTopBeamsById { get; set; } = [];
    public List<Beam> LeTopBeams { get; set; } = [];
    public List<BeamChainEnd> LeTopEnds { get; set; } = [];

    public List<int> LzBeamsById { get; set; } = [];
    public List<Beam> LzBeams { get; set; } = [];
    public List<BeamChainEnd> LzEnds { get; set; } = [];

    public List<int> L2BeamsById { get; set; } = [];
    public List<Beam> L2Beams { get; set; } = [];
    public List<BeamChainEnd> L2Ends { get; set; } = [];

    public List<int> L1BeamsById { get; set; } = [];
    public List<Beam> L1Beams { get; set; } = [];
    public List<BeamChainEnd> L1Ends { get; set; } = [];

    public string ChainName { get => GetChainName(); }
    public List<Beam> LongestChain { get => GetLongestChain(); }

    public List<Beam> ConnectedChaineBottom { get; set; } = [];
    public List<Beam> ConnectedChaineTop { get; set; } = [];
    public List<Beam> ConnectedChainz { get; set; } = [];
    public List<Beam> ConnectedChain2 { get; set; } = [];
    public List<Beam> ConnectedChain1 { get; set; } = [];

    public void SetConnectedChains()
    {
        ConnectedChaineTop = GetConnectedChain(LeTopEnds, LeTopBeams);
        ConnectedChaineBottom = GetConnectedChain(LeBottomEnds, LeBottomBeams);
        ConnectedChainz = GetConnectedChain(LzEnds, LzBeams);
        ConnectedChain2 = GetConnectedChain(L2Ends, L2Beams);
        ConnectedChain1 = GetConnectedChain(L1Ends, L1Beams);
    }

    public void ResetToPrimaryBeam(Beam primaryBeam)
    {
        L1BeamsById.Clear();
        L2BeamsById.Clear();
        LzBeamsById.Clear();
        LeTopBeamsById.Clear();
        LeBottomBeamsById.Clear();
        L1Beams = [primaryBeam];
        L2Beams = [primaryBeam];
        LzBeams = [primaryBeam];
        LeTopBeams = [primaryBeam];
        LeBottomBeams = [primaryBeam];
    }

    private static List<Beam> GetConnectedChain(List<BeamChainEnd> ends, List<Beam> beams)
    {
        if (ends == null || ends.Count <= 0 || beams == null || beams.Count <= 2)
            return beams;

        var orderedConnected = new List<Beam>();
        var allNodes = ends.Select(n => n.Node)?.ToList();
        if (allNodes == null || allNodes.Count <= 0)
            return orderedConnected;

        var startBeam = ends.First().Beam;
        var endBeam = ends.Last().Beam;
        if (startBeam == null || endBeam == null)
            return orderedConnected;

        orderedConnected.Add(startBeam);
        var remainingBeams = beams.Where(beam => beam.Number != startBeam.Number && beam.Number != endBeam.Number)?.ToList();
        if (remainingBeams == null || remainingBeams.Count <= 0)
        {
            orderedConnected.Add(endBeam);
            return orderedConnected;
        }

        var nextBeam = startBeam;
        while (nextBeam != null)
        {
            nextBeam = remainingBeams.FirstOrDefault(beam => beam.HasCommonNode(nextBeam));
            if (nextBeam != null)
            {
                orderedConnected.Add(nextBeam);
                remainingBeams.Remove(nextBeam);
            }
        }
        orderedConnected.Add(endBeam);
        return orderedConnected;
    }

    public void SetLengths()
    {
        LeBottom = LeBottomBeams.Sum(b => b.BeamLeBottom);
        LeTop = LeTopBeams.Sum(b => b.BeamLeTop);
        Lz = LzBeams.Sum(b => b.BeamLz);
        L2 = L2Beams.Sum(b => b.BeamL2);
        L1 = L1Beams.Sum(b => b.BeamL1);
    }

    private string GetChainName()
    {
        var longestChain = GetOrderedLongestChain();
        if (longestChain == null)
            return string.Empty;
        else
        {
            var chainNumbers = longestChain.Select(ch => ch.Number).ToList();
            if (chainNumbers != null && chainNumbers.Count > 4)
            {
                var shortChain = chainNumbers[..2].Concat(chainNumbers.Slice(chainNumbers.Count - 2, 2)).Select(num => num.ToString()).ToList();
                shortChain.Insert(2, "...");
                return string.Join("-", shortChain);
            }
            return string.Join("-", longestChain.Select(ch => ch.Number).ToList());
        }
    }

    private List<Beam> GetOrderedLongestChain()
    {
        var orderedChain = new List<Beam>();
        var longestChain = GetLongestChain();

        if (longestChain.Count < 2)
            return longestChain;

        if (longestChain.Count == 2)
            return [.. longestChain.OrderBy(lc => lc.Number)];

        var freeEndNodes = longestChain.Select(b => b.Node1).Concat(longestChain.Select(b => b.Node2))?.GroupBy(n => n)?.Where(g => g.Count() < 2)?.Select(g => g.First())?.ToList();
        if (freeEndNodes != null && freeEndNodes.Count == 2)
        {
            var endBeams = longestChain.Where(b => b.HasCommonNode(freeEndNodes))
                                       .OrderBy(b => b.Number)
                                       .ToList();
            if (endBeams != null && endBeams.Count == 2)
            {
                var hasCompleted = false;
                var nextBeam = endBeams[0];

                orderedChain.Add(nextBeam);
                while (!hasCompleted)
                {
                    var connected = longestChain.FirstOrDefault(lc => nextBeam != null && lc.Number != nextBeam.Number && nextBeam.HasCommonNode(lc) && !orderedChain.Contains(lc));
                    if (connected != null)
                        orderedChain.Add(connected);

                    nextBeam = connected;
                    hasCompleted = nextBeam == null || nextBeam.HasCommonNode(endBeams[1]);
                }

                orderedChain.Add(endBeams[1]);
            }
        }

        return orderedChain;
    }

    private List<Beam> GetLongestChain()
    {
        var longestChain = new List<Beam>();
        if (LeTopBeams == null || LeBottomBeams == null || L2Beams == null || L1Beams == null || LzBeams == null)
            return longestChain;

        var chains = new List<List<Beam>>() { L2Beams, L1Beams, LzBeams, LeTopBeams, LeBottomBeams };

        return chains.OrderByDescending(list => list.Count).First();
    }

    [RelayCommand]
    public void SetChainKValues()
    {
        L2Beams.ForEach(b => { b.BeamChain.K2 = K2; b.BeamChain.K1 = K1; b.BeamChain.Kz = Kz; b.BeamChain.KeTop = KeTop; b.BeamChain.KeBottom = KeBottom; });
        L1Beams.ForEach(b => { b.BeamChain.K2 = K2; b.BeamChain.K1 = K1; b.BeamChain.Kz = Kz; b.BeamChain.KeTop = KeTop; b.BeamChain.KeBottom = KeBottom; });
        LzBeams.ForEach(b => { b.BeamChain.K2 = K2; b.BeamChain.K1 = K1; b.BeamChain.Kz = Kz; b.BeamChain.KeTop = KeTop; b.BeamChain.KeBottom = KeBottom; });
        LeTopBeams.ForEach(b => { b.BeamChain.K2 = K2; b.BeamChain.K1 = K1; b.BeamChain.Kz = Kz; b.BeamChain.KeTop = KeTop; b.BeamChain.KeBottom = KeBottom; });
        LeBottomBeams.ForEach(b => { b.BeamChain.K2 = K2; b.BeamChain.K1 = K1; b.BeamChain.Kz = Kz; b.BeamChain.KeTop = KeTop; b.BeamChain.KeBottom = KeBottom; });
    }

    public List<int> EndL2Nodes { get => GetFreeNodes(L2Beams); }
    public List<int> EndL1Nodes { get => GetFreeNodes(L1Beams); }
    public List<int> EndLzNodes { get => GetFreeNodes(LzBeams); }
    public List<int> EndLeTopNodes { get => GetFreeNodes(LeTopBeams); }
    public List<int> EndLeBottomNodes { get => GetFreeNodes(LeBottomBeams); }

    private static List<int> GetFreeNodes(IEnumerable<Beam> beams)
    {
        var freeNodes = new List<int>();

        var nodes1 = beams.Select(b => b.Node1);
        var nodes2 = beams.Select(b => b.Node2);

        if (nodes1 == null || nodes2 == null)
            return freeNodes;

        var allNodes = nodes1.Concat(nodes2);

        foreach (var node in allNodes.Distinct())
        {
            if (allNodes.Count(n => n == node) == 1)
            {
                freeNodes.Add(node);
            }
        }

        return freeNodes;
    }

    public List<Beam> GetChainBeamsForAxis(BeamAxis beamAxis)
    {
        return beamAxis switch
        {
            BeamAxis.Principal1 => L1Beams,
            BeamAxis.Principal2 => L2Beams,
            BeamAxis.PrincipalZ => LzBeams,
            BeamAxis.PrincipalETop => LeTopBeams,
            BeamAxis.PrincipalEBottom => LeBottomBeams,
            _ => [],
        };
    }

    public List<int> GetChainBeamsByIdForAxis(BeamAxis beamAxis)
    {
        return beamAxis switch
        {
            BeamAxis.Principal1 => L1BeamsById,
            BeamAxis.Principal2 => L2BeamsById,
            BeamAxis.PrincipalZ => LzBeamsById,
            BeamAxis.PrincipalETop => LeTopBeamsById,
            BeamAxis.PrincipalEBottom => LeBottomBeamsById,
            _ => [],
        };
    }

    public void SetBeamChainsForAxis(BeamAxis beamAxis, List<int> beamsById, List<Beam> beams)
    {
        switch (beamAxis)
        {
            case BeamAxis.Principal1:
                L1Beams = beams;
                L1BeamsById = beamsById;
                break;
            case BeamAxis.Principal2:
                L2Beams = beams;
                L2BeamsById = beamsById;
                break;
            case BeamAxis.PrincipalZ:
                LzBeams = beams;
                LzBeamsById = beamsById;
                break;
            case BeamAxis.PrincipalETop:
                LeTopBeams = beams;
                LeTopBeamsById = beamsById;
                break;
            case BeamAxis.PrincipalEBottom:
                LeBottomBeams = beams;
                LeBottomBeamsById = beamsById;
                break;
            case BeamAxis.All:
                break;
            default:
                break;
        }
    }
}
