using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Events;
using SD.Data.Interfaces;
using SD.Element.Design.Interfaces;
using System.Reflection;

namespace Hatch.Pdg.SD.ViewModels;
public partial class SplashViewModel : ObservableObject
{
    [ObservableProperty]
    public string? applicationVersion = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();

    [ObservableProperty]
    public string applicationEnvironment;

    [ObservableProperty]
    public string applicationStateMessage = "Launching...";

    [ObservableProperty]
    public string strand7ApiFolder;

    [ObservableProperty]
    public bool hasStrand7LocationError;

    private readonly ISplashService _splashService;
    private readonly IRuntimeAppSettings _runtimeAppSettings;
    private readonly IFemFilePathService _femFilePathService;
    private readonly IEventAggregator _eventAggregator;
    private readonly AppRestartEvent _appRestartEvent;

    public SplashViewModel(ISplashService splashService,
                           IRuntimeAppSettings runtimeAppSettings,
                           IFemFilePathService femFilePathService,
                           IEventAggregator eventAggregator)
    {
        _splashService = splashService;
        _runtimeAppSettings = runtimeAppSettings;
        _femFilePathService = femFilePathService;
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

        _appRestartEvent = _eventAggregator.GetEvent<AppRestartEvent>();
    }

    [RelayCommand]
    public void BrowseStrand7ApiLocation()
    {
        var openFileDialog = new FolderBrowserDialog { };

        if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            return;

        Strand7ApiFolder = openFileDialog.SelectedPath;
        _femFilePathService.InsertStrandApiPath(Strand7ApiFolder);

        _runtimeAppSettings.RequiresRestart = true;
        _appRestartEvent.Publish();
    }


    [RelayCommand]
    public void Minimize()
    {
        _splashService.MinimizeSplashWindow();
    }

    [RelayCommand]
    public void Close()
    {
        _splashService.CloseSplash(true);
    }
}
