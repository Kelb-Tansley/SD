using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Core.Shared.Models;
using SD.Core.Strand;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public abstract class BeamPropertiesService : IBeamPropertiesService
{
    public Section GetBeamSection(string? name, SectionType sectionType, bool canDesign, double[] materialData, double[] sectionData, UnitFactor unitFactor, int i, bool isBGLSection, double[] bGLDimensions)
    {
        var structural = isBGLSection
            ? GetBGLStructuralProperties(sectionType, unitFactor, sectionData, materialData, string.Empty, bGLDimensions)
            : GetStructuralProperties(sectionType, unitFactor, sectionData, materialData, string.Empty);

        structural.CanDesign = canDesign;
        structural.Number = i;
        structural.Name = name ?? "NoDesign property name";

        return structural;
    }
    /// <summary>
    /// Beam section properties interpretted from Strand7 API. Read from table on page 1004 of API documentation. 
    /// Strand7 provides 6 properties which define all dimensions:  D1 D2 D3 T1 T2 T3.
    /// </summary>
    private Section GetStructuralProperties(SectionType sectionType, UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        return sectionType switch
        {
            SectionType.IorH => GetIorHStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            SectionType.LipChannel => GetChannelStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            SectionType.Angle => GetAngleStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            SectionType.CircularHollow => GetCircularStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            SectionType.RectangularHollow => GetRectangularStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            SectionType.T => GetTStrand7Section(unitFactor, sectionData, materialData, steelGrade),
            _ => new UnknownSection(sectionType, new Material(0, 0, 0)),
        };
    }
    private Section GetBGLStructuralProperties(SectionType sectionType, UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        return sectionType switch
        {
            SectionType.IorH => GetIorHStrand7BGLSection(unitFactor, sectionData, materialData, steelGrade, bGLDimensions),
            SectionType.LipChannel => GetChannelStrand7BGLSection(unitFactor, sectionData, materialData, steelGrade, bGLDimensions),
            SectionType.Angle => GetAngleStrand7BGLSection(unitFactor, sectionData, materialData, steelGrade, bGLDimensions),
            SectionType.RectangularHollow => GetRectangularStrand7BGLSection(unitFactor, sectionData, materialData, steelGrade, bGLDimensions),
            SectionType.T => GetTStrand7BGLSection(unitFactor, sectionData, materialData, steelGrade, bGLDimensions),
            _ => new UnknownSection(sectionType, new Material(0, 0, 0)),
        };
    }

    private IorHSection GetIorHStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        var t2 = sectionData[St7.ipT2] * unitFactor.Length;
        var t3 = sectionData[St7.ipT3] * unitFactor.Length;
        return new IorHSection(b1: sectionData[St7.ipD1] * unitFactor.Length,
                               b2: sectionData[St7.ipD2] * unitFactor.Length,
                               d: sectionData[St7.ipD3] * unitFactor.Length,
                               t1: t1,
                               t2: t2,
                               t3: t3,
                               material: GetMaterialProperties(t1, t2, t3, steelGrade, SectionType.IorH, materialData, unitFactor),
                               agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                               ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                               ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                               iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                               iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                               j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                               aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                               aMinor: sectionData[St7.ipSL1] * unitFactor.Length);
    }
    private ChannelSection GetChannelStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        var t2 = sectionData[St7.ipT2] * unitFactor.Length;
        return new ChannelSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                  d: sectionData[St7.ipD2] * unitFactor.Length,
                                  t1: t1,
                                  t2: t2,
                                  material: GetMaterialProperties(t1, t2, 0, steelGrade, SectionType.LipChannel, materialData, unitFactor),
                                  agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                  ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                                  ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                                  iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                  iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                  j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                                  aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                                  aMinor: Math.Abs(sectionData[St7.ipSL1]) * unitFactor.Length);
    }
    private AngleSection GetAngleStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        //Even though Strand7 allows two thicknesses for angle sections, most design codes do not account for this.
        return new AngleSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                d: sectionData[St7.ipD2] * unitFactor.Length,
                                t: t1,
                                material: GetMaterialProperties(t1, 0, 0, steelGrade, SectionType.Angle, materialData, unitFactor),
                                agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                                ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                                iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
    }
    private CircularSection GetCircularStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        return new CircularSection(d: sectionData[St7.ipD1] * unitFactor.Length,
                                   t: t1,
                                   material: GetMaterialProperties(t1, 0, 0, steelGrade, SectionType.CircularHollow, materialData, unitFactor),
                                   agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                   iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                   iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                   j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
    }
    private RectangularSection GetRectangularStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        var t2 = sectionData[St7.ipT2] * unitFactor.Length;
        return new RectangularSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                      d: sectionData[St7.ipD2] * unitFactor.Length,
                                      t1: t1,
                                      t2: t2,
                                      material: GetMaterialProperties(t1, t2, 0, steelGrade, SectionType.RectangularHollow, materialData, unitFactor),
                                      agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                      iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                      iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                      j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
    }
    private TSection GetTStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade)
    {
        //From the collection of South African T-Sections, T sections can have the major axis as x-x (1-1) or y-y (2-2). 2-2 axis alligns with y-y.
        var is22Major = sectionData[St7.ipI22] > sectionData[St7.ipI11];
        var ceMajor = (is22Major ? sectionData[St7.ipXBAR] : sectionData[St7.ipYBAR]) * unitFactor.Length;
        var ceMinor = (is22Major ? sectionData[St7.ipYBAR] : sectionData[St7.ipXBAR]) * unitFactor.Length;
        var iMajor = (is22Major ? sectionData[St7.ipI22] : sectionData[St7.ipI11]) * Math.Pow(unitFactor.Length, 4);
        var iMinor = (is22Major ? sectionData[St7.ipI11] : sectionData[St7.ipI22]) * Math.Pow(unitFactor.Length, 4);
        var t1 = sectionData[St7.ipT1] * unitFactor.Length;
        var t2 = sectionData[St7.ipT2] * unitFactor.Length;

        return new TSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                            d: sectionData[St7.ipD2] * unitFactor.Length,
                            t1: t1,
                            t2: t2,
                            material: GetMaterialProperties(t1, t2, 0, steelGrade, SectionType.T, materialData, unitFactor),
                            agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                            ceMajor: ceMajor,
                            ceMinor: ceMinor,
                            iMajor: iMajor,
                            iMinor: iMinor,
                            j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
    }

    private IorHSection GetIorHStrand7BGLSection(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        var t1 = bGLDimensions[4] * unitFactor.Length;
        var t2 = bGLDimensions[5] * unitFactor.Length;
        var t3 = bGLDimensions[3] * unitFactor.Length;
        return new IorHSection(b1: bGLDimensions[1] * unitFactor.Length,
                               b2: bGLDimensions[2] * unitFactor.Length,
                               d: bGLDimensions[0] * unitFactor.Length,
                               t1: t1,
                               t2: t2,
                               t3: t3,
                               material: GetMaterialProperties(t1, t2, t3, steelGrade, SectionType.IorH, materialData, unitFactor),
                               agr: sectionData[St7.ipBXSArea],
                               ceMajor: sectionData[St7.ipBXSYBar],
                               ceMinor: sectionData[St7.ipBXSXBar],
                               iMajor: sectionData[St7.ipBXSI11],
                               iMinor: sectionData[St7.ipBXSI22],
                               j: sectionData[St7.ipBXSJ],
                               aMajor: sectionData[St7.ipBXSSL2],
                               aMinor: sectionData[St7.ipBXSSL1],
                               zeMajor: sectionData[St7.ipBXSZ11Plus],
                               zeMinor: sectionData[St7.ipBXSZ22Plus],
                               zplMajor: sectionData[St7.ipBXSS11],
                               zplMinor: sectionData[St7.ipBXSS22],
                               cw: sectionData[St7.ipBXSIw]);
    }
    private ChannelSection GetChannelStrand7BGLSection(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        var t1 = bGLDimensions[4] * unitFactor.Length;
        var t2 = bGLDimensions[3] * unitFactor.Length;
        return new ChannelSection(b: bGLDimensions[1] * unitFactor.Length,
                                  d: bGLDimensions[0] * unitFactor.Length,
                                  t1: t1,
                                  t2: t2,
                                  material: GetMaterialProperties(t1, t2, 0, steelGrade, SectionType.LipChannel, materialData, unitFactor),
                                  agr: sectionData[St7.ipBXSArea],
                                  ceMajor: sectionData[St7.ipBXSYBar],
                                  ceMinor: sectionData[St7.ipBXSXBar],
                                  iMajor: sectionData[St7.ipBXSI11],
                                  iMinor: sectionData[St7.ipBXSI22],
                                  j: sectionData[St7.ipBXSJ],
                                  aMajor: sectionData[St7.ipBXSSL2],
                                  aMinor: Math.Abs(sectionData[St7.ipBXSSL1]),
                                  zeMajor: sectionData[St7.ipBXSZ11Plus],
                                  zeMinor: sectionData[St7.ipBXSZ22Plus],
                                  zplMajor: sectionData[St7.ipBXSS11],
                                  zplMinor: sectionData[St7.ipBXSS22],
                                  cw: sectionData[St7.ipBXSIw]);
    }
    private AngleSection GetAngleStrand7BGLSection(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        var t1 = bGLDimensions[2] * unitFactor.Length;
        //Even though Strand7 allows two thicknesses for angle sections, most design codes do not account for this.
        return new AngleSection(b: bGLDimensions[1] * unitFactor.Length,
                                d: bGLDimensions[0] * unitFactor.Length,
                                t: t1,
                                material: GetMaterialProperties(t1, 0, 0, steelGrade, SectionType.Angle, materialData, unitFactor),
                                agr: sectionData[St7.ipBXSArea],
                                ceMajor: sectionData[St7.ipBXSYBar],
                                ceMinor: sectionData[St7.ipBXSXBar],
                                iMajor: sectionData[St7.ipBXSI11],
                                iMinor: sectionData[St7.ipBXSI22],
                                rMajor: sectionData[St7.ipBXSr1],
                                rMinor: sectionData[St7.ipBXSr2],
                                iXX: sectionData[St7.ipBXSIxxL],
                                iYY: sectionData[St7.ipBXSIyyL],
                                iXY: sectionData[St7.ipBXSIxyL],
                                alpha: sectionData[St7.ipBXSAngle] * 180 / Math.PI,
                                j: sectionData[St7.ipBXSJ],
                                zeMajor: sectionData[St7.ipBXSZ11Plus],
                                zeMinor: sectionData[St7.ipBXSZ22Plus],
                                zeXX: sectionData[St7.ipBXSZxxPlus],
                                zeYY: sectionData[St7.ipBXSZyyPlus]);
    }
    private RectangularSection GetRectangularStrand7BGLSection(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        var t1 = bGLDimensions[2] * unitFactor.Length;
        var t2 = bGLDimensions[3] * unitFactor.Length;
        return new RectangularSection(b: bGLDimensions[1] * unitFactor.Length,
                                      d: bGLDimensions[0] * unitFactor.Length,
                                      t1: t1,
                                      t2: t2,
                                      material: GetMaterialProperties(t1, t2, 0, steelGrade, SectionType.RectangularHollow, materialData, unitFactor),
                                      agr: sectionData[St7.ipBXSArea],
                                      iMajor: sectionData[St7.ipBXSI11],
                                      iMinor: sectionData[St7.ipBXSI22],
                                      j: sectionData[St7.ipBXSJ],
                                      zeMajor: sectionData[St7.ipBXSZ11Plus],
                                      zeMinor: sectionData[St7.ipBXSZ22Plus],
                                      zplMajor: sectionData[St7.ipBXSS11],
                                      zplMinor: sectionData[St7.ipBXSS22]);
    }
    private TSection GetTStrand7BGLSection(UnitFactor unitFactor, double[] sectionData, double[] materialData, string steelGrade, double[] bGLDimensions)
    {
        var d = bGLDimensions[0] * unitFactor.Length;
        var b = bGLDimensions[1] * unitFactor.Length;
        var tw = bGLDimensions[2] * unitFactor.Length;
        var tf = bGLDimensions[3] * unitFactor.Length;

        var material = GetMaterialProperties(tf, tw, 0, steelGrade, SectionType.T, materialData, unitFactor);

        // Decide which principal axis is major (1–1 or 2–2)
        var is22Major = sectionData[St7.ipBXSIyyL] > sectionData[St7.ipBXSIxxL];

        var ceMajor = is22Major ? sectionData[St7.ipBXSXBar] : sectionData[St7.ipBXSYBar];
        var ceMinor = is22Major ? sectionData[St7.ipBXSYBar] : sectionData[St7.ipBXSXBar];

        var yeNa = is22Major ? d - ceMinor : d - ceMajor;

        var iMajor = is22Major ? sectionData[St7.ipBXSIyyL] : sectionData[St7.ipBXSIxxL];
        var iMinor = is22Major ? sectionData[St7.ipBXSIxxL] : sectionData[St7.ipBXSIyyL];

        var rMajor = is22Major ? sectionData[St7.ipBXSry] : sectionData[St7.ipBXSrx];
        var rMinor = is22Major ? sectionData[St7.ipBXSrx] : sectionData[St7.ipBXSry];

        var zeMajor = is22Major ? Math.Min(sectionData[St7.ipBXSZyyPlus], sectionData[St7.ipBXSZyyMinus]) : Math.Min(sectionData[St7.ipBXSZxxPlus], sectionData[St7.ipBXSZxxMinus]);
        var zeMinor = is22Major ? Math.Min(sectionData[St7.ipBXSZxxPlus], sectionData[St7.ipBXSZxxMinus]) : Math.Min(sectionData[St7.ipBXSZyyPlus], sectionData[St7.ipBXSZyyMinus]);

        var zplMajor = is22Major ? sectionData[St7.ipBXSSyy] : sectionData[St7.ipBXSSxx];
        var zplMinor = is22Major ? sectionData[St7.ipBXSSxx] : sectionData[St7.ipBXSSyy];

        return new TSection(b: b,
                            d: d,
                            t1: tf,
                            t2: tw,
                            material: material,
                            agr: sectionData[St7.ipBXSArea],
                            yeNa: yeNa,
                            iXX: sectionData[St7.ipBXSIxxL],
                            iYY: sectionData[St7.ipBXSIyyL],
                            ceMajor: ceMajor,
                            ceMinor: ceMinor,
                            iMajor: iMajor,
                            iMinor: iMinor,
                            rMajor: rMajor,
                            rMinor: rMinor,
                            j: sectionData[St7.ipBXSJ],
                            zeMajor: zeMajor,
                            zeMinor: zeMinor,
                            zplMajor: zplMajor,
                            zplMinor: zplMinor,
                            cw: sectionData[St7.ipBXSIw]);
    }

    protected abstract Material GetMaterialProperties(double t1, double t2, double t3, string steelGrade, SectionType sectionType, double[] materialData, UnitFactor unitFactor);
    public abstract void UpdateSectionMaterial(Section section);
}
