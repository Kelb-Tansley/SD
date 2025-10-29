using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;
public interface IElementDesignService
{
    public Task<IEnumerable<UlsResultPeak>> RunUlsDesign(int modelId, List<Beam>? beams);
    public Task RunUlsDesignUpdate(int modelId, List<Beam> beams);
}