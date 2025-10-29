using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;
public interface IDataAccessService
{
    public Task SaveBeamSettings(string fileName, IEnumerable<Section> beamProperties);
    public Task SaveDesignSettings(BeamDesignSettings designSettings);
}