using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.UI.Events;
using SD.UI.ViewModel;

namespace SD.UI.Main.ViewModels;

public partial class ToolBarViewModel : ViewModelBase
{
    private readonly IViewManagementModel _viewManagementModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUlsDesignResults _ulsDesignResults;

    private readonly FileClosedEvent _fileClosedEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;
    private readonly CalculateEvent _calculateEvent;
    private readonly RefreshEvent _refreshEvent;

    [ObservableProperty]
    public required partial IDesignModel DesignModel { get; set; }

    [ObservableProperty]
    public required partial IFemModelParameters FemModelParameters { get; set; }

    [ObservableProperty]
    public required partial IBeamAxisDisplay BeamAxisDisplay { get; set; }

    [ObservableProperty]
    public partial bool FemModelOpened { get; set; }

    [ObservableProperty]
    public partial int NonDesignableSectionsCount { get; set; }

    [ObservableProperty]
    public partial bool UseEnvelopeLoadCase { get; set; }

    public ToolBarViewModel(IViewManagementModel viewManagementModel,
                            IFemModel femModel,
                            IDesignModel designModel,
                            IProcessModel processModel,
                            IUlsDesignResults ulsDesignResults,
                            IFemModelParameters femModelParameters,
                            IEventAggregator eventAggregator,
                            IBeamAxisDisplay beamAxisDisplay) : base(processModel)
    {
        _viewManagementModel = viewManagementModel ?? throw new ArgumentNullException(nameof(viewManagementModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        DesignModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        FemModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        BeamAxisDisplay = beamAxisDisplay ?? throw new ArgumentNullException(nameof(beamAxisDisplay));

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