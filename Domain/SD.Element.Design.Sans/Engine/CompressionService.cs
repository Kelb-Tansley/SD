using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Models;

namespace SD.Element.Design.Sans.Engine;
public class CompressionService : SansService
{
    /// <summary>
    /// Determines the compressive resistance to section 13.1 of SANS 10162-1
    /// </summary>
    public static CompressionResistance CompressiveResistance(Beam beam, SectionClassification classification, double cu, bool kIsOne = false) //Section 13.3 of SANS 10162-1
    {
        switch (beam.Section.SectionType)
        {
            case SectionType.IorH:
                {
                    var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4IorH(beam.Section, classification, cu);
                    var cr = DoublyAndAxiSymmetricSections(section: beam.Section,
                                                           area: aef,
                                                           ky: kIsOne ? 1D : beam.BeamChain.K1,
                                                           kx: kIsOne ? 1D : beam.BeamChain.K2,
                                                           kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                           ly: beam.BeamChain.L1,
                                                           lx: beam.BeamChain.L2,
                                                           lz: beam.BeamChain.Lz);
                    return new CompressionResistance(crMajor: cr.Crx,
                                                     crMinor: cr.Cry,
                                                     crTF: cr.Crz,
                                                     feMajor: cr.Fex,
                                                     feMinor: cr.Fey,
                                                     feMajorK1: cr.FexK1,
                                                     feMinorK1: cr.FeyK1);
                }
            case SectionType.LipChannel:
                {
                    var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4Channel(beam.Section, classification, cu);
                    var cr = SinglySymmetricSections(section: beam.Section,
                                                     area: aef,
                                                     kMinor: kIsOne ? 1D : beam.BeamChain.K1,
                                                     kMajor: kIsOne ? 1D : beam.BeamChain.K2,
                                                     kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                     lMinor: beam.BeamChain.L1,
                                                     lMajor: beam.BeamChain.L2,
                                                     lz: beam.BeamChain.Lz,
                                                     isMajorSymmetric: true);
                    return new CompressionResistance(crMajor: Math.Min(cr.Cry, cr.Cryz),
                                                     crMinor: cr.Crx,
                                                     crTF: cr.Cryz,
                                                     feMajor: cr.Fey,
                                                     feMinor: cr.Fex,
                                                     feMajorK1: cr.FeyK1,
                                                     feMinorK1: cr.FexK1);
                }
            case SectionType.Angle:
                {
                    // Equal leg angles are singly symmetric
                    if (beam.Section.D == beam.Section.B1)
                    {

                        var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4Angle(beam.Section, classification, cu);
                        var cr = SinglySymmetricSections(section: beam.Section,
                                                         area: aef,
                                                         kMinor: kIsOne ? 1D : beam.BeamChain.K1,
                                                         kMajor: kIsOne ? 1D : beam.BeamChain.K2,
                                                         kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                         lMinor: beam.BeamChain.L1,
                                                         lMajor: beam.BeamChain.L2,
                                                         lz: beam.BeamChain.Lz,
                                                         isMajorSymmetric: true);
                        return new CompressionResistance(crMajor: Math.Min(cr.Cry, cr.Cryz),
                                                         crMinor: cr.Crx,
                                                         crTF: cr.Cryz,
                                                         feMajor: cr.Fey,
                                                         feMinor: cr.Fex,
                                                         feMajorK1: cr.FeyK1,
                                                         feMinorK1: cr.FexK1);
                    }
                    else
                    {
                        var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4Angle(beam.Section, classification, cu);
                        var cr = AsymmetricSections(section: beam.Section,
                                                    area: aef,
                                                    ky: kIsOne ? 1D : beam.BeamChain.K1,
                                                    kx: kIsOne ? 1D : beam.BeamChain.K2,
                                                    kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                    ly: beam.BeamChain.L1,
                                                    lx: beam.BeamChain.L2,
                                                    lz: beam.BeamChain.Lz);
                        return new CompressionResistance(crMajor: Math.Min(cr.Cry, cr.Cryz),
                                                         crMinor: cr.Crx,
                                                         crTF: cr.Cryz,
                                                         feMajor: cr.Fey,
                                                         feMinor: cr.Fex,
                                                         feMajorK1: cr.FeyK1,
                                                         feMinorK1: cr.FexK1);
                    }
                }
            case SectionType.CircularHollow:
                {
                    var aef = classification.Section != ElementClass.Class4 ?
                        beam.Section.Agr : GetEffectiveAreaForClass4Circular(beam.Section, cu);
                    var cr = DoublyAndAxiSymmetricSections(section: beam.Section,
                                                           area: aef,
                                                           ky: kIsOne ? 1D : beam.BeamChain.K1,
                                                           kx: kIsOne ? 1D : beam.BeamChain.K2,
                                                           kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                           ly: beam.BeamChain.L1,
                                                           lx: beam.BeamChain.L2,
                                                           lz: beam.BeamChain.Lz);
                    return new CompressionResistance(crMajor: cr.Crx,
                                                     crMinor: cr.Cry,
                                                     crTF: cr.Crz,
                                                     feMajor: cr.Fex,
                                                     feMinor: cr.Fey,
                                                     feMajorK1: cr.FexK1,
                                                     feMinorK1: cr.FeyK1);
                }
            case SectionType.RectangularHollow:
                {
                    var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4Rectangular(beam.Section, classification, cu);
                    var cr = DoublyAndAxiSymmetricSections(section: beam.Section,
                                                           area: aef,
                                                           ky: kIsOne ? 1D : beam.BeamChain.K1,
                                                           kx: kIsOne ? 1D : beam.BeamChain.K2,
                                                           kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                           ly: beam.BeamChain.L1,
                                                           lx: beam.BeamChain.L2,
                                                           lz: beam.BeamChain.Lz);
                    return new CompressionResistance(crMajor: cr.Crx,
                                                     crMinor: cr.Cry,
                                                     crTF: cr.Crz,
                                                     feMajor: cr.Fex,
                                                     feMinor: cr.Fey,
                                                     feMajorK1: cr.FexK1,
                                                     feMinorK1: cr.FeyK1);
                }
            case SectionType.T:
                {
                    var isYMajor = ((TSection)beam.Section).IsYMajor;
                    var aef = classification.Section != ElementClass.Class4 ? beam.Section.Agr : GetEffectiveAreaForClass4Tee(beam.Section, classification, cu);
                    var cr = SinglySymmetricSections(section: beam.Section,
                                                     area: aef,
                                                     kMajor: kIsOne ? 1D : isYMajor ? beam.BeamChain.K2 : beam.BeamChain.K1,
                                                     kMinor: kIsOne ? 1D : isYMajor ? beam.BeamChain.K1 : beam.BeamChain.K2,
                                                     kz: kIsOne ? 1D : beam.BeamChain.Kz,
                                                     lMajor: isYMajor ? beam.BeamChain.L2 : beam.BeamChain.L1,
                                                     lMinor: isYMajor ? beam.BeamChain.L1 : beam.BeamChain.L2,
                                                     lz: beam.BeamChain.Lz,
                                                     isYMajor);
                    return new CompressionResistance(crMajor: isYMajor ? Math.Min(cr.Cry, cr.Cryz) : Math.Min(cr.Crx, cr.Cryz),
                                                     crMinor: isYMajor ? cr.Crx : cr.Cry,
                                                     crTF: cr.Cryz,
                                                     feMajor: isYMajor ? cr.Fey : cr.Fex,
                                                     feMinor: isYMajor ? cr.Fex : cr.Fey,
                                                     feMajorK1: isYMajor ? cr.FeyK1 : cr.FexK1,
                                                     feMinorK1: isYMajor ? cr.FexK1 : cr.FeyK1);
                }
        }
        throw new NotImplementedException(nameof(CompressiveResistance));
    }

    private static SansLocalCompressionResistance DoublyAndAxiSymmetricSections(Section section, double area, double kx, double ky, double kz, double lx, double ly, double lz)
    {
        var ro2 = Math.Pow(section.AMajor, 2) + Math.Pow(section.AMinor, 2) + Math.Pow(section.RMajor, 2) + Math.Pow(section.RMinor, 2);
        var Ω = 1 - ((Math.Pow(section.AMajor, 2) + Math.Pow(section.AMinor, 2)) / ro2);

        var fexK1 = CalculateFe(section, section.RMajor, 1, lx);
        var feyK1 = CalculateFe(section, section.RMinor, 1, ly);

        var fex = CalculateFe(section, section.RMajor, kx, lx);
        var fey = CalculateFe(section, section.RMinor, ky, ly);
        var fez = CalculateFez(section, kz, lz, ro2);

        //1,34 for hot-rolled, fabricated structural sections, and hollow structural sections manufactured according to
        //SANS 657 - 1(cold - formed non - stress - relieved); or 2,24 for doubly symmetric welded three - plate members
        //with flange edges oxy - flame - cut and hollow structural sections manufactured according to
        //ISO 657 - 14(hot - formed or cold - formed stress - relieved).
        var n = section.IsPlateGirder ? 2.24 : 1.34;

        return new SansLocalCompressionResistance()
        {
            Fex = fex,
            Fey = fey,
            FexK1 = fexK1,
            FeyK1 = feyK1,
            Crx = AxisCompressiveResistance(area, section.Material.MinFy, fex, n),
            Cry = AxisCompressiveResistance(area, section.Material.MinFy, fey, n),
            Crz = AxisCompressiveResistance(area, section.Material.MinFy, fez, n)
        };
    }

    public static double AxisCompressiveResistance(double area, double fy, double f, double n)
    {
        return Φ * area * fy * Math.Pow(1 + Math.Pow(Math.Sqrt(fy / f), 2 * n), -1 / n);
    }

    private static SansLocalCompressionResistance SinglySymmetricSections(Section section, double area, double kMajor, double kMinor, double kz, double lMajor, double lMinor, double lz, bool isMajorSymmetric = false)
    {
        // y axis taken as the axis of symmetry
        var ro2 = Math.Pow(section.AMajor, 2) + Math.Pow(section.AMinor, 2) + Math.Pow(section.RMajor, 2) + Math.Pow(section.RMinor, 2);
        var Ω = 1 - ((Math.Pow(section.AMajor, 2) + Math.Pow(section.AMinor, 2)) / ro2);

        var fexK1 = isMajorSymmetric ? CalculateFe(section, section.RMinor, 1, lMinor) : CalculateFe(section, section.RMajor, 1, lMajor);
        var feyK1 = isMajorSymmetric ? CalculateFe(section, section.RMajor, 1, lMajor) : CalculateFe(section, section.RMinor, 1, lMinor);

        var fex = isMajorSymmetric ? CalculateFe(section, section.RMinor, kMinor, lMinor) : CalculateFe(section, section.RMajor, kMajor, lMajor);
        var fey = isMajorSymmetric ? CalculateFe(section, section.RMajor, kMajor, lMajor) : CalculateFe(section, section.RMinor, kMinor, lMinor);

        var fez = CalculateFez(section, kz, lz, ro2);

        var feyz = (fey + fez) / (2 * Ω) * (1 - Math.Sqrt(1 - 4 * fey * fez * Ω / Math.Pow(fey + fez, 2)));

        //1,34 for hot-rolled, fabricated structural sections, and hollow structural sections manufactured according to
        //SANS 657 - 1(cold - formed non - stress - relieved); 
        var n = 1.34;

        return new SansLocalCompressionResistance()
        {
            Fex = fex,
            Fey = fey,
            FexK1 = fexK1,
            FeyK1 = feyK1,
            Crx = AxisCompressiveResistance(area, section.Material.MinFy, fex, n),
            Cry = AxisCompressiveResistance(area, section.Material.MinFy, fey, n),
            Cryz = AxisCompressiveResistance(area, section.Material.MinFy, feyz, n)
        };
    }
    private static SansLocalCompressionResistance AsymmetricSections(Section section, double area, double kx, double ky, double kz, double lx, double ly, double lz)
    {
        var xo = section.AMinor;
        var yo = section.AMajor;
        var ro2 = Math.Pow(xo, 2) + Math.Pow(yo, 2) + Math.Pow(section.RMajor, 2) + Math.Pow(section.RMinor, 2);

        var fexK1 = CalculateFe(section, section.RMinor, 1, lx);
        var feyK1 = CalculateFe(section, section.RMajor, 1, ly);
        var fex = CalculateFe(section, section.RMinor, kx, lx);
        var fey = CalculateFe(section, section.RMajor, ky, ly);
        var fez = CalculateFez(section, kz, lz, ro2);

        //If we represent the function with x and simple constants we set:
        //f(x) = (x - a) (x - b) (x - c) - x^2 (x - b) d - x^2 (x - a) e

        //From wolframalpha, the first derivative is:
        //d/dx(f(x)) = a (b + c + 2 (e - 1) x) + b (c + 2 (d - 1) x) - x (2 c + 3 x (d + e - 1))

        var a = fex;
        var b = fey;
        var c = fez;
        var d = Math.Pow(xo, 2) / ro2;
        var e = Math.Pow(yo, 2) / ro2;

        var x = 0D;
        var maxIterations = 100;
        var accuracy = 0.00001D;

        for (int t = 0; t < maxIterations; t++)
        {
            //Primary function
            var fx = (x - a) * (x - b) * (x - c) - Math.Pow(x, 2) * (x - b) * d - Math.Pow(x, 2) * (x - a) * e;

            //First derivative of the primary function
            var dfx = a * (b + c + 2 * (e - 1) * x) + b * (c + 2 * (d - 1) * x) - x * (2 * c + 3 * x * (d + e - 1));

            if (Math.Abs(fx / dfx) > accuracy)
                x = x - fx / dfx;
            else
                break;

            if (t == maxIterations - 1)
                x = 0; // Return the null root if no root is found
        }

        var fe = x;

        //1,34 for hot-rolled, fabricated structural sections, and hollow structural sections manufactured according to
        //SANS 657 - 1(cold - formed non - stress - relieved); 
        var n = 1.34;

        return new SansLocalCompressionResistance()
        {
            Fex = fex,
            Fey = fey,
            FexK1 = fexK1,
            FeyK1 = feyK1,
            Crx = AxisCompressiveResistance(area, section.Material.MinFy, fex, n),
            Cry = AxisCompressiveResistance(area, section.Material.MinFy, fey, n),
            Cryz = AxisCompressiveResistance(area, section.Material.MinFy, fe, n)
        };
    }

    private static double CalculateFe(Section section, double r, double k, double l)
    {
        return Math.Pow(Math.PI, 2) * section.Material.Es / Math.Pow(k * l / r.MinZero(), 2).MinZero();
    }
    private static double CalculateFez(Section section, double kz, double lz, double ro2)
    {
        return (Math.Pow(Math.PI, 2) * section.Material.Es * section.Cw / Math.Pow(kz * lz, 2).MinZero() + section.Material.Gs * section.J) * (1 / (section.Agr * ro2).MinZero());
    }

    private static double GetEffectiveAreaForClass4IorH(Section section, SectionClassification classification, double cu)
    {
        var kFlange = 0.43D;
        var kWeb = 4D;

        var f = cu / section.Agr;

        var aef1 = classification.Element1 == ElementClass.Class4 ?
            section.T1 * 2 * GetReducedElementWidth(section.B1 / 2, section.T1, kFlange, section.Material.Es, f, section.Material.FyElement1, 200D) : 0;
        var aef2 = classification.Element2 == ElementClass.Class4 ?
            section.T2 * 2 * GetReducedElementWidth(section.B2 / 2, section.T2, kFlange, section.Material.Es, f, section.Material.FyElement2, 200D) : 0;
        var aef3 = classification.Element3 == ElementClass.Class4 ?
            section.T3 * GetReducedElementWidth(((IorHSection)section).Hw, section.T3, kWeb, section.Material.Es, f, section.Material.FyElement3, 670D) : 0;

        return section.Agr - aef1 - aef2 - aef3;
    }
    private static double GetEffectiveAreaForClass4Channel(Section section, SectionClassification classification, double cu)
    {
        var kFlange = 0.43D;
        var kWeb = 4D;

        var f = cu / section.Agr;

        var aef1 = classification.Element1 == ElementClass.Class4 ?
            section.T1 * 2 * GetReducedElementWidth(section.B1, section.T1, kFlange, section.Material.Es, f, section.Material.FyElement1, 200D) : 0;
        var aef2 = classification.Element2 == ElementClass.Class4 ?
            section.T2 * GetReducedElementWidth(((ChannelSection)section).Hw, section.T2, kWeb, section.Material.Es, f, section.Material.FyElement2, 670D) : 0;

        return section.Agr - aef1 - aef2;
    }
    private static double GetEffectiveAreaForClass4Angle(Section section, SectionClassification classification, double cu)
    {
        var k = 0.43D;

        var f = cu / section.Agr;

        var aef1 = classification.Element1 == ElementClass.Class4 ?
            section.T1 * GetReducedElementWidth(section.B1, section.T1, k, section.Material.Es, f, section.Material.FyElement1, 200D) : 0;
        var aef2 = classification.Element2 == ElementClass.Class4 ?
            section.T1 * GetReducedElementWidth(section.D, section.T1, k, section.Material.Es, f, section.Material.FyElement2, 200D) : 0;

        return section.Agr - aef1 - aef2;
    }
    private static double GetEffectiveAreaForClass4Rectangular(Section section, SectionClassification classification, double cu)
    {
        var k = 4D;

        var f = cu / section.Agr;

        var aef1 = classification.Element1 == ElementClass.Class4 ?
            section.T1 * 2 * GetReducedElementWidth(section.B1 - 4 * section.T1, section.T1, k, section.Material.Es, f, section.Material.FyElement1, 670D) : 0;
        var aef2 = classification.Element2 == ElementClass.Class4 ?
            section.T2 * 2 * GetReducedElementWidth(section.D - 4 * section.T2, section.T2, k, section.Material.Es, f, section.Material.FyElement2, 670D) : 0;

        return section.Agr - aef1 - aef2;
    }
    private static double GetEffectiveAreaForClass4Tee(Section section, SectionClassification classification, double cu)
    {
        var k = 4D;

        var f = cu / section.Agr;

        var aef1 = classification.Element1 == ElementClass.Class4 ?
            section.T1 * 2 * GetReducedElementWidth(section.B1 / 2, section.T1, k, section.Material.Es, f, section.Material.FyElement1, 200D) : 0;
        var aef2 = classification.Element2 == ElementClass.Class4 ?
            section.T2 * GetReducedElementWidth(section.D, section.T2, k, section.Material.Es, f, section.Material.FyElement2, 340D) : 0;

        return section.Agr - aef1 - aef2;
    }
    private static double GetEffectiveAreaForClass4Circular(Section section, double cu)
    {
        var class4Limit = 23000 / section.Material.FyElement1;
        return section.Agr * class4Limit * section.T1 / section.D;
    }
    private static double GetReducedElementWidth(double b, double t, double k, double E, double f, double fy, double class4Limit)
    {
        var wlim = 0.644 * Math.Sqrt(k * E / f);
        var w = b / t;

        return w <= wlim ? 0D : b - t * class4Limit / Math.Sqrt(fy);

        //var temp = Math.Sqrt(k * E / f);
        //return w <= wlim ? b * t : Math.Min(0.95D * t * temp * (1 - (0.208D / w) * temp), b) * t;
    }
}