using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;
public interface IDataAccessService
{
    public Task<Guid> SaveFemFileByName(string fileName);
    public Task<Guid> GetFemFileIdByName(string fileName);
    public Task SaveBeamSettings(string fileName, IEnumerable<Section> beamProperties);
    public Task SaveDesignSettings(BeamDesignSettings designSettings);
}