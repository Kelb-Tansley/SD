using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Core.Shared.Models;
using SD.Core.Strand;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;
public abstract class BeamPropertiesService : IBeamPropertiesService
{
    public Section GetBeamSection(string? name, SectionType sectionType, bool beamPropertyChecked, double[] materialData, double[] sectionData, UnitFactor unitFactor, int i)
    {
        var structural = GetStructuralProperties(sectionType, unitFactor, sectionData, materialData, string.Empty);
        structural.CanDesign = beamPropertyChecked;
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
            _ => throw new NotImplementedException(),
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
                                j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                                aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                                aMinor: (sectionData[St7.ipXBAR] - sectionData[St7.ipSL1]) * unitFactor.Length);
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

    protected abstract Material GetMaterialProperties(double t1, double t2, double t3, string steelGrade, SectionType sectionType, double[] materialData, UnitFactor unitFactor);
    public abstract void UpdateSectionMaterial(Section section);
}
