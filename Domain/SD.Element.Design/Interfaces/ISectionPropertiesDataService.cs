using SD.Core.Shared.Models.BeamModels;

namespace SD.Element.Design.Interfaces;

public interface ISectionPropertiesDataService
{
    public Task GetSectionDesignSettingsByFileName(string fileName, IEnumerable<Section> sections);
    public Task SaveSectionDesignSettings(string fileName, IEnumerable<Section> sections);
}