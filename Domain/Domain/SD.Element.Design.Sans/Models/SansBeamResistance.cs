
namespace SD.Element.Design.Sans.Models;
public class SansBeamResistance : BeamResistance
{
    /// <summary>
    /// Returns the axial compression class of the section, which is independent of the loads. This is set once per beam.
    /// </summary>
    public SectionClassification CompressionClass { get; private set; }

    public SansBeamResistance(Beam beam)
    {
        SetTr(beam.Section);
        SetVr(beam.Section);
        CompressionClass = ClassificationService.ClassifyAxialCompression(beam.Section);
        SetSlenderness(beam);
    }

    private void SetTr(Section section) => Tr = TensionService.TensileResistance(section);
    private void SetVr(Section section)
    {
        var vr = ShearService.ShearResistance(section);
        VrMajor = vr.VrMajor;
        VrMinor = vr.VrMinor;
    }
    private void SetSlenderness(Beam beam)
    {
        SlendernessMajor = beam.BeamChain.K2 * beam.BeamChain.L2 / beam.Section.RMajor;
        SlendernessMinor = beam.BeamChain.K1 * beam.BeamChain.L1 / beam.Section.RMinor;
    }
}
