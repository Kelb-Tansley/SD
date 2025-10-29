using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Models;

namespace SD.Element.Design.Sans.Engine;
public class CombinedService : SansService
{
    public static double AllowableStress(Material material) => Φ * material.MinFy;

    /// <summary>
    /// Returns the factor of the combined tension and bending applied loads to resistance.
    /// </summary>
    public static double CombinedTensionAndBending(BeamForces forces, Beam beam, MomentResistance momentResistance, SectionClassification flexClass)
    {
        if (beam.Resistance == null)
            throw new ArgumentNullException(nameof(beam));

        var tensionAndBending = forces.MaxAxialForce / beam.Resistance.Tr
            + Math.Abs(forces.MaxAbsMuMajor / momentResistance.MrMajorSupported)
            + Math.Abs(forces.MaxAbsMuMinor / momentResistance.MrMinorSupported);

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
        {
            tensionAndBending = Math.Max(tensionAndBending,
                Math.Abs(forces.MaxAbsMuMajor / momentResistance.MrMajor)
                - forces.MaxAxialForce * beam.Section.ZplMajor / (momentResistance.MrMajor * beam.Section.Agr));

            tensionAndBending = Math.Max(tensionAndBending,
                Math.Abs(forces.MaxAbsMuMinor / momentResistance.MrMinor)
                - forces.MaxAxialForce * beam.Section.ZplMinor / (momentResistance.MrMinor * beam.Section.Agr));
        }
        else
        {
            tensionAndBending = Math.Max(tensionAndBending,
                Math.Abs(forces.MaxAbsMuMajor / momentResistance.MrMajor)
                - forces.MaxAxialForce * beam.Section.ZeMajor / (momentResistance.MrMajor * beam.Section.Agr));

            tensionAndBending = Math.Max(tensionAndBending,
                Math.Abs(forces.MaxAbsMuMinor / momentResistance.MrMinor)
                - forces.MaxAxialForce * beam.Section.ZeMinor / (momentResistance.MrMinor * beam.Section.Agr));
        }

        return tensionAndBending;
    }

    /// <summary>
    /// Determines the compression and bending resistance to section 13.8.2 and 13.8.3, parts a), b) and c) of SANS 10162:1
    /// </summary>
    public static CompressionAndBendingStrength CombinedCompressionAndBending(BeamForces forces, Beam beam, double w1Major, double w1Minor, CompressionResistance cr, MomentResistance mr, SectionClassification flexClass, bool isBracedFrame)
    {
        var strength = new CompressionAndBendingStrength();

        var isSubClass2IorH = beam.Section.SectionType == SectionType.IorH && (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2);
        var factor = isSubClass2IorH ? 0.85D : 1D;

        var cu = Math.Abs(forces.MinAxialForce);

        //Cross-sectional strength
        var cr0 = Φ * beam.Section.Agr * beam.Section.Material.MinFy;

        var u1major = CalculateU1(beam, cu, w1Major, beam.Section.IMajor, beam.BeamChain.L2, beam.BeamChain.K2);

        var u1Minor = CalculateU1(beam, cu, w1Minor, beam.Section.IMinor, beam.BeamChain.L1, beam.BeamChain.K1);

        var β = isSubClass2IorH ? 0.6D : 1D;
        strength.CrossSectional = Interaction(forces, cr0, u1major, mr.MrMajorSupported, u1Minor, mr.MrMinorSupported, β, factor);

        //Overall member strength
        var crK1 = CompressionService.CompressiveResistance(beam, GetAxialClass(beam), cu, true);
        cr0 = forces.MinMuMinor == 0 && forces.MaxMuMinor == 0 ? crK1.CrMajor : Math.Min(crK1.CrMajor, crK1.CrMinor);
        u1major = isBracedFrame ? u1major : 1D;
        u1Minor = isBracedFrame ? u1Minor : 1D;
        var λcy = Math.Sqrt(beam.Section.Material.MinFy / cr.FeMajor);
        β = isSubClass2IorH ? Math.Min(0.6D + 0.4D * λcy, 0.85D) : 1D;
        strength.OverallMember = Interaction(forces, cr0, u1major, mr.MrMajorSupported, u1Minor, mr.MrMinorSupported, β, factor);

        LateralTorsionalBucklingStrength(forces, beam, w1Major, cr, mr, isBracedFrame, strength, factor, cu, u1major, u1Minor, β);

        return strength;
    }

    private static void LateralTorsionalBucklingStrength(BeamForces forces, Beam beam, double w1Major, CompressionResistance cr, MomentResistance mr, bool isBracedFrame, CompressionAndBendingStrength strength, double factor, double cu, double u1major, double u1Minor, double β)
    {
        //lateral torsional buckling strength
        u1major = isBracedFrame ? Math.Max(u1major, 1D) : 1D;

        strength.LateralTorsionalBuckling = Interaction(forces, cr.Cr, u1major, mr.MrMajorTopUnsupported, u1Minor, mr.MrMinorSupported, β, factor);

        // For top and bottom flange stability
        if (isBracedFrame)
        {
            u1major = CalculateU1(beam, cu, w1Major, beam.Section.IMajor, beam.BeamChain.LeBottom, beam.BeamChain.KeBottom);
            u1major = Math.Max(u1major, 1D);
        }

        var lTBBottomFlange = Interaction(forces, cr.Cr, u1major, mr.MrMajorBottomUnsupported, u1Minor, mr.MrMinorSupported, β, factor);

        if (isBracedFrame)
        {
            u1major = CalculateU1(beam, cu, w1Major, beam.Section.IMajor, beam.BeamChain.LeTop, beam.BeamChain.KeTop);
            u1major = Math.Max(u1major, 1D);
        }

        var lTBTopFlange = Interaction(forces, cr.Cr, u1major, mr.MrMajorTopUnsupported, u1Minor, mr.MrMinorSupported, β, factor);

        strength.LateralTorsionalBuckling = Math.Max(strength.LateralTorsionalBuckling, Math.Max(lTBBottomFlange, lTBTopFlange));
    }

    private static double CalculateU1(Beam beam, double cu, double w1, double inertia, double length, double kFactor)
    {
        var ce = Math.Pow(Math.PI, 2) * beam.Section.Material.Es * inertia / Math.Pow(length * kFactor, 2);
        return w1 / (1 - Math.Abs(cu / ce));
    }

    private static double Interaction(BeamForces forces, double cr, double u1major, double mrMajor, double u1Minor, double mrMinor, double β, double factor)
    {
        return Math.Abs(forces.MinAxialForce / cr) + Math.Abs(factor * u1major * forces.MaxAbsMuMajor / mrMajor) + Math.Abs(β * u1Minor * forces.MaxAbsMuMinor / mrMinor);
    }
}
