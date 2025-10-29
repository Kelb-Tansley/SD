using SD.Data.Interfaces;
using SD.Data.Entities;

namespace SD.Data.Mapping;
public class DesignSettingsMapper : IEntityMapper<DesignSettings, BeamDesignSettings>
{
    public DesignSettings Map(BeamDesignSettings settings)
    {
        return new DesignSettings()
        {
            BeamAllignmentAngleTolerance = settings.BeamAllignmentAngleTolerance,
            BeamRestraintAngleTolerance = settings.BeamRestraintAngleTolerance,
            BeamRotationAngleTolerance = settings.BeamRotationAngleTolerance,
            BeamMinStations = settings.BeamMinStations,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
        };
    }

    public IEnumerable<DesignSettings> MapAll(IEnumerable<BeamDesignSettings> fromModel)
    {
        throw new NotImplementedException();
    }
}
