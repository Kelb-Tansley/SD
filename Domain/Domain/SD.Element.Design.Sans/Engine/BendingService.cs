using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Models;

namespace SD.Element.Design.Sans.Engine;
public class BendingService : SansService
{
    /// <summary>
    /// Determines the bending resistance to section 13.5 and 13.6 of SANS 10162-1
    /// </summary>
    public static MomentResistance? MomentResistance(Beam beam, BeamForces forces, BendingConstants sbc, SectionClassification flexClass)
    {
        Calculateω2Major(forces, sbc);
        Calculateω2Minor(forces, sbc);
        if (sbc.McrMajorω == 0 || sbc.McrMinorω == 0)
            return null;

        switch (beam.Section.SectionType)
        {
            case SectionType.IorH:
                return IorHMomentResistance(beam, flexClass, sbc.McrMajorω, sbc.McrMinorω, forces.MaxAbsMuMajor, forces.MinAxialForce, beam.Section as IorHSection);
            case SectionType.LipChannel:
                return ChannelMomentResistance(beam, flexClass, sbc.McrMajorω, sbc.McrMinorω, forces.MaxAbsMuMajor, forces.MinAxialForce, beam.Section as ChannelSection);
            case SectionType.Angle:
                break;
            case SectionType.CircularHollow:
                return CircularMomentResistance(beam, flexClass, sbc.McrMajorω, sbc.McrMinorω, beam.Section as CircularSection);
            case SectionType.RectangularHollow:
                return RectangularMomentResistance(beam, flexClass, sbc.McrMajorω, sbc.McrMinorω, beam.Section as RectangularSection);
            case SectionType.T:
                return TeeMomentResistance(beam, flexClass, sbc.McrMajorω, sbc.McrMinorω, forces.MaxAbsMuMajor, forces.MinAxialForce, beam.Section as TSection);
        }

        throw new ArgumentNullException(nameof(MomentResistance));
    }

    /// <summary>
    /// ω2 is the coefficient to account for increased moment resistance of a laterally unsupported beam segment when subject to a moment gradient. 
    /// Defined in section 13.6(a) of the SANS code for the major axis.
    /// </summary>
    /// <param name="forces"></param>
    /// <param name="sbc"></param>
    private static void Calculateω2Major(BeamForces forces, BendingConstants sbc)
    {
        //The ratio of the smaller factored moment to the larger factored moment at opposite ends of the unbraced length, positive for double curvature and negative for single curvature
        var κ = forces.SmallerStartOrEndMuMajor() / forces.LargerStartOrEndMuMajor();

        //Positive implies double curvature while - is single
        var curvature = forces.MinMuMajor < 0 & forces.MaxMuMajor > 0 ? 1 : -1;
        sbc.Curvature = curvature > 0 ? 2 : 1;

        κ = Math.Abs(κ) * curvature;

        //ω2 = 1,75 + 1,05κ + 0,3κ2 ≤ 2,5 for unbraced lengths subject to end moments; or = 1,0 when the bending moment at any point within the unbraced length is larger
        //than the larger end moment or when there is no effective lateral support for the compression flange at one of the ends of the unsupported length.
        // TODO: How can we interpret this last condition from the Strand7 model?

        if (forces.MaxAbsMuMajor <= Math.Max(Math.Abs(forces.StartMuMajor), Math.Abs(forces.EndMuMajor)))
        {
            sbc.Loadω2Case = 1;
            sbc.McrMajorω = Math.Min(1.75D + 1.05D * κ + 0.3D * Math.Pow(κ, 2), 2.5D);
        }
        else
        {
            sbc.Loadω2Case = 2;
            sbc.McrMajorω = 1;
        }
    }
    /// <summary>
    /// ω2 is the coefficient to account for increased moment resistance of a laterally unsupported beam segment when subject to a moment gradient. 
    /// Defined in section 13.6(a) of the SANS code for the minor axis.
    /// </summary>
    /// <param name="forces"></param>
    /// <param name="sbc"></param>
    private static void Calculateω2Minor(BeamForces forces, BendingConstants sbc)
    {
        //The ratio of the smaller factored moment to the larger factored moment at opposite ends of the unbraced length, positive for double curvature and negative for single curvature
        var κ = forces.SmallerStartOrEndMuMinor() / forces.LargerStartOrEndMuMinor();

        //Positive implies double curvature while - is single
        var curvature = forces.MinMuMinor < 0 & forces.MaxMuMinor > 0 ? 1 : -1;

        κ *= curvature;

        //ω2 = 1,75 + 1,05κ + 0,3κ2 ≤ 2,5 for unbraced lengths subject to end moments; or = 1,0 when the bending moment at any point within the unbraced length is larger
        //than the larger end moment or when there is no effective lateral support for the compression flange at one of the ends of the unsupported length.
        // TODO: How can we interpret this last condition from the Strand7 model?
        if (forces.MaxAbsMuMinor <= Math.Max(Math.Abs(forces.StartMuMinor), Math.Abs(forces.EndMuMinor)))
        {
            sbc.McrMinorω = Math.Min(1.75D + 1.05D * κ + 0.3D * Math.Pow(κ, 2), 2.5D);
        }
        else
        {
            sbc.McrMinorω = 1;
        }
    }
    private static MomentResistance RectangularMomentResistance(Beam beam, SectionClassification flexClass, double w2Major, double w2Minor, RectangularSection? section)
    {
        if (section == null || flexClass == null)
            throw new ArgumentNullException(nameof(RectangularSection));

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMrplMajor(section), GetMrplMinor(section));
        else if (flexClass.Section == ElementClass.Class4)
        {
            var fy_e = Math.Min(670D / Math.Sqrt(section.Material.MinFy) / ((section.B1 - 4 * section.T1) / section.T1), 1);
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section) * fy_e, GetMreMinor(section) * fy_e);
        }
        return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section), GetMreMinor(section));
    }

    private static MomentResistance CircularMomentResistance(Beam beam, SectionClassification flexClass, double w2Major, double w2Minor, CircularSection? section)
    {
        if (section == null || flexClass == null)
            throw new ArgumentNullException(nameof(CircularSection));

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMrplMajor(section), GetMrplMinor(section));
        else if (flexClass.Section == ElementClass.Class4)
        {
            var fy_e = Math.Min(66000D / section.Material.MinFy / (section.D / section.T1), 1);
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section) * fy_e, GetMreMinor(section) * fy_e);
        }
        return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section), GetMreMinor(section));
    }

    private static MomentResistance? ChannelMomentResistance(Beam beam, SectionClassification flexClass, double w2Major, double w2Minor, double muMajor, double Cu, ChannelSection? section)
    {
        if (section == null || flexClass == null)
            throw new ArgumentNullException(nameof(ChannelSection));

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMrplMajor(section), GetMrplMinor(section), GetMreMajor(section), GetMreMinor(section));
        else if (flexClass.Section == ElementClass.Class4)
        {
            if (flexClass.Element3 != ElementClass.Class4 && (flexClass.Element1 == ElementClass.Class4 || flexClass.Element2 == ElementClass.Class4))
                return GetMomentResistance(beam, w2Major, w2Minor, section, Math.Min(GetMreffMajor(section), GetMreMajor(section)), Math.Min(GetMreffMinor(section), GetMreMinor(section))); // If the flanges are class 4 but not the web
            else if (flexClass.Element3 == ElementClass.Class4 && flexClass.Element1 != ElementClass.Class4 && flexClass.Element2 != ElementClass.Class4)
            {
                // If the flanges are not class 4 but the web is
                var λw = section.Hw / section.T2; // Web slenderness ratio
                var λwLimit = 1900D / Math.Sqrt(muMajor / (Φ * section.ZeMajor)); // Web slenderness limit

                if (λw > 83000D / Math.Sqrt(section.Material.FyElement1))
                {
                    section.CanDesign = false;
                    return null;
                }

                if (λw >= λwLimit)
                {
                    var aw = section.Ag3;
                    var af = 2 * section.Ag1;
                    var αwf = Math.Min(1D - 0.0005D * (aw / af) * (λw - λwLimit), 1);

                    if (Cu < 0) // If a compressive force is present
                        αwf *= 1 - 0.65 * Math.Abs(Cu) / (Φ * section.Agr * section.Material.MinFy);

                    return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section) * αwf, GetMreMinor(section));
                }
            }
            else if (flexClass.Element3 == ElementClass.Class4 && flexClass.Element1 == ElementClass.Class4)
            {
                // If the flanges are class 4 and the web is then SANS 10162:1 cannot design the section
                section.CanDesign = false;
                return null;
            }
        }

        return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section), GetMreMinor(section));
    }

    private static MomentResistance? IorHMomentResistance(Beam beam, SectionClassification flexClass, double w2Major, double w2Minor, double muMajor, double Cu, IorHSection? section)
    {
        if (section == null || flexClass == null)
            throw new ArgumentNullException(nameof(IorHSection));

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
            return GetMomentResistance(beam, w2Major, w2Minor, section, GetMrplMajor(section), GetMrplMinor(section));
        else if (flexClass.Section == ElementClass.Class4)
        {
            if (flexClass.Element3 != ElementClass.Class4 && (flexClass.Element1 == ElementClass.Class4 || flexClass.Element2 == ElementClass.Class4))
                return GetMomentResistance(beam, w2Major, w2Minor, section, Math.Min(GetMreffMajor(section), GetMreMajor(section)), Math.Min(GetMreffMinor(section), GetMreMinor(section))); // If the flanges are class 4 but not the web
            else if (flexClass.Element3 == ElementClass.Class4 && flexClass.Element1 != ElementClass.Class4 && flexClass.Element2 != ElementClass.Class4)
            {
                // If the flanges are not class 4 but the web is
                var λw = section.Hw / section.T3; // Web slenderness ratio
                var λwLimit = 1900D / Math.Sqrt(muMajor / (Φ * section.ZeMajor)); // Web slenderness limit

                if (λw > 83000D / Math.Sqrt(section.Material.FyElement1))
                {
                    section.CanDesign = false;
                    return null;
                }

                if (λw >= λwLimit)
                {
                    var aw = section.Ag3;
                    var af = section.Ag1 + section.Ag2;
                    var αwf = Math.Min(1D - 0.0005D * (aw / af) * (λw - λwLimit), 1);

                    if (Cu < 0) // If a compressive force is present
                        αwf *= 1 - 0.65 * Math.Abs(Cu) / (Φ * section.Agr * section.Material.MinFy);

                    return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section) * αwf, GetMreMinor(section));
                }
            }
            else if (flexClass.Element3 == ElementClass.Class4 && flexClass.Element1 == ElementClass.Class4)
            {
                // If the flanges are class 4 and the web is then SANS 10162:1 cannot design the section
                section.CanDesign = false;
                return null;
            }
        }

        return GetMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section), GetMreMinor(section));
    }

    private static MomentResistance? TeeMomentResistance(Beam beam, SectionClassification flexClass, double w2Major, double w2Minor, double muMajor, double Cu, TSection? section)
    {
        if (section == null || flexClass == null)
            throw new ArgumentNullException(nameof(TSection));

        if (flexClass.Section == ElementClass.Class1 || flexClass.Section == ElementClass.Class2)
            return GetMonoSymMomentResistance(beam, w2Major, w2Minor, section, GetMrplMajor(section), GetMrplMinor(section));
        else if (flexClass.Section == ElementClass.Class4)
        {
            if (flexClass.Element2 != ElementClass.Class4 && flexClass.Element1 == ElementClass.Class4)
                // Class 4 flange detected, but not the web
                return GetMonoSymMomentResistance(beam, w2Major, w2Minor, section, Math.Min(GetMreffMajor(section), GetMreMajor(section)), Math.Min(GetMreffMinor(section), GetMreMinor(section)));
            else if (flexClass.Element2 == ElementClass.Class4 && flexClass.Element1 != ElementClass.Class4)
            {
                // If the flanges are not class 4 but the web is
                var λw = section.Hw / section.T2; // Web slenderness ratio
                var λwLimit = 1900D / Math.Sqrt(muMajor / (Φ * section.ZeMajor)); // Web slenderness limit

                if (λw > 83000D / Math.Sqrt(section.Material.FyElement2))
                {
                    section.CanDesign = false;
                    return null;
                }

                // This limit may be waived if analysis indicates that buckling of the compression flange into the web will not occur at factored load levels. 
                if (λw >= λwLimit)
                {
                    var αwf = Math.Min(1D - 0.0005D * (section.Ag2 / section.Ag1) * (λw - λwLimit), 1D);

                    if (Cu < 0) // If a compressive force is present
                        αwf *= 1 - 0.65 * Math.Abs(Cu) / (Φ * section.Agr * section.Material.MinFy);

                    return GetMonoSymMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section) * αwf, GetMreMinor(section));
                }
            }
            else if (flexClass.Element3 == ElementClass.Class4 && flexClass.Element1 == ElementClass.Class4)
            {
                // If the flanges are class 4 and the web is then SANS 10162:1 cannot design the section
                section.CanDesign = false;
                return null;
            }
        }

        return GetMonoSymMomentResistance(beam, w2Major, w2Minor, section, GetMreMajor(section), GetMreMinor(section));
    }

    private static double GetMreffMajor(Section section)
    {
        var b1 = ClassificationService.GetBreadthEffective(section);
        if (b1 < section.B1)
        {
            return section.SectionType switch
            {
                SectionType.IorH => new IorHSection(b1, b1, section.D, section.T1, section.T2, section.T3, section.Material).ZeMajor * section.Material.MinFy,
                SectionType.LipChannel => new ChannelSection(b1, section.D, section.T1, section.T2, section.Material).ZeMajor * section.Material.MinFy,
                _ => 0,
            };
        }
        return section.ZeMajor * section.Material.MinFy;
    }
    private static double GetMreffMinor(Section section)
    {
        var b1 = ClassificationService.GetBreadthEffective(section);
        if (b1 < section.B1)
        {
            return section.SectionType switch
            {
                SectionType.IorH => new IorHSection(b1, b1, section.D, section.T1, section.T2, section.T3, section.Material).ZeMinor * section.Material.MinFy,
                SectionType.LipChannel => new ChannelSection(b1, section.D, section.T1, section.T2, section.Material).ZeMinor * section.Material.MinFy,
                _ => 0,
            };
        }
        return section.ZeMinor * section.Material.MinFy;
    }
    private static double GetMreMajor(Section section)
    {
        return section.ZeMajor * section.Material.MinFy;
    }
    private static double GetMreMinor(Section section)
    {
        return section.ZeMinor * section.Material.MinFy;
    }
    private static double GetMrplMajor(Section section)
    {
        return section.ZplMajor * section.Material.MinFy;
    }
    private static double GetMrplMinor(Section section)
    {
        return section.ZplMinor * section.Material.MinFy;
    }

    private static MomentResistance GetMomentResistance(Beam beam, double w2Major, double w2Minor, Section section, double mrMajor, double mrMinor, double? mrUMajor = null, double? mrUMinor = null)
    {
        return new MomentResistance()
        {
            MrMajorSupported = Φ * mrMajor,
            MrMinorSupported = Φ * mrMinor,
            MrMajorTopUnsupported = UnsupportedMr(beam.BeamChain.KeTop * beam.BeamChain.LeTop, section.IMinor, w2Major, mrUMajor == null ? mrMajor : (double)mrUMajor, section),
            MrMajorBottomUnsupported = UnsupportedMr(beam.BeamChain.KeBottom * beam.BeamChain.LeBottom, section.IMinor, w2Major, mrUMajor == null ? mrMajor : (double)mrUMajor, section),
            MrMinorTopUnsupported = UnsupportedMr(beam.BeamChain.KeTop * beam.BeamChain.LeTop, section.IMajor, w2Minor, mrUMinor == null ? mrMinor : (double)mrUMinor, section),
            MrMinorBottomUnsupported = UnsupportedMr(beam.BeamChain.KeBottom * beam.BeamChain.LeBottom, section.IMajor, w2Minor, mrUMinor == null ? mrMinor : (double)mrUMinor, section)
        };
    }

    private static MomentResistance GetMonoSymMomentResistance(Beam beam, double w2Major, double w2Minor, Section section, double mrMajor, double mrMinor, double? mrUMajor = null, double? mrUMinor = null)
    {
        double beta_major = 0.0, beta_minor = 0.0;
        // Direction of moment still needs to be included: if flange is in comrpession beta is positive otherwise beta is negative
        if (((TSection)section).IsYMajor) // Need to confirm direction of major & minor
            beta_minor = 0.9D * (section.D - section.T1 / 2D) * (1D - Math.Pow(section.IMajor / section.IMinor, 2)); // This is a approx. equation
                                                                                                                     //   beta_minor = (section.T1 * section.T1 * section.B1 * (6 * section.T1 * section.T1 + section.B1 * section.B1) / 24 +
                                                                                                                     //       (section.D * section.D - section.T1 * section.T1) * section.T2 * (6 * section.D * section.D + 6 * section.T1 * section.T1 + section.T2 * section.T2) / 24 - 
                                                                                                                     //       ((TSection)section).YeNA*(3*section.IMinor+section.IMajor)-Math.Pow(((TSection)section).YeNA, 3)*section.Agr)/section.IMinor - 2*"shearcentre";
        else
            beta_major = 0.9D * (section.D - section.T1 / 2D) * (1D - Math.Pow(section.IMinor / section.IMajor, 2)); // This is a approx. equation

        return new MomentResistance()
        {
            MrMajorSupported = Φ * mrMajor,
            MrMinorSupported = Φ * mrMinor,
            MrMajorTopUnsupported = UnsupportedMonoSymMr(beam.BeamChain.KeTop * beam.BeamChain.LeTop, beta_major, section.IMinor, w2Major, mrUMajor == null ? mrMajor : (double)mrUMajor, section),
            MrMajorBottomUnsupported = UnsupportedMonoSymMr(beam.BeamChain.KeBottom * beam.BeamChain.LeBottom, beta_major, section.IMinor, w2Major, mrUMajor == null ? mrMajor : (double)mrUMajor, section),
            MrMinorBottomUnsupported = UnsupportedMonoSymMr(beam.BeamChain.KeBottom * beam.BeamChain.LeBottom, beta_minor, section.IMajor, w2Minor, mrUMinor == null ? mrMinor : (double)mrUMinor, section),
            MrMinorTopUnsupported = UnsupportedMonoSymMr(beam.BeamChain.KeTop * beam.BeamChain.LeTop, beta_minor, section.IMajor, w2Minor, mrUMinor == null ? mrMinor : (double)mrUMinor, section)
        };
    }

    private static double UnsupportedMr(double kl, double i, double ω2, double m, Section section)
    {
        var mcr = CalculateMcr(kl, i, ω2, section);
        return CalculateUnsupportedMr(mcr, m);
    }

    private static double UnsupportedMonoSymMr(double kl, double beta_x, double i, double ω2, double m, Section section)
    {
        var mcr = CalculateMonoSymMcr(kl, beta_x, i, ω2, section);
        return CalculateUnsupportedMr(mcr, m);
    }

    private static double CalculateMcr(double kl, double i, double ω2, Section section)
    {
        return ω2 * Math.PI / kl * Math.Sqrt(section.Material.Es * i * section.Material.Gs * section.J
            + Math.Pow(section.Material.Es * Math.PI / kl, 2) * i * section.Cw);
    }

    private static double CalculateMonoSymMcr(double kl, double beta_x, double i, double ω2, Section section)
    {
        return ω2 * Math.Pow(Math.PI / kl, 2) * (beta_x / 2 * section.Material.Es * Math.PI * i / kl + Math.Sqrt(Math.Pow(beta_x / 2 * section.Material.Es * Math.PI * i / kl, 2) + section.Material.Es * i * section.Material.Gs * section.J + Math.Pow(section.Material.Es * Math.PI / kl, 2) * i * section.Cw));
    }

    private static double CalculateUnsupportedMr(double mcr, double m)
    {
        //13.6 - Laterally Unsupported Members
        if (Math.Abs(mcr) > 0.67 * Math.Abs(m))
            return Math.Min(1.15 * Φ * m * (1 - 0.28 * m / mcr), Φ * m);
        else
            return Φ * mcr;
    }
}