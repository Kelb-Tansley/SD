using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface IEffectiveLengthDataService
{
    public Task SaveBeamKValues(string fileName, IEnumerable<BeamKValue> kValues);
    public Task<IEnumerable<BeamKValue>> GetBeamKValuesByFileName(string fileName);
}
