using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Enum;

namespace SD.Element.Design.Interfaces;
public interface IDesignCodeAdapter
{
    public IElementDesignService GetDesignService(DesignCode designCode);
    public IDeflectionService GetDeflectionService(DesignCode designCode);
    public IBeamPropertiesService GetBeamPropertiesService(DesignCode designCode);
    public IExportResultsService GetExportResultsService(DesignCode designCode);
}