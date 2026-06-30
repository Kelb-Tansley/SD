using SD.Data.Entities;

namespace SD.Data.Mapping;

public static class SectionSettingsEntityMapper
{
    public static void MapToSectionSetting(this Section section, SectionDesignSetting setting)
    {
        section.IsBottomFlangeRestraint = setting.IsBottomFlangeRestraint;
        section.IsLateralRestraint = setting.IsLateralRestraint;
        section.IsPlateGirder = setting.IsPlateGirder;
        section.IsBracedFrame = setting.IsBracedFrame;
        section.IsTopFlangeRestraint = setting.IsTopFlangeRestraint;
        section.IsTorsionalRestraint = setting.IsTorsionalRestraint;
        section.WebStiffenerSpacing = setting.WebStiffenerSpacing;
        section.NetAreaFactor = setting.NetAreaFactor;
    }

    public static void UpdateProperties(this SectionDesignSetting setting, Section section)
    {
        setting.IsBottomFlangeRestraint = section.IsBottomFlangeRestraint;
        setting.IsLateralRestraint = section.IsLateralRestraint;
        setting.IsPlateGirder = section.IsPlateGirder;
        setting.IsBracedFrame = section.IsBracedFrame;
        setting.IsTopFlangeRestraint = section.IsTopFlangeRestraint;
        setting.IsTorsionalRestraint = section.IsTorsionalRestraint;
        setting.WebStiffenerSpacing = section.WebStiffenerSpacing;
        setting.NetAreaFactor = section.NetAreaFactor;
    }

    public static SectionDesignSetting MapToSectionDesignSettingEntity(this Section section, Guid value)
    {
        return new SectionDesignSetting
        {
            FemFileStableId = value,
            PropertyNumber = section.Number,
            IsBottomFlangeRestraint = section.IsBottomFlangeRestraint,
            IsLateralRestraint = section.IsLateralRestraint,
            IsPlateGirder = section.IsPlateGirder,
            IsBracedFrame = section.IsBracedFrame,
            IsTopFlangeRestraint = section.IsTopFlangeRestraint,
            IsTorsionalRestraint = section.IsTorsionalRestraint,
            NetAreaFactor = section.NetAreaFactor,
            WebStiffenerSpacing= section.WebStiffenerSpacing
        };
    }
}