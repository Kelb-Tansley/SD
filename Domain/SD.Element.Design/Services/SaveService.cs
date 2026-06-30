using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class SaveService(IFemModel femModel, IBeamDesignService beamDesignService) : ISaveService
{
    private readonly IFemModel _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
    private readonly IBeamDesignService _beamDesignService = beamDesignService ?? throw new ArgumentNullException(nameof(beamDesignService));

    public async Task SaveAsync(IEnumerable<Beam> beams, IEnumerable<Section> sections)
    {
        var fileName = _femModel.FileName;
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        await _beamDesignService.SetBeamValuesByFileName(fileName, beams, sections);
    }
}