using SD.Core.Infrastructure.Interfaces;
using SD.Core.Infrastructure.Logging;
using SD.Core.Shared.Enum;
using SD.Data.Entities;
using SD.Data.Interfaces;
using SD.Data.Mapping;
using System.Windows;

namespace SD.Data.Services;

public class BeamKFactorDataService(IUnitOfWork unitOfWork, IFemFilePathDataService femFilePathDataService, INotificationService notificationService, ILoggerService logger) : IBeamKFactorDataService
{
    private readonly IRepository<BeamKValueEntity> _beamKValueRepo = unitOfWork.GetRepository<BeamKValueEntity>();

    private readonly IFemFilePathDataService _femFilePathDataService = femFilePathDataService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILoggerService _logger = logger;

    public async Task<IEnumerable<BeamKValue>> GetBeamKValuesByFileName(string fileName)
    {
        var result = new List<BeamKValue>();
        if (string.IsNullOrWhiteSpace(fileName))
            return result;

        try
        {
            // The file may have been moved, so it cannot be found by the full path.
            var file = await _femFilePathDataService.GetFileByName(fileName);
            if (file is null)
                return result;

            if (!file.ExactMatchFound)
            {
                var yesNoResult = _notificationService.NotifyUserWithYesNoOption(
                    new Notification("File found in different location.",
                                     $"The file '{fileName}' was found in a different location. Do you want to use the K values saved to '{file.FemModelFilePath}'?",
                                     WarningLevel.Warning));
                if (yesNoResult == MessageBoxResult.No)
                    return result;
            }

            // If it can, then get the K values for each beam in the list and set them to the beam objects.
            var kValues = await _beamKValueRepo.Where(b => b.FemFileStableId == file.FileId);
            foreach (var kValue in kValues ?? Enumerable.Empty<BeamKValueEntity>())
                result.Add(kValue.MapToBeamKValue());
        }
        catch (Exception ex)
        {
            _logger.LogError(typeof(BeamKFactorDataService), $"Failed to retrieve K-values for file '{fileName}': {ex.Message}");
        }

        return result;
    }

    public async Task SaveBeamKValues(string fileName, IEnumerable<BeamKValue> kValues)
    {
        if (string.IsNullOrWhiteSpace(fileName) || kValues == null)
            return;

        var file = await _femFilePathDataService.GetOrCreateFileByName(fileName);
        if (file is null)
            return;

        var kValuesList = kValues.ToList();
        if (kValuesList.Count == 0)
            return;

        var existing = await _beamKValueRepo.Where(b => b.FemFileStableId == file.FileId);
        var existingDict = existing?.ToDictionary(e => e.BeamNumber) ?? new Dictionary<int, BeamKValueEntity>();

        var toUpdate = new List<BeamKValueEntity>();
        var toAdd = new List<BeamKValueEntity>();

        foreach (var kValue in kValuesList)
        {
            if (existingDict.TryGetValue(kValue.BeamNumber, out var found))
            {
                found.UpdateProperties(kValue);
                toUpdate.Add(found);
            }
            else
            {
                var entity = kValue.MapToBeamKValueEntity(file.FileId!.Value);
                toAdd.Add(entity);
            }
        }

        if (toUpdate.Count > 0)
            await _beamKValueRepo.UpdateAllAsync(toUpdate);

        if (toAdd.Count > 0)
            await _beamKValueRepo.AddAllAsync(toAdd);

        await unitOfWork.Commit();
    }
}