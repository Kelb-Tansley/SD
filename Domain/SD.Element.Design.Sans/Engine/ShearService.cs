using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Models;

namespace SD.Element.Design.Sans.Engine;
public class ShearService : SansService
{
    /// <summary>
    /// Determines the shear resistance of a beam to section 13.4 of SANS 10162-1
    /// </summary>
    public static ShearResistance ShearResistance(Section bp)
    {
        return bp.SectionType switch
        {
            SectionType.IorH or SectionType.LipChannel => ShearResistanceForSectionWithTwoFlanges(bp, !bp.IsPlateGirder, bp.T2),
            SectionType.Angle => ShearResistanceForAngleSection(bp, !bp.IsPlateGirder),
            SectionType.T => ShearResistanceForTSections((TSection)bp, !bp.IsPlateGirder),
            SectionType.CircularHollow => ShearResistanceForCircularHollowSection(bp),
            SectionType.RectangularHollow => ShearResistanceForSectionWithTwoFlanges(bp, !bp.IsPlateGirder, bp.T2, 2),
            _ => throw new ArgumentOutOfRangeException(nameof(bp), "Section type is unknown."),
        };
    }

    private static ShearResistance ShearResistanceForCircularHollowSection(Section bp)
    {
        var vr = Φ * (bp.Agr / 4D) * 0.66D * bp.Material.FyElement1;
        return new ShearResistance(vrMajor: vr, vrMinor: vr);
    }

    /// <summary>
    /// Determines the shear resistance of the webs of flexural members with two flanges.
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="isRolled"></param> Plate girders are not rolled sections.
    /// <param name="webCount"></param> Accommodates sections with more than one web. Box sections.
    private static ShearResistance ShearResistanceForSectionWithTwoFlanges(Section bp, bool isRolled, double tw, int webCount = 1)
    {
        // Unsupported web height.
        var hw = bp.D - bp.T1 - bp.T2;

        // Centre-to-centre distance between transverse web stiffeners.
        var s = bp.WebStiffenerSpacing;

        var fy = bp.Material.FyElement1;

        // Shear buckling coefficient, 13.4.1.1
        var kv = s / hw < 1D ? 4D + 5.34D / Math.Pow(s / hw, 2) : 5.34D + 4D / Math.Pow(s / hw, 2);

        // Shear Buckling stress ratio. It is important that Fy is in MPa here.
        var bucklingRatio = Math.Sqrt(kv / fy);

        // Aspect ratio of web.
        var λweb = hw / tw;

        var fs = 0D;
        if (λweb <= 440D * bucklingRatio)
            fs = 0.66D * fy;
        if (λweb <= 500D * bucklingRatio && λweb > 440D * bucklingRatio)
            fs = 290D * Math.Sqrt(fy * kv) / λweb;
        if (λweb <= 620D * bucklingRatio && λweb > 500D * bucklingRatio)
        {
            // Aspect coefficient.
            var ka = 1D / Math.Sqrt(1 + Math.Pow(s / hw, 2));
            var fcri = 290D * Math.Sqrt(fy * kv) / λweb;
            var ft = ka * (0.5D * fy - 0.866D * fcri);

            fs = fcri + ft;
        }
        else if (λweb > 620D * bucklingRatio)
        {
            // Aspect coefficient.
            var ka = 1D / Math.Sqrt(1 + Math.Pow(s / hw, 2));
            var fcre = 180000D * kv / Math.Pow(λweb, 2);
            var ft = ka * (0.5D * fy - 0.866D * fcre);
            fs = fcre + ft;
        }

        return new ShearResistance(
            vrMajor: Φ * ((isRolled ? bp.D : hw) * tw * webCount) * fs,
            vrMinor: Φ * (bp.B1 * bp.T1 * 2) * 0.66 * fy
            );
    }

    /// <summary>
    /// Determines the shear resistance of the webs of T sections.
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="isRolled"></param> Plate girders are not rolled sections.
    private static ShearResistance ShearResistanceForTSections(TSection bp, bool isRolled)
    {
        // Web height. Full H is used for rolled steel sections.
        var hw = isRolled ? bp.D : bp.D - bp.T1;
        var bw = isRolled ? bp.B1 : bp.B1 - bp.T2;

        var fs = 0.66D * bp.Material.FyElement1;

        var vrMajor = bp.IsYMajor ? Φ * hw * bp.T2 * fs : Φ * bw * bp.T1 * fs;
        var vrMinor = bp.IsYMajor ? Φ * bw * bp.T1 * fs : Φ * hw * bp.T2 * fs;

        return new ShearResistance(vrMajor: vrMajor, vrMinor: vrMinor);
    }

    /// <summary>
    /// Determines the shear resistance of the webs of angle sections.
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="isRolled"></param> Plate girders are not rolled sections.
    private static ShearResistance ShearResistanceForAngleSection(Section bp, bool isRolled)
    {
        // Web height. Full H is used for rolled steel sections.
        var hw = isRolled ? bp.D : bp.D - bp.T1;
        var bw = isRolled ? bp.B1 : bp.B1 - bp.T1;

        var fs = 0.66D * bp.Material.FyElement1;

        return new ShearResistance(vrMajor: Φ * (hw * bp.T1) * fs, vrMinor: Φ * (bw * bp.T1) * fs);
    }
}
