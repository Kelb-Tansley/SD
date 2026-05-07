using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.UI.Events;
using SD.UI.ViewModel;

namespace SD.UI.Main.ViewModels;

public partial class ToolBarViewModel : ViewModelBase
{
    private readonly IRegionManager _regionManager;
    private readonly IViewManagementModel _viewManagementModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUlsDesignResults _ulsDesignResults;

    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;
    private readonly CalculateEvent _calculateEvent;
    private readonly RefreshEvent _refreshEvent;

    [ObservableProperty]
    public required partial IDesignModel DesignModel { get; set; }

    [ObservableProperty]
    public required IFemModelParameters _femModelParameters;

    [ObservableProperty]
    public required IBeamAxisDisplay _beamAxisDisplay;

    [ObservableProperty]
    public bool _femModelOpened;

    [ObservableProperty]
    public int _nonDesignableSectionsCount;

    [ObservableProperty]
    public bool _useEnvelopeLoadCase;

    public ToolBarViewModel(IRegionManager regionManager,
                            IViewManagementModel viewManagementModel,
                            IFemModel femModel,
                            IDesignModel designModel,
                            IProcessModel processModel,
                            IUlsDesignResults ulsDesignResults,
                            IFemModelParameters femModelParameters,
                            IEventAggregator eventAggregator,
                            IBeamAxisDisplay beamAxisDisplay) : base(processModel)
    {
        _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
        _viewManagementModel = viewManagementModel ?? throw new ArgumentNullException(nameof(viewManagementModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        DesignModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _beamAxisDisplay = beamAxisDisplay ?? throw new ArgumentNullException(nameof(beamAxisDisplay));

        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _refreshEvent = _eventAggregator.GetEvent<RefreshEvent>();
        _designCodeChangedEvent = _eventAggregator.GetEvent<DesignCodeChangedEvent>();
        _calculateEvent = _eventAggregator.GetEvent<CalculateEvent>();
    }

    [RelayCommand]
    private void DesignCodeChanged()
    {
        _designCodeChangedEvent?.Publish();

        Refresh();
    }

    [RelayCommand]
    private void Calculate()
    {
        _calculateEvent?.Publish();
    }

    [RelayCommand]
    private void Refresh()
    {
        FemModelParameters.Clear();
        _ulsDesignResults.Clear(); //Check if this is correct
        _refreshEvent.Publish();
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        _viewManagementModel.IsDrawerOpen = true;
        await Task.Delay(200);
        _viewManagementModel.IsDialogOpen = true;
    }

    [RelayCommand]
    public void Loaded()
    {
        _fileClosedEvent.Subscribe(FileClosed);
        _designCodeChangedEvent.Subscribe(DesignCodeChanged);
    }

    [RelayCommand]
    public void Closing()
    {
        _fileClosedEvent.Unsubscribe(FileClosed);
    }

    private void FileClosed()
    {
        ProcessModel.IsFemModelLoaded = false;
        FemModelOpened = false;
        _femModel.ClearFile();
        FemModelParameters.Clear();
        _ulsDesignResults.Clear();
    }
}