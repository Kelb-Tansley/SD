using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Events;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.Enums;
using SD.UI.Events;
using SD.UI.ViewModel;

namespace SD.UI.Main.ViewModels;
public partial class MenuViewModel : FemViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IStrandApiService _strandApiService;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly AppShutdownEvent _appExitEvent;
    private readonly WindViewLoadEvent _windViewLoadEvent;
    private readonly GeneralToolsViewChangedEvent _generalToolsViewChangedEvent;

    [ObservableProperty]
    private IFemModel _femModel;

    public MenuViewModel(IViewManagementModel viewManagementModel,
                         IFemModel femModel,
                         IStrandApiService strandApiService,
                         IEventAggregator eventAggregator) : base(viewManagementModel)
    {
        _femModel = femModel;
        _eventAggregator = eventAggregator;
        _strandApiService = strandApiService;

        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _appExitEvent = _eventAggregator.GetEvent<AppShutdownEvent>();
        _windViewLoadEvent = _eventAggregator.GetEvent<WindViewLoadEvent>();
        _generalToolsViewChangedEvent = _eventAggregator.GetEvent<GeneralToolsViewChangedEvent>();

        _fileClosedEvent.Subscribe(FileClosed);
    }

    [RelayCommand]
    public void CloseFile()
    {
        _fileClosedEvent.Publish();
    }

    [RelayCommand]
    public void ExitApp()
    {
        _appExitEvent.Publish();
    }

    [RelayCommand]
    public async Task BeamWindLoad()
    {
        ViewManagementModel.IsDrawerOpen = true;

        _generalToolsViewChangedEvent.Publish(GeneralToolsView.WindLoading);
        await Task.Delay(150);

        _windViewLoadEvent.Publish();

        ViewManagementModel.IsRightDrawerOpen = true;
    }

    [RelayCommand]
    public async Task BucklingAnalysis()
    {
        ViewManagementModel.IsDrawerOpen = true;

        _generalToolsViewChangedEvent.Publish(GeneralToolsView.BucklingAnalysis);
        await Task.Delay(150);

        ViewManagementModel.IsRightDrawerOpen = true;
    }

    private void FileClosed()
    {
        _strandApiService.CloseAllFemFiles(FemModels.ModelId);
        _strandApiService.CloseAllFemFiles(FemModels.DisplayModelId);
        _strandApiService.CloseAllFemFiles(FemModels.DesignModelId);
        _strandApiService.CloseAllFemFiles(FemModels.ServiceabilityModelId);
    }
}
