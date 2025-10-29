using SD.Core.Shared.Models.BeamModels.Sections;

namespace SD.Element.Design.Sans.Engine;
public class ClassificationService : SansService
{

    public static double GetBreadthEffective(Section section)
    {
        return section.SectionType switch
        {
            SectionType.IorH => Math.Min(200D * section.T1 / Math.Sqrt(section.Material.FyElement1), section.T1 * 60) * 2,
            SectionType.LipChannel => Math.Min(200D * section.T1 / Math.Sqrt(section.Material.FyElement1), section.T1 * 60),
            _ => 0,
        };
    }
    /// <summary>
    /// Classify the elements which makes up the beam section that is subject to axial compression. See table 3 of SANS 10162-1
    /// </summary>
    public static SectionClassification ClassifyAxialCompression(Section section)
    {
        switch (section.SectionType)
        {
            case SectionType.IorH:
                {
                    var sansSection = section as IorHSection ?? throw new ArgumentNullException(nameof(section));
                    var bottomflangeClass = sansSection.B1 / 2 / sansSection.T1 <= 200D / Math.Sqrt(sansSection.Material.FyElement1) ? ElementClass.Class3 : ElementClass.Class4;
                    var topflangeClass = sansSection.B2 / 2 / sansSection.T2 <= 200D / Math.Sqrt(sansSection.Material.FyElement2) ? ElementClass.Class3 : ElementClass.Class4;
                    var webClass = sansSection.Hw / sansSection.T3 <= 670D / Math.Sqrt(sansSection.Material.FyElement3) ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(topflangeClass, bottomflangeClass, webClass);
                }
            case SectionType.LipChannel:
                {
                    var sansSection = section as ChannelSection ?? throw new ArgumentNullException(nameof(section));
                    var flangeClass = sansSection.B1 / sansSection.T1 <= 200D / Math.Sqrt(sansSection.Material.FyElement1) ? ElementClass.Class3 : ElementClass.Class4;
                    var webClass = sansSection.Hw / sansSection.T2 <= 670D / Math.Sqrt(sansSection.Material.FyElement2) ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(flangeClass, null, webClass);
                }
            case SectionType.Angle:
                {
                    var xLegClass = section.B1 / section.T1 <= 200D / Math.Sqrt(section.Material.FyElement1) ? ElementClass.Class3 : ElementClass.Class4;
                    var yLegClass = section.D / section.T1 <= 200D / Math.Sqrt(section.Material.FyElement2) ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(xLegClass, yLegClass);
                }
            case SectionType.CircularHollow:
                {
                    var shellClass = section.D / section.T1 <= 23000D / section.Material.FyElement1 ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(shellClass);
                }
            case SectionType.RectangularHollow:
                {
                    var flangeClass = (section.B1 - 4 * section.T1) / section.T1 <= 670D / Math.Sqrt(section.Material.FyElement1) ? ElementClass.Class3 : ElementClass.Class4;
                    var webClass = (section.D - 4 * section.T2) / section.T2 <= 670D / Math.Sqrt(section.Material.FyElement2) ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(flangeClass, webClass);
                }
            case SectionType.T:
                {
                    var flangeClass = section.B1 / 2 / section.T1 <= 200D / Math.Sqrt(section.Material.FyElement1) ? ElementClass.Class3 : ElementClass.Class4;
                    var stemClass = section.D / section.T2 <= 340D / Math.Sqrt(section.Material.FyElement2) ? ElementClass.Class3 : ElementClass.Class4;
                    return new SectionClassification(flangeClass, stemClass);
                }
        }
        throw new NotImplementedException(nameof(ClassifyAxialCompression));
    }

    /// <summary>
    /// Calculates the flexural class of the sans section, which is a function of the axial compressive force in the section.
    /// This is calculated for every load case combination and beam with flexural loads.
    /// Classify the elements which makes up the beam section that is subject to axial compression. See table 3 of SANS 10162-1
    /// </summary>
    public static SectionClassification ClassifyFlexuralCompression(Section section, double cu)
    {
        switch (section.SectionType)
        {
            case SectionType.IorH:
                {
                    var sansSection = section as IorHSection ?? throw new ArgumentNullException(nameof(section));
                    var bottomflangeClass = GetElementFlexuralClass(sansSection.B1 / 2, sansSection.T1, sansSection.Material.FyElement1, 145D, 170D, 200D);
                    var topflangeClass = GetElementFlexuralClass(sansSection.B2 / 2, sansSection.T2, sansSection.Material.FyElement2, 145D, 170D, 200D);
                    var webClass = GetWebsFlexuralClass(sansSection.Hw, sansSection.T3, sansSection.Material.FyElement3, cu, sansSection.Agr * sansSection.Material.MinFy);
                    return new SectionClassification(topflangeClass, bottomflangeClass, webClass);
                }
            case SectionType.LipChannel:
                {
                    var sansSection = section as ChannelSection ?? throw new ArgumentNullException(nameof(section));
                    var flangeClass = GetElementFlexuralClass(sansSection.B1, sansSection.T1, sansSection.Material.FyElement1, 145D, 170D, 200D);
                    var webClass = GetWebsFlexuralClass(sansSection.Hw, sansSection.T2, sansSection.Material.FyElement2, cu, sansSection.Agr * sansSection.Material.MinFy);
                    return new SectionClassification(flangeClass, null, webClass);
                }
            case SectionType.Angle:
                {
                    var xLegClass = GetElementFlexuralClass(section.B1, section.T1, section.Material.FyElement1, 145D, 170D, 200D);
                    var yLegClass = GetElementFlexuralClass(section.D, section.T2, section.Material.FyElement2, 145D, 170D, 200D);
                    return new SectionClassification(xLegClass, yLegClass);
                }
            case SectionType.CircularHollow:
                {
                    var shellClass = GetElementFlexuralClass(section.D, section.T1, Math.Pow(section.Material.FyElement1, 2), 13000D, 18000D, 66000D);
                    return new SectionClassification(shellClass);
                }
            case SectionType.RectangularHollow:
                {
                    if (section.B1 == section.D)
                    {
                        var elementClass = GetElementFlexuralClass(section.B1 - 4 * section.T1, section.T1, section.Material.FyElement1, 525D, 525D, 670D);
                        return new SectionClassification(elementClass, elementClass);
                    }

                    var flangeClass = GetElementFlexuralClass(section.B1 - 4 * section.T1, section.T1, section.Material.FyElement1, 420D, 525D, 670D);
                    var webClass = GetElementFlexuralClass(section.D - 4 * section.T2, section.T2, section.Material.FyElement2, 420D, 525D, 670D);
                    return new SectionClassification(flangeClass, webClass);
                }
            case SectionType.T:
                {
                    var flangeClass = GetElementFlexuralClass(section.B1 / 2, section.T1, section.Material.FyElement1, 145D, 170D, 200D);
                    var stemClass = GetElementFlexuralClass(section.D, section.T2, section.Material.FyElement2, 145D, 170D, 340D);
                    return new SectionClassification(flangeClass, stemClass);
                }
        }
        throw new NotImplementedException(nameof(ClassifyFlexuralCompression));
    }

    private static ElementClass GetElementFlexuralClass(double b, double t, double fy, double class1, double class2, double class3)
    {
        var factor = (b / t) * Math.Sqrt(fy);
        return factor <= class1 ? ElementClass.Class1 : factor <= class2 ? ElementClass.Class2 : factor <= class3 ? ElementClass.Class3 : ElementClass.Class4;
    }

    private static ElementClass GetWebsFlexuralClass(double b, double t, double fy, double cu, double cy)
    {
        if (b / t <= 1100 / Math.Sqrt(fy) * (1 - 0.39 * cu / (Φ * cy)))
            return ElementClass.Class1;
        else if (b / t <= 1700 / Math.Sqrt(fy) * (1 - 0.61 * cu / (Φ * cy)))
            return ElementClass.Class2;
        else if (b / t <= 1900 / Math.Sqrt(fy) * (1 - 0.65 * cu / (Φ * cy)))
            return ElementClass.Class3;
        else return ElementClass.Class4;
    }
}
