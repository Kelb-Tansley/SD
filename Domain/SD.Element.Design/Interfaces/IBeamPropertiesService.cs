using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;
public interface IBeamPropertiesService
{
    public Section GetBeamSection(string? name, SectionType sectionType, bool beamPropertyChecked, double[] materialData, double[] sectionData, UnitFactor unitFactor, int i);
    public void UpdateSectionMaterial(Section section);
}
