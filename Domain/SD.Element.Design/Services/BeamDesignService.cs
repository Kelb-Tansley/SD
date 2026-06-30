using SD.Core.Shared.Mappers;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class BeamDesignService(IBeamKFactorDataService beamKFactorDataService, ISectionPropertiesDataService sectionPropertiesDataService) : IBeamDesignService
{
    public async Task GetSectionPropertiesByFileName(string fileName, IEnumerable<Section> sections)
    {
        await sectionPropertiesDataService.GetSectionDesignSettingsByFileName(fileName, sections);
    }

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

    public async Task SetBeamValuesByFileName(string fileName, IEnumerable<Beam> beams, IEnumerable<Section> sections)
    {
        if (string.IsNullOrWhiteSpace(fileName) || beams is null)
            return;

        await sectionPropertiesDataService.SaveSectionDesignSettings(fileName, sections);

        var modifiedBeams = beams.Where(b => b.BeamChain.ValuesChanged);
        var kValues = modifiedBeams.Select(b => b.MapToBeamKValue(b.Number));
        if (kValues is null)
            return;

        await beamKFactorDataService.SaveBeamKValues(fileName, kValues!);

        foreach (var b in modifiedBeams)
            b.BeamChain.ValuesChanged = false;
    }
}