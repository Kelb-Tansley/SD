using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Loading;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;

namespace SD.UI.Loading.ViewModels;

public partial class WindLoadingViewModel : FemViewModelBase
{
    private readonly IFemModelParameters _femModelParameters;
    private readonly IStrandApiService _strandApiService;

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
                                IFemModelParameters femModelParameters) : base(viewManagementModel)
    {
        _strandApiService = strandApiService;
        _femModelParameters = femModelParameters;

        _windViewLoadEvent = eventAggregator.GetEvent<WindViewLoadEvent>();
        _windViewLoadEvent.Subscribe(GetLoadCases);

        SelectedLoadDirection = LoadDirections[0];
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
        _strandApiService.ApplyBeamWindLoads(FemModels.ModelId, SelectedLoadCase.Number, windLoadVector, WindLoadingModel, _femModelParameters.Beams, _femModelParameters.UnitFactor);
    }

    private void GetLoadCases()
    {
        LoadCases.SetRange(_strandApiService.GetPrimaryLoadCases(FemModels.ModelId));
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await CloseRightDrawer();
    }
}
