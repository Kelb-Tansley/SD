using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.Element.Design.Services;

public class SaveService : ISaveService, IDisposable
{
    private readonly IFemModel _femModel;
    private readonly IBeamDesignService _beamDesignService;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private bool _disposed;

    public SaveService(IFemModel femModel, IBeamDesignService beamDesignService)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _beamDesignService = beamDesignService ?? throw new ArgumentNullException(nameof(beamDesignService));
    }

    public async Task SaveAsync(IEnumerable<Beam> beams, IEnumerable<Section> sections)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var fileName = _femModel.FileName;
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        await _saveLock.WaitAsync();
        try
        {
            await _beamDesignService.SetBeamValuesByFileName(fileName, beams, sections);
        }
        finally
        {
            _saveLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _saveLock.Dispose();
        _disposed = true;
    }
}