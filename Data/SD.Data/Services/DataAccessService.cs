using SD.Data.Entities;
using SD.Data.Interfaces;

namespace SD.Data.Services;

public class DataAccessService(
    IUnitOfWork unitOfWork,
    IEntityMapper<BeamPropertySettings, Section> beamPropertiesMapper,
    IEntityMapper<DesignSettings, BeamDesignSettings> designSettingsMapper) : IDataAccessService
{
    private readonly IRepository<FemFileEntity> _femFileRepo = unitOfWork.GetRepository<FemFileEntity>();
    private readonly IRepository<DesignSettings> _designSettingsRepo = unitOfWork.GetRepository<DesignSettings>();
    private readonly IRepository<BeamPropertySettings> _beamPropertiesRepo = unitOfWork.GetRepository<BeamPropertySettings>();

    private readonly IEntityMapper<DesignSettings, BeamDesignSettings> _designSettingsMapper = designSettingsMapper ?? throw new ArgumentNullException(nameof(designSettingsMapper));
    private readonly IEntityMapper<BeamPropertySettings, Section> _beamPropertiesMapper = beamPropertiesMapper ?? throw new ArgumentNullException(nameof(beamPropertiesMapper));

    public async Task<Guid> SaveFemFileByName(string fileName)
    {
        // First check that file does not already exist
        var existingFile = await GetFemFileIdByName(fileName);
        if (existingFile != Guid.Empty)
            return existingFile;

        var femFile = new FemFileEntity() { FileName = fileName };
        await _femFileRepo.AddAsync(femFile);

        await unitOfWork.Commit();
        return femFile.Id;
    }

    public async Task<Guid> GetFemFileIdByName(string fileName)
    {
        return (await _femFileRepo.FirstOrDefault(f => f.FileName.Equals(fileName)))?.Id ?? Guid.Empty;
    }

    public async Task SaveBeamSettings(string fileName, IEnumerable<Section> beamProperties)
    {
        var settings = _beamPropertiesMapper.MapAll(beamProperties);
        foreach (var setting in settings)
            setting.FemFile.FileName = fileName;
        await _beamPropertiesRepo.AddAllAsync(settings);
        await unitOfWork.Commit();
    }

    public async Task SaveDesignSettings(BeamDesignSettings designSettings)
    {
        var settings = _designSettingsMapper.Map(designSettings);

        await _designSettingsRepo.AddAsync(settings);
        await unitOfWork.Commit();
    }

    public async Task<DesignSettings?> GetDesignSettings()
    {
        return await _designSettingsRepo.GetByIdAsync(Guid.NewGuid());
    }
}