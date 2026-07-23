using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models.BeamModels.Sections;

public class UnknownSection(SectionType sectionType, Material material) : Section(sectionType, material)
{
    public override double GetSectionDepth() => D;
    public override double GetSectionBreadth() => B1;
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
