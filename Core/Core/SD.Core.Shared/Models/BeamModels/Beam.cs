using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.BeamModels;
public partial class Beam : ObservableObject
{
    public int Number { get; set; }

    public double BeamL2 { get; set; }
    public double BeamL1 { get; set; }
    public double BeamLz { get; set; }
    public double BeamLeTop { get; set; }
    public double BeamLeBottom { get; set; }

    public double Kt_Bending { get; set; } = 1D;
    public double Kt_Tension { get; set; } = 1D;
    public double Kl { get; set; } = 1.4D;
    public double Kr { get; set; } = 1D;
    public double Beta_m { get; set; } = -1D;

    public required Section Section { get; set; }
    public BeamResistance? Resistance { get; set; }

    public BeamChain BeamChain { get; set; } = new();

    [ObservableProperty]
    public bool isSelected = false;

    public int Node1 { get; set; }
    public int Node2 { get; set; }

    public bool CanDesign()
    {
        return Section.CanDesign;
    }

    public List<Beam>? GetConnectedBeams(IEnumerable<Beam> beams)
    {
        return beams.Where(bm => bm.Node1 == Node1 || bm.Node1 == Node2 || bm.Node2 == Node1 || bm.Node2 == Node2)?.ToList();
    }

    public void ResetToDefault()
    {
        BeamChain.L1Beams = [this];
        BeamChain.L1BeamsById = [Number];
        BeamChain.L2Beams = [this];
        BeamChain.L2BeamsById = [Number];
        BeamChain.LzBeams = [this];
        BeamChain.LzBeamsById = [Number];
        BeamChain.LeTopBeams = [this];
        BeamChain.LeTopBeamsById = [Number];
        BeamChain.LeBottomBeams = [this];
        BeamChain.LeBottomBeamsById = [Number];
    }
}