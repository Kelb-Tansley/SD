using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models.BeamModels.Sections;

public class UnknownSection(SectionType sectionType, Material material) : Section(sectionType, material)
{
    protected override string GetDisplayName()
    {
        return "Unknown section type.";
    }

    protected override string GetTypeDisplay()
    {
        return "Unknown";
    }

    protected override void SetDefaultRestraints() { }
}
