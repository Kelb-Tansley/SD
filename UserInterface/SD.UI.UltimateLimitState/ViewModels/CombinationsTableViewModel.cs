using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.Data.Interfaces;
using SD.Element.Design.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.Enums;
using SD.UI.Events;
using SD.UI.Services;
using SD.UI.ViewModel;
using System.ComponentModel;

namespace SD.UI.UltimateLimitState.ViewModels;

public partial class CombinationsTableViewModel : LoadCasesViewModelBase
{
    private readonly IDesignCodeAdapter _femDesignAdapter;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IDesignModel _designModel;
    private readonly IFemModel _femModel;
    private readonly IBeamAxisDisplay _beamAxisDisplay;
    private readonly INotificationService _notificationService;
    private readonly IFemFilePathBlobService _femFilePathService;
    private readonly IEffectiveLengthService _effectiveLengthService;
    private readonly IStrandApiService _strandApiService;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IBeamKFactorService _beamKFactorService;

    private readonly RefreshEvent _refreshEvent;
    private readonly RefreshCalculationEvent _refreshCalculationEvent;
    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly RunUlsSolverEvent _runUlsSolverEvent;
    private readonly DesignContourChangedEvent _designContourChangedEvent;
    private readonly CalculateEvent _calculateEvent;

    private LastEventEnum _lastEventEnum;
    private bool _isMainModelResultsOpen;

    private readonly SemaphoreSlim _reloadSemaphore = new(1, 1);

    public CombinationsTableViewModel(IProcessModel processModel,
                                      IDesignModel designModel,
                                      IFemModel femModel,
                                      IDesignCodeAdapter femDesignAdapter,
                                      IEventAggregator eventAggregator,
                                      IFemFilePathBlobService femFilePathService,
                                      IFemModelDisplayService femModelDisplayService,
                                      IFemModelParameters femModelParameters,
                                      INotificationService notificationService,
                                      IStrandApiService strandApiService,
                                      IEffectiveLengthService effectiveLengthService,
                                      IBeamAxisDisplay beamAxisDisplay,
                                      IBeamKFactorService beamKFactorService,
                                      IUlsDesignResults ulsDesignResults) : base(processModel, eventAggregator)
    {
        _designModel = designModel;
        _femModel = femModel;
        _beamAxisDisplay = beamAxisDisplay;
        _femDesignAdapter = femDesignAdapter;
        _femModelDisplayService = femModelDisplayService;
        FemModelParameters = femModelParameters;
        _notificationService = notificationService;
        _femFilePathService = femFilePathService ?? throw new ArgumentNullException(nameof(femFilePathService));
        _effectiveLengthService = effectiveLengthService;
        _strandApiService = strandApiService;
        _ulsDesignResults = ulsDesignResults;
        _beamKFactorService = beamKFactorService;

        _refreshEvent = _eventAggregator.GetEvent<RefreshEvent>();
        _refreshCalculationEvent = _eventAggregator.GetEvent<RefreshCalculationEvent>();
        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _runUlsSolverEvent = _eventAggregator.GetEvent<RunUlsSolverEvent>();
        _designContourChangedEvent = _eventAggregator.GetEvent<DesignContourChangedEvent>();
        _calculateEvent = _eventAggregator.GetEvent<CalculateEvent>();

        _fileOpenedEvent.Subscribe(async () => await Strand7FileOpened());
        _refreshEvent.Subscribe(async () => await Refresh());
        _refreshCalculationEvent.Subscribe(async () => await RefreshCalculation());
        _runUlsSolverEvent.Subscribe(async () => await UpdateAndRunUlsSolver());
        _designContourChangedEvent.Subscribe(async () => await DesignContourChanged());
        _calculateEvent.Subscribe(async () => await Calculate());
    }

    [ObservableProperty]
    public required partial IFemModelParameters FemModelParameters { get; set; }

    [RelayCommand]
    private async Task LoadCaseChanged()
    {
        await UpdateAndRunUlsSolver();
    }

    private async Task Strand7FileOpened()
    {
        var femModelOpened = false;
        try
        {
            await SetPrimaryProcess();

            if (!string.IsNullOrWhiteSpace(_femModel.FileName))
            {
                await _reloadSemaphore.RunInBackgroundAsync(() => _femModelDisplayService.ReloadFemDisplayModel(FemModels.ModelId, _femModel.FileName, true));

                _isMainModelResultsOpen = false;

                femModelOpened = await TryLoadFemModelProperties();
                if (femModelOpened)
                {
                    await _reloadSemaphore.RunInBackgroundAsync(() => _femModelDisplayService.OpenFemFile(FemModels.DisplayModelId, _femModel.FileName, true));

                    UpdateLoadCombinations(FemModelParameters.LoadCaseCombinations);

                    await GetEffectiveLengths();

                    await DesignContourChanged();

                    // Publish the event to notify the application that the FEM model has been loaded.
                    _eventAggregator.GetEvent<FemLoadedEvent>().Publish();
                }
            }
        }
        catch (Exception ex)
        {
            femModelOpened = false;
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            await SetPrimaryProcess(true);
            ProcessModel.IsFemModelLoaded = femModelOpened;
            _femModel.FileExists = femModelOpened;
            _isMainModelResultsOpen = femModelOpened;
        }
    }

    private async Task GetEffectiveLengths()
    {
        var designLengthsTask = Task.Run(() =>
            _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, _designModel.IsDesignLengthCalculated, FemModelParameters, _designModel.DesignSettings));

        var beamKFactorsTask = _beamKFactorService.GetBeamKValuesByFileName(_femModel.FileName, FemModelParameters.Beams);

        await Task.WhenAll(designLengthsTask, beamKFactorsTask);
    }

    /// <summary>
    /// Attempts to load the properties of the FEM model. If the load fails, the user is prompted to run the solver. If the user chooses to run the solver, the method will recursively call itself.
    /// If the user chooses not to run the solver, the method will navigate back to the file browser view.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the FEM model properties were successfully loaded.</returns>
    private async Task<bool> TryLoadFemModelProperties()
    {
        var result = _femModelDisplayService.LoadFemModelProperties(FemModels.ModelId, _designModel.DesignCode.ToDesignCodeEnum(), _femModel.FileName);

        bool femModelOpened = result.IsSuccess;
        if (femModelOpened)
        {
            _ = await _femFilePathService.AddUpdateFilePathsAsync(_femModel.FileName);
            SubscribeToPropertyChangedEvents();
        }
        else
        {
            var userChoice = _notificationService.NotifyUserWithYesNoOption(new Notification("Error", result.Message));
            if (userChoice == System.Windows.MessageBoxResult.Yes)
            {
                _strandApiService.RunLinearStaticAnalysis(FemModels.ModelId);
                return await TryLoadFemModelProperties();
            }
            else
            {
                // If the fem model properties fail to load and the user rejects the option to run solver, then navigate back to the file browser view.
                _fileClosedEvent.Publish();
                return false;
            }
        }

        return femModelOpened;
    }

    private void SubscribeToPropertyChangedEvents()
    {
        foreach (var beam in FemModelParameters.Beams)
        {
            UnsubscribeFromBeam(beam);
            SubscribeToBeam(beam);
        }
        void UnsubscribeFromBeam(Beam beam) => beam.BeamChain?.PropertyChanged -= OnBeamChainPropertyChanged;
        void SubscribeToBeam(Beam beam) => beam.BeamChain?.PropertyChanged += OnBeamChainPropertyChanged;
        void OnBeamChainPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is BeamChain chain && chain.ValuesChanged)
                _eventAggregator?.GetEvent<KValuesChangedEvent>()?.Publish(true);
        }
    }

    protected override async Task SelectedLoadCombinationsChanged() => await UpdateAndRunUlsSolver();

    private async Task Refresh()
    {
        var femModelOpened = false;
        try
        {
            if (!string.IsNullOrWhiteSpace(_femModel.FileName))
            {
                await SetPrimaryProcess();
                _femModelDisplayService.ReloadFemDisplayModel(FemModels.DisplayModelId, _femModel.FileName, true);

                femModelOpened = await TryLoadFemModelProperties();
                if (femModelOpened)
                {
                    await RefreshCalculation();
                }
            }
            else
                femModelOpened = false;
        }
        catch (Exception ex)
        {
            femModelOpened = false;
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            ProcessModel.IsFemModelLoaded = femModelOpened;
            await SetPrimaryProcess(true);
        }
    }

    private async Task RefreshCalculation()
    {
        await GetEffectiveLengths();

        switch (_lastEventEnum)
        {
            case LastEventEnum.ContourChanged:
                {
                    _femModelDisplayService.ReloadFemDisplayModel(FemModels.ModelId, _femModel.FileName, true);
                    _isMainModelResultsOpen = false;

                    await DesignContourChanged();
                    break;
                }
            case LastEventEnum.LoadCaseChanged:
                {
                    await LoadCaseChanged();
                    break;
                }
            default:
                break;
        }
    }

    private async Task DesignContourChanged()
    {
        try
        {
            if (_beamAxisDisplay.SelectedDesignableBeam is not null)
                await _femModelDisplayService.DisplayDesignableBeams(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle);
            else if (_beamAxisDisplay.SelectedDesignLength is not null)
                await _femModelDisplayService.DisplayDesignLengths(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, _beamAxisDisplay.SelectedDesignLength.BeamAxis);
            else if (_beamAxisDisplay.SelectedSlendernessOrientation is not null)
                await _femModelDisplayService.DisplayDesignSlenderness(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, _beamAxisDisplay.SelectedSlendernessOrientation.BeamAxis);
            else if (_beamAxisDisplay.SelectedKFactor is not null)
                await _femModelDisplayService.DisplayDesignKFactors(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, _beamAxisDisplay.SelectedKFactor.BeamAxis);
            else if (_beamAxisDisplay.SelectedUlsUtilizationType is not null)
                await _femModelDisplayService.DisplaySansDesignResults(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, _ulsDesignResults.SansUlsResults, _beamAxisDisplay.SelectedUlsUtilizationType.SansUtilizationType);
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfError(new Notification("Error", ex.Message));
        }
        finally
        {
            _lastEventEnum = LastEventEnum.ContourChanged;
        }
    }

    private async Task UpdateAndRunUlsSolver()
    {
        try
        {
            var hasSelectedItem = AssignLoadCaseCombinationsToRun();
            if (!hasSelectedItem)
            {
                _notificationService.ShowSnackNotification(new ShortNotification("No load case combination selected."));
                return;
            }

            await SetPrimaryProcess(false, true, true);

            if (!_isMainModelResultsOpen)
                _femModelDisplayService.OpenFemResultsFile(FemModels.ModelId, _femModel.FileName);

            await _femDesignAdapter.GetDesignService(_designModel.DesignCode.ToDesignCodeEnum()).RunUlsDesign(FemModels.ModelId, FemModelParameters?.Beams?.ToList());

            await DesignContourChanged();

            _eventAggregator.GetEvent<LoadCaseChangedEvent>().Publish();
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            _lastEventEnum = LastEventEnum.LoadCaseChanged;
            await SetPrimaryProcess(true, true, true);
        }
    }

    private bool AssignLoadCaseCombinationsToRun()
    {
        var selectedLoadCases = LoadCaseCombinations?.Where(lcc => lcc.Include)?.ToList();
        FemModelParameters.LoadCaseCombinations?.ToList()?.ForEach(lcc => lcc.Include = false);
        if (selectedLoadCases == null || selectedLoadCases.Count == 0)
            return false;

        var hasSelectedItem = false;
        foreach (var combination in selectedLoadCases)
        {
            var match = FemModelParameters.LoadCaseCombinations?.FirstOrDefault(lcc => lcc.Number == combination.Number);
            if (match != null)
            {
                match.Include = true;
                hasSelectedItem = true;
            }
        }
        return hasSelectedItem;
    }
}