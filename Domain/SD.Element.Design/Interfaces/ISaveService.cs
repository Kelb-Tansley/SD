using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface ISaveService
{
    public Task SaveAsync(IEnumerable<Beam> beams, IEnumerable<Section> sections);
}
