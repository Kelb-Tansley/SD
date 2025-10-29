using SD.Core.Shared.Models.BeamModels.Sections;

namespace SD.Fem.Strand7.Helpers
{
    public static class BeamHelper
    {
        public static Section GetBeamPropertyFromStructural(string? name, int strand7SectionType, bool beamPropertyChecked, double[] materialData, double[] sectionData, UnitFactor unitFactor, int i)
        {
            var sectionType = SectionTypeHelper.SectionTypeFromStrand(strand7SectionType);
            var structural = GetStructuralProperties(sectionType, unitFactor, sectionData, materialData);
            structural.CanDesign = beamPropertyChecked;
            structural.Number = i;
            structural.Name = name ?? "NoDesign property name";

            return structural;
        }
        /// <summary>
        /// Beam section properties interpretted from Strand7 API. Read from table on page 1004 of API documentation. 
        /// Strand7 provides 6 properties which define all dimensions:  D1 D2 D3 T1 T2 T3.
        /// </summary>
        private static Section GetStructuralProperties(SectionType sectionType, UnitFactor unitFactor, double[] sectionData, double[] materialData)
        {
            return sectionType switch
            {
                SectionType.IorH => GetIorHStrand7Section(unitFactor, sectionData, materialData),
                SectionType.LipChannel => GetChannelStrand7Section(unitFactor, sectionData, materialData),
                SectionType.Angle => GetAngleStrand7Section(unitFactor, sectionData, materialData),
                SectionType.CircularHollow => GetCircularStrand7Section(unitFactor, sectionData, materialData),
                SectionType.RectangularHollow => GetRectangularStrand7Section(unitFactor, sectionData, materialData),
                SectionType.T => GetTStrand7Section(unitFactor, sectionData, materialData),
                SectionType.Unknown => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
        private static IorHSection GetIorHStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
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
                                   material: GetMaterialProperties(t1, t2, t3, materialData, unitFactor),
                                   agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                   ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                                   ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                                   iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                   iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                   j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                                   aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                                   aMinor: sectionData[St7.ipSL1] * unitFactor.Length);
        }
        private static ChannelSection GetChannelStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
        {
            var t1 = sectionData[St7.ipT1] * unitFactor.Length;
            var t2 = sectionData[St7.ipT2] * unitFactor.Length;
            return new ChannelSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                      d: sectionData[St7.ipD2] * unitFactor.Length,
                                      t1: t1,
                                      t2: t2,
                                      material: GetMaterialProperties(t1, t2, 0, materialData, unitFactor),
                                      agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                      ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                                      ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                                      iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                      iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                      j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                                      aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                                      aMinor: Math.Abs(sectionData[St7.ipSL1]) * unitFactor.Length);
        }
        private static AngleSection GetAngleStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
        {
            var t1 = sectionData[St7.ipT1] * unitFactor.Length;
            //Even though Strand7 allows two thicknesses for angle sections, most design codes do not account for this.
            return new AngleSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                    d: sectionData[St7.ipD2] * unitFactor.Length,
                                    t: t1,
                                    material: GetMaterialProperties(t1, 0, 0, materialData, unitFactor),
                                    agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                    ceMajor: sectionData[St7.ipYBAR] * unitFactor.Length,
                                    ceMinor: sectionData[St7.ipXBAR] * unitFactor.Length,
                                    iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                    iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                    j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4),
                                    aMajor: sectionData[St7.ipSL2] * unitFactor.Length,
                                    aMinor: (sectionData[St7.ipXBAR] - sectionData[St7.ipSL1]) * unitFactor.Length);
        }
        private static CircularSection GetCircularStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
        {
            var t1 = sectionData[St7.ipT1] * unitFactor.Length;
            return new CircularSection(d: sectionData[St7.ipD1] * unitFactor.Length,
                                       t: t1,
                                       material: GetMaterialProperties(t1, 0, 0, materialData, unitFactor),
                                       agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                       iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                       iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                       j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
        }
        private static RectangularSection GetRectangularStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
        {
            var t1 = sectionData[St7.ipT1] * unitFactor.Length;
            var t2 = sectionData[St7.ipT2] * unitFactor.Length;
            return new RectangularSection(b: sectionData[St7.ipD1] * unitFactor.Length,
                                          d: sectionData[St7.ipD2] * unitFactor.Length,
                                          t1: t1,
                                          t2: t2,
                                          material: GetMaterialProperties(t1, t2, 0, materialData, unitFactor),
                                          agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                          iMajor: sectionData[St7.ipI11] * Math.Pow(unitFactor.Length, 4),
                                          iMinor: sectionData[St7.ipI22] * Math.Pow(unitFactor.Length, 4),
                                          j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
        }
        private static TSection GetTStrand7Section(UnitFactor unitFactor, double[] sectionData, double[] materialData)
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
                                material: GetMaterialProperties(t1, t2, 0, materialData, unitFactor),
                                agr: sectionData[St7.ipAREA] * unitFactor.Length * unitFactor.Length,
                                ceMajor: ceMajor,
                                ceMinor: ceMinor,
                                iMajor: iMajor,
                                iMinor: iMinor,
                                j: sectionData[St7.ipJ] * Math.Pow(unitFactor.Length, 4));
        }
        private static Material GetMaterialProperties(double t1, double t2, double t3, double[] materialData, UnitFactor unitFactor)
            => new(fyElement1: t1 > 16 ? 345D : 350D, fyElement2: t2 > 16 ? 345D : 350D, fyElement3: t3 > 16 ? 345D : 350D)
            {
                Es = materialData[St7.ipModulus] * unitFactor.Stress,
                Gs = Math.Max(materialData[St7.ipModulus] / (2 * (1 + materialData[St7.ipPoisson])) * unitFactor.Stress, materialData[St7.ipShearModulus] * unitFactor.Stress)
            };

    }
}