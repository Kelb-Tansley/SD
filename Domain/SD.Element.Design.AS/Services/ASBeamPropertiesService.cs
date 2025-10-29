using SD.Element.Design.Services;
using System.Collections.ObjectModel;

namespace SD.Element.Design.AS.Services;
public class ASBeamPropertiesService : BeamPropertiesService
{
    public override void UpdateSectionMaterial(Section section)
    {
        var element1Grade = GradeByType(section.T1, section.Material.SteelGrade, section.SectionType);
        var element2Grade = GradeByType(section.T2, section.Material.SteelGrade, section.SectionType);
        var element3Grade = GradeByType(section.T3, section.Material.SteelGrade, section.SectionType);

        section.Material.FyElement1 = element1Grade.Yield;
        section.Material.FuElement1 = element1Grade.Tensile;
        section.Material.FyElement2 = element2Grade.Yield;
        section.Material.FuElement2 = element2Grade.Tensile;
        section.Material.FyElement3 = element3Grade.Yield;
        section.Material.FuElement3 = element3Grade.Tensile;
        section.Material.SetMinFy();
    }
    protected override Material GetMaterialProperties(double t1, double t2, double t3, string steelGrade, SectionType sectionType, double[] materialData, UnitFactor unitFactor)
    {
        var grades = AvailableGradesByType(sectionType);
        var element1Grade = GradeByType(t1, grades.First(), sectionType);
        var element2Grade = GradeByType(t2, grades.First(), sectionType);
        var element3Grade = GradeByType(t3, grades.First(), sectionType);

        return new Material(
            fyElement1: element1Grade.Yield,
            fyElement2: element2Grade.Yield,
            fyElement3: element3Grade.Yield
            )
        {
            FuElement1 = element1Grade.Tensile,
            FuElement2 = element2Grade.Tensile,
            FuElement3 = element3Grade.Tensile,
            Es = materialData[St7.ipModulus] * unitFactor.Stress,
            Gs = Math.Max(materialData[St7.ipModulus] / (2 * (1 + materialData[St7.ipPoisson])) * unitFactor.Stress, materialData[St7.ipShearModulus] * unitFactor.Stress),
            AvailableSteelGrades = new ObservableCollection<string>(grades),
            SteelGrade = grades.First(),
            Density = materialData[St7.ipDensity] / Math.Pow(unitFactor.Length, 2),
        };
    }

    private List<string> AvailableGradesByType(SectionType sectionType)
    {
        return sectionType switch
        {
            SectionType.IorH or SectionType.LipChannel or SectionType.Angle or SectionType.T => ["300", "350"],
            SectionType.CircularHollow or SectionType.RectangularHollow => ["C250", "C300", "C350"],
            _ => throw new NotImplementedException($"{nameof(GetMaterialProperties)} with value {nameof(sectionType)} not implemented"),
        };
    }

    private SteelStrength GradeByType(double t, string steelGrade, SectionType sectionType)
    {
        return sectionType switch
        {
            SectionType.IorH or SectionType.LipChannel or SectionType.Angle or SectionType.T => SectionGrade(steelGrade, t),
            SectionType.CircularHollow or SectionType.RectangularHollow => HollowSectionGrade(steelGrade, t),
            _ => throw new NotImplementedException($"{nameof(GetMaterialProperties)} with value {nameof(sectionType)} not implemented"),
        };
    }

    private SteelStrength SectionGrade(string steelGrade, double t)
    {
        if (steelGrade == "350")
        {
            if (t <= 11)
                return new SteelStrength(360, 480);
            else if (t > 11 && t < 40)
                return new SteelStrength(340, 480);
            else if (t >= 40)
                return new SteelStrength(330, 480);
        }
        else if (steelGrade == "300")
        {
            if (t < 11)
                return new SteelStrength(320, 440);
            else if (t >= 11 && t <= 17)
                return new SteelStrength(300, 440);
            else if (t > 17)
                return new SteelStrength(280, 440);
        }
        throw new NotImplementedException($"{nameof(SectionGrade)} with value {nameof(steelGrade)} not implemented");
    }
    private SteelStrength HollowSectionGrade(string steelGrade, double t)
    {
        if (steelGrade == "C250") return new SteelStrength(450, 500);
        else if (steelGrade == "C350") return new SteelStrength(350, 430);
        else if (steelGrade == "C250") return new SteelStrength(250, 320);

        throw new NotImplementedException($"{nameof(SectionGrade)} with value {nameof(steelGrade)} not implemented");
    }
}

public class SteelStrength(double yield, double tensile)
{
    public double Yield { get; set; } = yield;
    public double Tensile { get; set; } = tensile;
}
