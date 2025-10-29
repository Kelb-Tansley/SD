using Microsoft.Extensions.DependencyInjection;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Infrastructure.Services;
using SD.Core.Shared.Enum;
using SD.Element.Design.AS.Services;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Sans.Services;

namespace Hatch.Pdg.SD.Adapters;
public class DesignCodeAdapter : IDesignCodeAdapter
{
    private readonly IContainerProvider? _containerProvider = null;
    private readonly IServiceCollection? _services = null;
    private readonly ServiceProvider? _serviceProvider = null;
    private bool _isTest;

    public DesignCodeAdapter(IServiceCollection services)
    {
        _services = services;
        _isTest = true;
        _serviceProvider = services.BuildServiceProvider();
    }

    public DesignCodeAdapter(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public IElementDesignService GetDesignService(DesignCode designCode)
    {
        if (_isTest && _serviceProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _serviceProvider.GetService<SansDesignService>(),
                DesignCode.AS => _serviceProvider.GetService<ASDesignService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        else if (_containerProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _containerProvider.Resolve<SansDesignService>(),
                DesignCode.AS => _containerProvider.Resolve<ASDesignService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        throw new NotImplementedException(nameof(GetDesignService));
    }

    public IDeflectionService GetDeflectionService(DesignCode designCode)
    {
        if (_isTest && _serviceProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _serviceProvider.GetService<SansDeflectionService>(),
                DesignCode.AS => _serviceProvider.GetService<ASDeflectionService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        else if (_containerProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _containerProvider.Resolve<SansDeflectionService>(),
                DesignCode.AS => _containerProvider.Resolve<ASDeflectionService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        throw new NotImplementedException(nameof(GetDeflectionService));
    }

    public IBeamPropertiesService GetBeamPropertiesService(DesignCode designCode)
    {
        if (_isTest && _serviceProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _serviceProvider.GetService<SansBeamPropertiesService>(),
                DesignCode.AS => _serviceProvider.GetService<ASBeamPropertiesService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        else if (_containerProvider != null)
            return designCode switch
            {
                DesignCode.SANS => _containerProvider.Resolve<SansBeamPropertiesService>(),
                DesignCode.AS => _containerProvider.Resolve<ASBeamPropertiesService>(),
                _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
            };
        throw new NotImplementedException(nameof(GetBeamPropertiesService));
    }

    public IExportResultsService GetExportResultsService(DesignCode designCode)
    {
        return designCode switch
        {
            DesignCode.SANS => _containerProvider.Resolve<SansExportResultsService>(),
            DesignCode.AS => _containerProvider.Resolve<AsExportResultsService>(),
            _ => throw new NotImplementedException($"{nameof(DesignCodeAdapter)}: Unknown design code {nameof(designCode)}")
        };
    }
}
