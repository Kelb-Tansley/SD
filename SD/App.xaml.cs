using System.Windows;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Sans.Models;
using SD.Fem.Strand7.Services;
using SD.Fem.Strand7.Interfaces;
using SD.Core.Shared.Contracts;
using Hatch.Pdg.SD.Views;
using SD.UI.Singletons;
using SD.Core.Shared.Models;
using SD.Data.Services;
using SD.Data.Repository;
using SD.Data.Interfaces;
using SD.Data.Entities;
using SD.Data.Mapping;
using SD.Core.Shared.Models.BeamModels;
using Hatch.Pdg.SD.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using SD.UI.Services;
using SD.Element.Design.Sans.Services;
using Hatch.Pdg.SD.Adapters;
using SD.MathcadPrime.Interfaces;
using SD.MathcadPrime.Services;
using System.Reflection;
using SD.Core.Infrastructure.Logging;
using System.Diagnostics;
using SD.Core.Shared.Models.Core;
using SD.Element.Design.AS.Services;
using SD.Core.Shared.Events;
using SD.Element.Design.Models;
using SD.Element.Design.Services;
using SD.Core.Infrastructure.Interfaces;

namespace Hatch.Pdg.SD;
public partial class App : PrismApplication
{
    private AppShutdownEvent? _shutdownEvent;
    private ILoggerService? _logger;
    private IFemFilePathService? _femFilePathService;

    [STAThread]
    protected override Window CreateShell()
    {
        Exit += OnCurrentExit;
        SubscribeToAppExitEvent();

        SetupExceptionHandling();

        var splashService = Container.Resolve<ISplashService>();
        splashService.ShowSplash<Splash>();
        return Container.Resolve<Shell>();
    }

    private void SubscribeToAppExitEvent()
    {
        var eventAggregator = Container.Resolve<IEventAggregator>();
        _femFilePathService = Container.Resolve<IFemFilePathService>();
        _shutdownEvent = eventAggregator.GetEvent<AppShutdownEvent>();
        _shutdownEvent.Subscribe(ShutdownApplication);
        var restartEvent = eventAggregator.GetEvent<AppRestartEvent>();
        restartEvent.Subscribe(RestartApplication);
    }

    private void ShutdownApplication()
    {
        _femFilePathService?.SaveRuntimeSettings();

        // Ensure this runs on the UI thread
        Current.Dispatcher.Invoke(() =>
        {
            foreach (Window window in Current.Windows)
                window.Close();

            Current.Shutdown();
        });
    }

    private void RestartApplication()
    {
        try
        {
            _femFilePathService?.SaveRuntimeSettings();
            Process.Start(new ProcessStartInfo($"SD.Fem.Strand7.App.exe")
            {
                Arguments = "3000",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
        catch (Exception) { }

        _shutdownEvent?.Publish();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ISplashService, SplashService>();

        // Register services to interfaces
        containerRegistry.RegisterSingleton<IConnectionService, ConnectionService>();
        containerRegistry.RegisterSingleton<IDesignService, DesignService>();

        containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

        containerRegistry.Register<IStrandApiService, StrandApiService>();
        containerRegistry.Register<IStrandResultsService, StrandResultsService>();
        containerRegistry.Register<IContourFileService, ContourFileService>();
        containerRegistry.Register<IFemModelDisplayService, FemModelDisplayService>();

        containerRegistry.Register<IDesignCodeAdapter, DesignCodeAdapter>();
        containerRegistry.Register<IBeamChainService, BeamChainService>();
        containerRegistry.Register<IEffectiveLengthService, StrandEffectiveLengthService>();
        containerRegistry.Register<IBucklingAnalysisService, BucklingAnalysisService>();

        containerRegistry.Register<IDataAccessService, DataAccessService>();
        containerRegistry.Register<IFemFilePathService, FemFilePathService>();

        containerRegistry.Register<ITokenCacheService, TokenCacheService>();
        containerRegistry.Register<IAuthenticationService, AuthenticationService>();

        // Code specific services
        containerRegistry.Register<IDeflectionService, SansDeflectionService>();
        containerRegistry.Register<IElementDesignService, SansDesignService>();
        containerRegistry.Register<IElementDesignService, ASDesignService>();
        containerRegistry.Register<IBeamPropertiesService, ASBeamPropertiesService>();
        containerRegistry.Register<IBeamPropertiesService, SansBeamPropertiesService>();

        // Third party services
        containerRegistry.RegisterSingleton<IMathcadService, MathcadService>();
        containerRegistry.RegisterSingleton<ISansMathcadService, SansMathcadService>();
        containerRegistry.RegisterSingleton<IAsMathcadService, AsMathcadService>();

        // Register singleton models
        containerRegistry.RegisterSingleton<IFemModel, FemModel>();
        containerRegistry.RegisterSingleton<IProcessModel, ProcessModel>();
        containerRegistry.RegisterSingleton<IDesignModel, DesignModel>();
        containerRegistry.RegisterSingleton<IUlsDesignResults, UlsDesignResults>();
        //containerRegistry.RegisterSingleton<IASDesignResults, ASDesignResults>();
        containerRegistry.RegisterSingleton<IFemModelParameters, FemModelParameters>();
        containerRegistry.RegisterSingleton<IViewManagementModel, ViewManagementModel>();
        containerRegistry.RegisterSingleton<ISnackbarModel, SnackbarModel>();

        RegisterLogger(containerRegistry);
        RegisterRepositories(containerRegistry);
        RegisterMappers(containerRegistry);
        RegisterConfigSettings(containerRegistry);
        RegisterRuntimeSettings(containerRegistry);
    }

    private void RegisterLogger(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
        _logger = Container.Resolve<ILoggerService>();
    }

    private void RegisterRuntimeSettings(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IRuntimeAppSettings?, RuntimeAppSettings>();
        var femFilePathService = Container.Resolve<IFemFilePathService>();

        var appRuntimeSettings = femFilePathService.GetRuntimeSettings();

        if (appRuntimeSettings != null)
            containerRegistry.RegisterInstance(appRuntimeSettings);
    }

    private static void RegisterRepositories(IContainerRegistry containerRegistry)
    {
        //containerRegistry.Register<IRepository<BeamPropertySettings>, Repository<BeamPropertySettings>>();
        //containerRegistry.Register<IRepository<DesignSettings>, Repository<DesignSettings>>();
        containerRegistry.Register<IUnitOfWork, UnitOfWork>();
    }

    private static void RegisterMappers(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IEntityMapper<BeamPropertySettings, Section>, BeamPropertySettingsMapper>();
        containerRegistry.Register<IEntityMapper<DesignSettings, BeamDesignSettings>, DesignSettingsMapper>();
    }

    private static void RegisterConfigSettings(IContainerRegistry containerRegistry)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var appSettings = new AppSettings();
        configuration.GetSection("App").Bind(appSettings);
        containerRegistry.RegisterInstance<IAppSettings>(appSettings);
        appSettings.Initialize();

        var apiSettings = new ApiSettings();
        configuration.GetSection("Api").Bind(apiSettings);
        containerRegistry.RegisterInstance(apiSettings);
    }

    private void OnCurrentExit(object sender, ExitEventArgs e)
    {
        var mathcadService = Container.Resolve<IMathcadService>();
        mathcadService.CloseMathcad();
    }

    #region Exception Handling
    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            var aggException = e.Exception.Flatten();
            foreach (var exception in aggException.InnerExceptions)
                LogUnhandledException(exception, "TaskScheduler.UnobservedTaskException");

            e.SetObserved();
        };
    }
    private void LogUnhandledException(Exception exception, string source)
    {
        var message = $"UNHANDLED Exception ({source})";
        try
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
        }
        catch (Exception ex)
        {
            _logger.LogError(GetType(), $"{message} : Exception {ex.Message}");
        }
        finally
        {
            _logger.LogError(GetType(), $"{message} : Exception {exception.Message}");
            _shutdownEvent.Publish();
        }
    }
    #endregion
}