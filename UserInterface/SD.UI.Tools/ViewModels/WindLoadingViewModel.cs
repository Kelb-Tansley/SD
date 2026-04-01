using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Loading;
using SD.Element.Design.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;

namespace SD.UI.Tools.ViewModels;

public partial class WindLoadingViewModel : FemDisplayViewModelBase
{
    private readonly IFemModelParameters _femModelParameters;
    private readonly IStrandApiService _strandApiService;
    private readonly INotificationService _notificationService;
    private readonly IDesignModel _designModel;
    private readonly WindViewLoadEvent _windViewLoadEvent;

    [ObservableProperty]
    public WindLoadingModel _windLoadingModel = new();

    [ObservableProperty]
    public ObservableCollection<LoadCase> _loadCases = [];

    [ObservableProperty]
    public ObservableCollection<string> _loadDirections = ["X", "Y", "Z", "-X", "-Y", "-Z"];

    [ObservableProperty]
    public LoadCase? _selectedLoadCase;

    [ObservableProperty]
    public string _selectedLoadDirection;

    private double[] windLoadVector = [1, 0, 0];

    public WindLoadingViewModel(IViewManagementModel viewManagementModel,
                                IStrandApiService strandApiService,
                                IEventAggregator eventAggregator,
                                IFemModelParameters femModelParameters,
                                IFemModelDisplayService femModelDisplayService,
                                IDesignModel designModel,
                                INotificationService notificationService)
        : base(viewManagementModel, femModelDisplayService, FemModels.WindLoadingDisplayModelId)
    {
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));

        _windViewLoadEvent = eventAggregator.GetEvent<WindViewLoadEvent>();
        _windViewLoadEvent.Subscribe(GetLoadCases);

        SelectedLoadDirection = LoadDirections[0];
    }

    private void GetLoadCases(string filePath)
    {
        try
        {
            FilePath = filePath;

            IsModelOpen = true;

            UpdateFemModelView();

            LoadCases.SetRange(_strandApiService.GetPrimaryLoadCases(_modelId));
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfError(new Notification("Error", ex.Message));
        }
    }

    [RelayCommand]
    private void LoadDirectionChanged()
    {
        if (SelectedLoadDirection == "X")
            windLoadVector = [1, 0, 0];
        else if (SelectedLoadDirection == "Y")
            windLoadVector = [0, 1, 0];
        else if (SelectedLoadDirection == "Z")
            windLoadVector = [0, 0, 1];
        else if (SelectedLoadDirection == "-X")
            windLoadVector = [-1, 0, 0];
        else if (SelectedLoadDirection == "-Y")
            windLoadVector = [0, -1, 0];
        else if (SelectedLoadDirection == "-Z")
            windLoadVector = [0, 0, -1];

        WindLoadingModel.SetVector(windLoadVector);
    }

    [RelayCommand]
    private void ApplyWindLoad()
    {
        if (SelectedLoadCase == null)
            return;

        _strandApiService.GetFemModelParameters(_femModelParameters, _designModel.DesignCode.ToDesignCodeEnum(), _modelId, _designModel.SolverType, null);

        _strandApiService.ApplyBeamWindLoads(_modelId, SelectedLoadCase.Number, windLoadVector, WindLoadingModel, _femModelParameters.Beams, _femModelParameters.UnitFactor);
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await CloseRightDrawer();
    }
}