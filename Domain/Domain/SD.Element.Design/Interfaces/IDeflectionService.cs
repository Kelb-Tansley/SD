using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;
public interface IDeflectionService
{
    public Task<List<DeflectionResult>> GetDeflectionResults(int modelId, IEnumerable<LoadCaseCombination> loadCaseCombinations, IEnumerable<Beam> beams, DeflectionAxis deflectionAxis, DeflectionMethod deflectionMethod);
}
