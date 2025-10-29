using SD.Data.Entities;
using SD.Data.Interfaces;

namespace SD.Data.Mapping;
public class BeamPropertySettingsMapper : IEntityMapper<BeamPropertySettings, Section>
{
    public BeamPropertySettings Map(Section prop)
    {
        return new BeamPropertySettings()
        {
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            PropertyNumber = prop.Number,
            FileName = string.Empty,
            IsLateralRestraint = prop.IsLateralRestraint
        };
    }

    public IEnumerable<BeamPropertySettings> MapAll(IEnumerable<Section> properties)
    {
        var beamPropertySettings = new List<BeamPropertySettings>();
        foreach (var prop in properties)
            beamPropertySettings.Add(Map(prop));
        return beamPropertySettings;
    }
}
