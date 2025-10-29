using SD.MathcadPrime.Interfaces;
using SD.MathcadPrime.Services;
using Hatch.Pdg.SD.Adapters;
using Hatch.Pdg.SD.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using SD.Core.Infrastructure.Logging;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Core;
using SD.Data.Interfaces;
using SD.Data.Services;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Sans.Models;
using SD.Element.Design.Sans.Services;
using SD.Element.Design.AS.Models;
using SD.Element.Design.AS.Services;
using SD.Fem.Strand7.Interfaces;
using SD.Fem.Strand7.Services;
using SD.UI.Services;
using SD.UI.Singletons;
using SD.Element.Design.Models;
using SD.Element.Design.Services;
using SD.Core.Infrastructure.Interfaces;

namespace SD.Tests.Shared.Helpers;

public static class SpecFlowStartupHelper
{
    public static IServiceCollection StartupSpecflowTests()
    {
        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var appSettings = new AppSettings();
        configuration.GetSection("App").Bind(appSettings);
        appSettings.Initialize();
        services.AddSingleton<IAppSettings>(appSettings);

        var apiSettings = new ApiSettings();
        configuration.GetSection("Api").Bind(apiSettings);
        services.AddSingleton(apiSettings);

        //Primary services
        services.AddSingleton<IRuntimeAppSettings, RuntimeAppSettings>();
        services.AddScoped<IFemFilePathService, FemFilePathService>();
        services.AddScoped<ILoggerService, LoggerService>();

        // Register services to interfaces
        services.AddSingleton<ISplashService, SplashService>()
                .AddSingleton<IConnectionService, ConnectionService>();

        services.AddScoped<IEventAggregator, EventAggregator>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IContourFileService, ContourFileService>()
                .AddScoped<IFemModelDisplayService, FemModelDisplayService>()
                .AddScoped<IStrandResultsService, StrandResultsService>()
                .AddScoped<IBeamChainService, BeamChainService>()
                .AddScoped<IEffectiveLengthService, StrandEffectiveLengthService>()
                .AddScoped<IAsMathcadService, AsMathcadService>()
                .AddScoped<ISansMathcadService, SansMathcadService>()
                .AddScoped<IStrandApiService, StrandApiService>();

        // Register singleton models
        services.AddSingleton<IFemModel, FemModel>();
        services.AddSingleton<IDesignModel, DesignModel>();
        services.AddSingleton<IUlsDesignResults, UlsDesignResults>();
        //services.AddSingleton<IASDesignResults, ASDesignResults>();
        services.AddSingleton<IFemModelParameters, FemModelParameters>();

        // Code specific services
        services.AddScoped<SansDeflectionService>();
        services.AddScoped<ASDeflectionService>();
        services.AddScoped<SansDesignService>();
        services.AddScoped<ASDesignService>();
        services.AddScoped<ASBeamPropertiesService>();
        services.AddScoped<SansBeamPropertiesService>();

        var adapter = new DesignCodeAdapter(services);
        services.AddSingleton<IDesignCodeAdapter>(adapter);
        return services;
    }
}
