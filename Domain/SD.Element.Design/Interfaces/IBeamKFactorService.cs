using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface IBeamKFactorService
{
    public Task GetBeamKValuesByFileName(string fileName, IEnumerable<Beam> beams);
    public Task SetBeamKValuesByFileName(string fileName, IEnumerable<Beam> beams);
}