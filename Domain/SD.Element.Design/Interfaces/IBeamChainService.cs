using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface IBeamChainService
{
    public void GenerateBeamChains(List<Beam> beams);
}
