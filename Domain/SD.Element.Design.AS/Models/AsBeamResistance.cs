using SD.Element.Design.AS.Services;

namespace SD.Element.Design.AS.Models;
public class ASBeamResistance : BeamResistance
{
    public ASBeamResistance(Beam beam)
    {
        SetTr(beam);
        SetVr(beam.Section);
        SetSlenderness(beam);
    }

    private void SetTr(Beam beam) => Tr = ASTensionService.TensileResistance(beam);
    private void SetVr(Section section)
    {
        var vr = ASShearService.ShearResistance(section);
        VrMajor = vr.VrMajor;
        VrMinor = vr.VrMinor;
    }
    private void SetSlenderness(Beam beam)
    {
        SlendernessMajor = beam.BeamChain.K2 * beam.BeamChain.L2 / beam.Section.RMajor;
        SlendernessMinor = beam.BeamChain.K1 * beam.BeamChain.L1 / beam.Section.RMinor;
    }
}
