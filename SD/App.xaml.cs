using Microsoft.Extensions.Configuration;
using SD.Adapters;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Infrastructure.Logging;
using SD.Core.Infrastructure.Services;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Events;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Core;
using SD.Data.Interfaces;
using SD.Data.Repository;
using SD.Data.Services;
using SD.Element.Design.AS.Services;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Models;
using SD.Element.Design.Sans.Services;
using SD.Element.Design.Services;
using SD.Fem.Strand7.Interfaces;
using SD.Fem.Strand7.Services;
using SD.MathcadPrime.Interfaces;
using SD.MathcadPrime.Services;
using SD.Services;
using SD.UI.Models;
using SD.UI.Services;
using SD.UI.Singletons;
using SD.UI.UltimateLimitState.ViewModels;
using SD.Views;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SD;

public partial class App : PrismApplication
{
    private AppShutdownEvent? _shutdownEvent;
    private ILoggerService? _logger;
    private IFemFilePathBlobService? _femFilePathService;

    [STAThread]
    protected override Window CreateShell()
    {
        Exit += OnCurrentExit;
        SubscribeToAppExitEvent();

        SetupExceptionHandling();
        EnsureApplicationIntegrity();

        var splashService = Container.Resolve<ISplashService>();
        splashService.ShowSplash<Splash>();
        return Container.Resolve<Shell>();
    }

    private void EnsureApplicationIntegrity()
    {
        try
        {
            var integritySettings = Container.Resolve<IntegritySettings>();
            ArtifactIntegrityService.ValidateOrThrow(integritySettings, AppContext.BaseDirectory);
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(
                $"Application integrity validation failed and the app will now close.\n\n{exception.Message}",
                "Security Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Current.Shutdown();
            throw;
        }
    }

    private void SubscribeToAppExitEvent()
    {
        var eventAggregator = Container.Resolve<IEventAggregator>();
        _femFilePathService = Container.Resolve<IFemFilePathBlobService>();
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
            if (Current != null)
            {
                foreach (Window window in Current.Windows)
                    window.Close();

                Current.Shutdown();
            }
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
        catch (Exception ex)
        {
            _logger!.LogError(GetType(), $"Failed to restart application: Exception {ex.Message}");
        }

        _shutdownEvent?.Publish();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
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
        containerRegistry.Register<ITankDesignService, TankDesignService>();
        containerRegistry.Register<IStrandApiCreateService, StrandApiCreateService>();

        containerRegistry.Register<IDesignCodeAdapter, DesignCodeAdapter>();
        containerRegistry.Register<IBeamChainService, BeamChainService>();
        containerRegistry.Register<ISaveService, SaveService>();
        containerRegistry.Register<IBeamDesignService, BeamDesignService>();
        containerRegistry.Register<IEffectiveLengthService, StrandEffectiveLengthService>();
        containerRegistry.Register<IBucklingAnalysisService, BucklingAnalysisService>();

        containerRegistry.Register<IUlsDataExportService, UlsDataExportService>();

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
        containerRegistry.RegisterSingleton<IBeamAxisDisplay, BeamAxisDisplay>();

        // Register eagerly loaded view models as singletons
        containerRegistry.RegisterSingleton<FemModelViewModel>();
        containerRegistry.RegisterSingleton<CombinationsTableViewModel>();
        containerRegistry.RegisterSingleton<BeamFemModelViewModel>();
        containerRegistry.RegisterSingleton<BeamDesignViewModel>();

        RegisterLogger(containerRegistry);
        RegisterRepositories(containerRegistry);
        RegisterConfigSettings(containerRegistry);
        RegisterRuntimeSettings(containerRegistry);
        RegisterHttpClients(containerRegistry);
    }

    private void RegisterHttpClients(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IWebApiHttpClient, WebApiHttpClient>();
    }

    private void RegisterLogger(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
        _logger = Container.Resolve<ILoggerService>() ?? throw new InvalidOperationException("Logger service could not be resolved.");
    }

    private void RegisterRuntimeSettings(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IRuntimeAppSettings?, RuntimeAppSettings>();
        var femFilePathService = Container.Resolve<IFemFilePathBlobService>();

        var appRuntimeSettings = femFilePathService.GetRuntimeSettings();

        if (appRuntimeSettings != null)
            containerRegistry.RegisterInstance(appRuntimeSettings);
    }

    private static void RegisterRepositories(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<ISectionPropertiesDataService, SectionPropertiesDataService>();

        containerRegistry.Register<IBeamKFactorDataService, BeamKFactorDataService>();
        containerRegistry.Register<IFemFilePathDataService, FemFilePathDataService>();
        containerRegistry.Register<IFemFilePathBlobService, FemFilePathBlobService>();
        containerRegistry.Register<IUserPreferencesService, UserPreferencesService>();

        containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
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

        var integritySettings = new IntegritySettings();
        configuration.GetSection("Integrity").Bind(integritySettings);
        containerRegistry.RegisterInstance(integritySettings);
    }

    private void OnCurrentExit(object sender, ExitEventArgs e)
    {
        try
        {
            var mathcadService = Container.Resolve<IMathcadService>();
            if (mathcadService != null)
                mathcadService.CloseMathcad();
        }
        catch (Exception)
        {
            return;
        }
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
            _logger!.LogError(GetType(), $"{message} : Exception {ex.Message}");
        }
        finally
        {
            _logger!.LogError(GetType(), $"{message} : Exception {exception.Message}");
            _shutdownEvent!.Publish();
        }
    }
    #endregion
}