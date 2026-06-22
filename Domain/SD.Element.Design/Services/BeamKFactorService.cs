using SD.Core.Shared.Mappers;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class BeamKFactorService(IBeamKFactorDataService beamKFactorDataService) : IBeamKFactorService
{
    public async Task GetBeamKValuesByFileName(string fileName, IEnumerable<Beam> beams)
    {
        var kvalues = await beamKFactorDataService.GetBeamKValuesByFileName(fileName);
        if (kvalues is null)
            return;

        foreach (var kvalue in kvalues)
        {
            var beam = beams.FirstOrDefault(b => b.Number == kvalue.BeamNumber);
            if (beam is null)
                continue;

            beam.BeamChain.K1 = kvalue.K1;
            beam.BeamChain.K2 = kvalue.K2;
            beam.BeamChain.Kz = kvalue.Kz;
            beam.BeamChain.KeTop = kvalue.KeTop;
            beam.BeamChain.KeBottom = kvalue.KeBottom;
        }
    }

    public async Task SetBeamKValuesByFileName(string fileName, IEnumerable<Beam> beams)
    {
        if (string.IsNullOrWhiteSpace(fileName) || beams is null)
            return;

        var modifiedBeams = beams.Where(b => b.BeamChain.ValuesChanged);
        var kValues = modifiedBeams.Select(b => b.MapToBeamKValue(b.Number));
        if (kValues is null)
            return;

        await beamKFactorDataService.SaveBeamKValues(fileName, kValues);

        foreach (var b in modifiedBeams)
            b.BeamChain.ValuesChanged = false;
    }
}