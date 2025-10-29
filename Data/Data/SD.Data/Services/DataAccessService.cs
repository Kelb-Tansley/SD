using SD.Data.Entities;
using SD.Data.Interfaces;

namespace SD.Data.Services;
public class DataAccessService : IDataAccessService
{
    private readonly IRepository<DesignSettings> _designSettingsRepo;
    private readonly IRepository<BeamPropertySettings> _beamPropertiesRepo;

    private readonly IEntityMapper<DesignSettings, BeamDesignSettings> _designSettingsMapper;
    private readonly IEntityMapper<BeamPropertySettings, Section> _beamPropertiesMapper;

    public DataAccessService(
        IUnitOfWork unitOfWork,
        IEntityMapper<BeamPropertySettings, Section> beamPropertiesMapper,
        IEntityMapper<DesignSettings, BeamDesignSettings> designSettingsMapper)
    {
        _ = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _designSettingsRepo = unitOfWork.GetRepository<DesignSettings>();
        _beamPropertiesRepo = unitOfWork.GetRepository<BeamPropertySettings>();

        _designSettingsMapper = designSettingsMapper ?? throw new ArgumentNullException(nameof(designSettingsMapper));
        _beamPropertiesMapper = beamPropertiesMapper ?? throw new ArgumentNullException(nameof(beamPropertiesMapper));
    }
    public async Task SaveBeamSettings(string fileName, IEnumerable<Section> beamProperties)
    {
        var settings = _beamPropertiesMapper.MapAll(beamProperties);
        foreach (var setting in settings)
            setting.FileName = fileName;

        await _beamPropertiesRepo.AddAllAsync(settings);
    }
    public async Task SaveDesignSettings(BeamDesignSettings designSettings)
    {
        var settings = _designSettingsMapper.Map(designSettings);

        await _designSettingsRepo.AddAsync(settings);
    }
    public async Task<DesignSettings> GetDesignSettings()
    {
        return await _designSettingsRepo.GetByIdAsync(0);
    }
}
