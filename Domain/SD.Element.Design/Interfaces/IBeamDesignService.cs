using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface IBeamDesignService
{
    public Task GetSectionPropertiesByFileName(string fileName, IEnumerable<Section> sections);
    public Task GetBeamKValuesByFileName(string fileName, IEnumerable<Beam> beams);
    public Task SetBeamValuesByFileName(string fileName, IEnumerable<Beam> beams, IEnumerable<Section> sections);
}