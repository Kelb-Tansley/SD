using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface IBeamPropertiesService
{
    public Section GetBeamSection(string? name, SectionType sectionType, bool canDesign, double[] materialData, double[] sectionData, UnitFactor unitFactor, int i, bool isBGLSection, double[] bGLDimensions);
    public void UpdateSectionMaterial(Section section);
}
