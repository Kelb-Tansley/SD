using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Enum;
using SD.Data.Entities;
using SD.Data.Interfaces;
using SD.Data.Mapping;
using System.Windows;

namespace SD.Data.Services;

public class SectionPropertiesDataService(IUnitOfWork unitOfWork, IFemFilePathDataService femFilePathDataService, INotificationService notificationService) : ISectionPropertiesDataService
{
    private readonly IRepository<SectionDesignSetting> _sectionDesignSettingRepo = unitOfWork.GetRepository<SectionDesignSetting>();

    private readonly IFemFilePathDataService _femFilePathDataService = femFilePathDataService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task GetSectionDesignSettingsByFileName(string fileName, IEnumerable<Section> sections)
    {
        if (string.IsNullOrWhiteSpace(fileName) || sections is null || !sections.Any())
            return;

        try
        {
            // The file may have been moved, so it cannot be found by the full path.
            var file = await _femFilePathDataService.GetFileByName(fileName);
            if (file is null)
                return;

            if (!file.ExactMatchFound)
            {
                var yesNoResult = _notificationService.NotifyUserWithYesNoOption(
                    new Notification("File found in different location.",
                                     $"The file '{fileName}' was found in a different location. Do you want to use the properties saved to '{file.FemModelFilePath}'?",
                                     WarningLevel.Warning));
                if (yesNoResult == MessageBoxResult.No)
                    return;
            }

            // If it can, then get the K values for each beam in the list and set them to the beam objects.
            var settings = await _sectionDesignSettingRepo.Where(b => b.FemFileStableId == file.FileId);
            foreach (var setting in settings ?? Enumerable.Empty<SectionDesignSetting>())
            {
                var section = sections.FirstOrDefault(s => s.Number == setting.PropertyNumber);
                section?.MapToSectionSetting(setting);
            }
        }
        catch (Exception)
        { }
    }

    public async Task SaveSectionDesignSettings(string fileName, IEnumerable<Section> sections)
    {
        if (string.IsNullOrWhiteSpace(fileName) || sections == null)
            return;

        var file = await _femFilePathDataService.GetOrCreateFileByName(fileName);
        if (file is null)
            return;

        var existing = await _sectionDesignSettingRepo.Where(b => b.FemFileStableId == file.FileId);

        foreach (var section in sections)
        {
            var found = existing?.FirstOrDefault(e => e.PropertyNumber == section.Number);
            if (found is not null)
            {
                found!.UpdateProperties(section);
                await _sectionDesignSettingRepo.UpdateAsync(found);
            }
            else
            {
                var entity = section.MapToSectionDesignSettingEntity(file.FileId!.Value);
                await _sectionDesignSettingRepo.AddAsync(entity);
            }
        }

        await unitOfWork.Commit();
    }
}
