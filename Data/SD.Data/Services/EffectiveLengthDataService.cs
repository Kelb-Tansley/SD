using SD.Data.Entities;
using SD.Data.Interfaces;

namespace SD.Data.Services;

public class EffectiveLengthDataService(IUnitOfWork unitOfWork) : IEffectiveLengthDataService
{
    private readonly IRepository<BeamKValueEntity> _beamKValueRepo = unitOfWork.GetRepository<BeamKValueEntity>();
    private readonly IRepository<FemFileEntity> _femFileRepo = unitOfWork.GetRepository<FemFileEntity>();

    public async Task<IEnumerable<BeamKValue>> GetBeamKValuesByFileName(string fileName)
    {
        var result = new List<BeamKValue>();
        if (string.IsNullOrWhiteSpace(fileName))
            return result;

        var file = await _femFileRepo.FirstOrDefault(f => f.FileName == fileName);
        if (file == null)
            return result;

        var all = await _beamKValueRepo.GetAllAsync();
        var found = all.Where(b => b.FemFileStableId == file.StableId);
        foreach (var f in found)
        {
            result.Add(new BeamKValue()
            {
                BeamNumber = f.BeamNumber,
                K1 = f.K1,
                K2 = f.K2,
                Kz = f.Kz,
                KeTop = f.KeTop,
                KeBottom = f.KeBottom
            });
        }

        return result;
    }

    public async Task SaveBeamKValues(string fileName, IEnumerable<SD.Core.Shared.Models.BeamModels.BeamKValue> kValues)
    {
        if (string.IsNullOrWhiteSpace(fileName) || kValues == null)
            return;

        var file = await _femFileRepo.FirstOrDefault(f => f.FileName == fileName);
        if (file == null)
        {
            file = new FemFileEntity() { FileName = fileName };
            await _femFileRepo.AddAsync(file);
            await unitOfWork.Commit();
        }

        var existing = (await _beamKValueRepo.GetAllAsync()).Where(b => b.FemFileStableId == file.StableId).ToList();

        foreach (var kv in kValues)
        {
            var found = existing.FirstOrDefault(e => e.BeamNumber == kv.BeamNumber);
            if (found != null)
            {
                found.K1 = kv.K1;
                found.K2 = kv.K2;
                found.Kz = kv.Kz;
                found.KeTop = kv.KeTop;
                found.KeBottom = kv.KeBottom;
                found.LastUpdated = DateTime.UtcNow;
                await _beamKValueRepo.UpdateAsync(found);
            }
            else
            {
                var entity = new BeamKValueEntity()
                {
                    FemFileStableId = file.StableId,
                    FemFile = file,
                    BeamNumber = kv.BeamNumber,
                    K1 = kv.K1,
                    K2 = kv.K2,
                    Kz = kv.Kz,
                    KeTop = kv.KeTop,
                    KeBottom = kv.KeBottom,
                    LastUpdated = DateTime.UtcNow
                };
                await _beamKValueRepo.AddAsync(entity);
            }
        }

        await unitOfWork.Commit();
    }
}