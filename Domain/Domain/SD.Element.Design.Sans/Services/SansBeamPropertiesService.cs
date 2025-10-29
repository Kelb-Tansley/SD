using SD.Element.Design.Services;

namespace SD.Element.Design.Sans.Services;
public class SansBeamPropertiesService : BeamPropertiesService
{
    public override void UpdateSectionMaterial(Section section)
    {
        return;
    }
    protected override Material GetMaterialProperties(double t1, double t2, double t3, string steelGrade, SectionType sectionType, double[] materialData, UnitFactor unitFactor)
        => new(fyElement1: t1 > 16 ? 345D : 350D, fyElement2: t2 > 16 ? 345D : 350D, fyElement3: t3 > 16 ? 345D : 350D)
        {
            Es = materialData[St7.ipModulus] * unitFactor.Stress,
            Gs = Math.Max(materialData[St7.ipModulus] / (2 * (1 + materialData[St7.ipPoisson])) * unitFactor.Stress, materialData[St7.ipShearModulus] * unitFactor.Stress),
            FuElement1 = 450D,
            Density = materialData[St7.ipDensity] / Math.Pow(unitFactor.Length, 2),
        };
}
