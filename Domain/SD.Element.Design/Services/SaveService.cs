using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class SaveService : ISaveService
{
    private readonly IFemModel _femModel;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IEffectiveLengthDataService _effectiveLengthDataService;

    public SaveService(IFemModel femModel,
                       IUlsDesignResults ulsDesignResults,
                       IEffectiveLengthDataService effectiveLengthDataService)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _effectiveLengthDataService = effectiveLengthDataService ?? throw new ArgumentNullException(nameof(effectiveLengthDataService));
    }

    public async Task SaveAsync()
    {
        var fileName = _femModel.FileName;
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var dirtyResults = _ulsDesignResults.GetUlsResults()
            ?.Where(r => r.Beam.BeamChain.ValuesChanged)
            .ToList();

        if (dirtyResults is not { Count: > 0 })
            return;

        var kValues = dirtyResults.Select(r => new BeamKValue
        {
            BeamNumber = r.Beam.Number,
            K1 = r.Beam.BeamChain.K1,
            K2 = r.Beam.BeamChain.K2,
            Kz = r.Beam.BeamChain.Kz,
            KeTop = r.Beam.BeamChain.KeTop,
            KeBottom = r.Beam.BeamChain.KeBottom
        }).ToList();

        await _effectiveLengthDataService.SaveBeamKValues(fileName, kValues);

        foreach (var r in dirtyResults)
            r.Beam.BeamChain.ValuesChanged = false;
    }
}
