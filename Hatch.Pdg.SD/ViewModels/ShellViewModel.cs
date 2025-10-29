using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Events;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.Helpers;
using SD.UI.Loading.Views;
using SD.UI.Main.Views;
using SD.UI.Singletons;
using SD.UI.Tools.Views;
using SD.UI.UltimateLimitState.Views;
using SD.UI.ViewModel;

namespace Hatch.Pdg.SD.ViewModels;
public partial class ShellViewModel : FemViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConnectionService _connectionService;
    private readonly IRegionManager _regionManager;
    private readonly ISplashService _splashService;
    private readonly IRuntimeAppSettings _runtimeAppSettings;
    private readonly IEventAggregator _eventAggregator;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    public bool isFemLoaded = true;
    [ObservableProperty]
    public bool notificationDisplayed = false;
    [ObservableProperty]
    public bool isBrowserLoaded = true;
    [ObservableProperty]
    public bool showShell;
    [ObservableProperty]
    public WindowResizer? mainWindowResizer;
    [ObservableProperty]
    public IProcessModel _processModel;
    [ObservableProperty]
    public ISnackbarModel _snackbarModel;

    private FemLoadedEvent? _femLoadedEvent;
    private DialogOpenedEvent? _dialogOpenedEvent;
    private DialogClosedEvent? _dialogClosedEvent;
    private ShellResizeEvent? _shellResizeResizeEvent;
    private AppShutdownEvent? _appShutdownEvent;
    private FileClosedEvent? _fileClosedEvent;

    public ShellViewModel(IViewManagementModel viewManagementModel,
                          IRegionManager regionManager,
                          IAuthenticationService authenticationService,
                          IProcessModel processModel,
                          IConnectionService connectionService,
                          INotificationService notificationService,
                          ISplashService splashService,
                          IRuntimeAppSettings runtimeAppSettings,
                          IEventAggregator eventAggregator,
                          ISnackbarModel snackbarModel) : base(viewManagementModel)
    {
        _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        _processModel = processModel ?? throw new ArgumentNullException(nameof(processModel));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _splashService = splashService ?? throw new ArgumentNullException(nameof(splashService));
        _runtimeAppSettings = runtimeAppSettings ?? throw new ArgumentNullException(nameof(runtimeAppSettings));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _snackbarModel = snackbarModel ?? throw new ArgumentNullException(nameof(snackbarModel));

        _regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(MenuView));
        _regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
        _regionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolBarView));
        _regionManager.RegisterViewWithRegion(RegionNames.NavigationRegion, typeof(NavigationView));
        _regionManager.RegisterViewWithRegion(RegionNames.BrowserRegion, typeof(FileBrowserView));
        
        _regionManager.RegisterViewWithRegion(RegionNames.RightDrawerContentRegion, typeof(GeneralToolsView));

        _regionManager.RegisterViewWithRegion(RegionNames.SettingsRegion, typeof(SettingsView));
        _regionManager.RegisterViewWithRegion(RegionNames.NotificationRegion, typeof(NotificationView));
        _regionManager.RegisterViewWithRegion(RegionNames.BeamPropertiesRegion, typeof(BeamPropertiesView));
        _regionManager.RegisterViewWithRegion(RegionNames.DesignSettingsRegion, typeof(DesignSettingsView));
    }

    [RelayCommand]
    public void Loaded()
    {
        var certified = _authenticationService.CertifyApplication();
        if (certified.IsFailure)
        {
            _notificationService.ShutdownAfterErrorNotice(new Notification("Application Certification Error", certified.Message));
            return;
        }

        var authorised = _authenticationService.AuthoriseUser();
        if (authorised.IsFailure)
        {
            _notificationService.ShutdownAfterErrorNotice(new Notification("Application Authorisation Error", authorised.Message));
            return;
        }

        _appShutdownEvent = _eventAggregator.GetEvent<AppShutdownEvent>();

        if (_runtimeAppSettings.RequiresRestart)
            Task.Run(() => Task.Delay(5000)).GetAwaiter().GetResult();

        var strand7Connect = _connectionService.ConnectToStrand7Api();
        if (!strand7Connect)
            _appShutdownEvent.Publish();

        _splashService.CloseSplash(false);
        ShowShell = true;

        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _fileClosedEvent.Subscribe(BrowserLoaded);

        _femLoadedEvent = _eventAggregator.GetEvent<FemLoadedEvent>();
        _femLoadedEvent.Subscribe(FemLoaded);

        _dialogOpenedEvent = _eventAggregator.GetEvent<DialogOpenedEvent>();
        _dialogOpenedEvent.Subscribe(DialogOpened);

        _dialogClosedEvent = _eventAggregator.GetEvent<DialogClosedEvent>();
        _dialogClosedEvent.Subscribe(DialogClosed);

        _shellResizeResizeEvent = _eventAggregator.GetEvent<ShellResizeEvent>();

        BrowserLoaded();
    }

    [RelayCommand]
    public void WindowResize()
    {
        _shellResizeResizeEvent?.Publish();
    }

    [RelayCommand]
    public void WindowClose()
    {
        _appShutdownEvent?.Publish();
    }

    [RelayCommand]
    public async Task DrawerClosing()
    {
        await Task.Delay(500);
        ViewManagementModel.IsDrawerOpen = false;
    }

    private void DialogClosed()
    {
        NotificationDisplayed = false;
        ViewManagementModel.IsDialogOpen = false;
    }

    private void DialogOpened(Notification notification)
    {
        NotificationDisplayed = true;
        ViewManagementModel.IsDialogOpen = true;
    }

    private void FemLoaded()
    {
        IsFemLoaded = true;
        IsBrowserLoaded = false;
    }

    private void BrowserLoaded()
    {
        IsFemLoaded = false;
        IsBrowserLoaded = true;
    }

    [RelayCommand]
    public void Closing()
    {
        _connectionService.ReleaseStrand7Api();
        _femLoadedEvent?.Unsubscribe(FemLoaded);
        _dialogOpenedEvent?.Unsubscribe(DialogOpened);
        _fileClosedEvent?.Unsubscribe(BrowserLoaded);
    }
}