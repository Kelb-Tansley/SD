using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Models;

namespace SD.Tests.Sans.UnitTests;
public class UnitTestsBase
{
    public static Beam GetBeam(double l, Section section)
    {
        var beam = new Beam()
        {
            Section = section,
            BeamChain = new BeamChain()
            {
                L2 = l,
                L1 = l,
                Lz = l,
                LeTop = l
            }
        };
        beam.Resistance = new SansBeamResistance(beam);
        return beam;
    }
    public static Material GetMaterialProperties(double t1 = 0, double t2 = 0, double t3 = 0)
        => new(fyElement1: t1 > 16 ? 345D : 350D, t2 > 16 ? 345D : 350D, fyElement3: t3 > 16 ? 345D : 350D)
        {
            Es = 200000,
            Gs = 77000
        };
}
