using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class SaveService : ISaveService
{
    private readonly IFemModel _femModel;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IBeamKFactorService _beamKFactorService;

    public SaveService(IFemModel femModel,
                       IUlsDesignResults ulsDesignResults,
                       IBeamKFactorService beamKFactorService)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _beamKFactorService = beamKFactorService ?? throw new ArgumentNullException(nameof(beamKFactorService));
    }

    public async Task SaveAsync()
    {
        var fileName = _femModel.FileName;
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var beams = _ulsDesignResults.GetUlsResults()?.Select(r => r.Beam).ToList();

        await _beamKFactorService.SetBeamKValuesByFileName(fileName, beams);
    }
}
