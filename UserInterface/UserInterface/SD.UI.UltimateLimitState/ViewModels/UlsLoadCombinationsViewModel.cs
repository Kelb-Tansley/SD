using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Strand.Models;
using SD.Data.Interfaces;
using SD.Element.Design.AS.Enums;
using SD.Element.Design.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.Fem.Strand7.Services;
using SD.UI.Constants;
using SD.UI.Enums;
using SD.UI.Events;
using SD.UI.Models;
using SD.UI.ViewModel;

namespace SD.UI.UltimateLimitState.ViewModels;
public partial class UlsLoadCombinationsViewModel : LoadCasesViewModelBase
{
    private readonly IDesignCodeAdapter _femDesignAdapter;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IDesignModel _designModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly INotificationService _notificationService;
    private readonly IFemFilePathService _femFilePathService;
    private readonly IEffectiveLengthService _effectiveLengthService;
    private readonly IStrandApiService _strandApiService;
    private readonly RefreshEvent _refreshEvent;
    private readonly RefreshCalculationEvent _refreshCalculationEvent;
    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly RunUlsSolverEvent _runUlsSolverEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;
    private LastEventEnum _lastEventEnum;
    private bool _isMainModelResultsOpen;

    public UlsLoadCombinationsViewModel(IProcessModel processModel,
                                        IDesignModel designModel,
                                        IFemModel femModel,
                                        IDesignCodeAdapter femDesignAdapter,
                                        IEventAggregator eventAggregator,
                                        IFemFilePathService femFilePathService,
                                        IFemModelDisplayService femModelDisplayService,
                                        IFemModelParameters femModelParameters,
                                        INotificationService notificationService,
                                        IStrandApiService strandApiService,
                                        IEffectiveLengthService effectiveLengthService) : base(processModel)
    {
        _processModel = processModel;
        _designModel = designModel;
        _femModel = femModel;
        _eventAggregator = eventAggregator;
        _femDesignAdapter = femDesignAdapter;
        _femModelDisplayService = femModelDisplayService;
        _femModelParameters = femModelParameters;
        _notificationService = notificationService;
        _femFilePathService = femFilePathService ?? throw new ArgumentNullException(nameof(femFilePathService));
        _effectiveLengthService = effectiveLengthService;
        _strandApiService = strandApiService;

        _refreshEvent = _eventAggregator.GetEvent<RefreshEvent>();
        _refreshCalculationEvent = _eventAggregator.GetEvent<RefreshCalculationEvent>();
        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _runUlsSolverEvent = _eventAggregator.GetEvent<RunUlsSolverEvent>();
        _designCodeChangedEvent = _eventAggregator.GetEvent<DesignCodeChangedEvent>();

        _fileOpenedEvent.Subscribe(async () => await Strand7FileOpened());
        _refreshEvent.Subscribe(async () => await Refresh());
        _refreshCalculationEvent.Subscribe(async () => await RefreshCalculation());
        _runUlsSolverEvent.Subscribe(async () => await UpdateAndRunUlsSolver());
        _designCodeChangedEvent.Subscribe(DesignCodeChanged);

        DesignCodeChanged();
    }

    [ObservableProperty]
    public required IFemModelParameters _femModelParameters;

    [ObservableProperty]
    public BeamAxisDisplay beamAxisDisplay;

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
                _femModelDisplayService.ReloadFemDisplayModel(FemModels.ModelId, _femModel.FileName, true);
                _isMainModelResultsOpen = false;

                femModelOpened = await TryLoadFemModelProperties();
                if (femModelOpened)
                {
                    _femModelDisplayService.OpenFemFile(FemModels.DisplayModelId, _femModel.FileName, true);

                    UpdateLoadCombinations(FemModelParameters.LoadCaseCombinations);
                    _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, _designModel.IsDesignLengthCalculated, FemModelParameters, _designModel.DesignSettings);
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
            _ = await _femFilePathService.AddUpdateFilePathsAsync(_femModel.FileName);
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

    protected override async Task SelectedLoadCombinationsChanged()
    {
        try
        {
            await SetPrimaryProcess(false, true, true);
            if (LoadCaseCombinations == null)
                return;

            var selectedLoadCaseNumbers = LoadCaseCombinations.Where(sll => sll.Include)?.Select(sll => sll.Number);
            await UpdateAndRunUlsSolver();
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            await SetPrimaryProcess(true, true, true);
        }
    }

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
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, _designModel.IsDesignLengthCalculated, FemModelParameters, _designModel.DesignSettings);

        switch (_lastEventEnum)
        {
            case LastEventEnum.LengthChanged:
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

    [RelayCommand]
    private async Task DesignContourChanged()
    {
        try
        {
            if (BeamAxisDisplay.SelectedDesignLength == null)
            {
                BeamAxisDisplay.SelectedDesignLength = BeamAxisDisplay.DesignLengths.First();
            }

            switch (BeamAxisDisplay.SelectedDesignLength?.ResultType)
            {
                case ResultType.BeamLength:
                    await _femModelDisplayService.DisplayDesignLengths(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, BeamAxisDisplay.SelectedDesignLength.BeamAxis);
                    break;
                case ResultType.Slenderness:
                    await _femModelDisplayService.DisplayDesignSlenderness(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, BeamAxisDisplay.SelectedDesignLength.BeamAxis);
                    break;
                default:
                    break;
            }
        }
        catch (Exception) { throw; }
        finally
        {
            _lastEventEnum = LastEventEnum.LengthChanged;
        }
    }

    private void DesignCodeChanged()
    {
        BeamAxisDisplay = new(_designModel.DesignCode);
    }

    private async Task UpdateAndRunUlsSolver()
    {
        try
        {
            await SetPrimaryProcess(false, true, true);

            var hasSelectedItem = AssignLoadCaseCombinationsToRun();
            if (hasSelectedItem)
            {
                if (!_isMainModelResultsOpen)
                    _femModelDisplayService.OpenFemResultsFile(FemModels.ModelId, _femModel.FileName);

                var results = await _femDesignAdapter.GetDesignService(_designModel.DesignCode.ToDesignCodeEnum()).RunUlsDesign(FemModels.ModelId, FemModelParameters?.Beams?.ToList());

                await _femModelDisplayService.DisplayDesignResults(FemModels.DisplayModelId, _femModel.FileName, _femModel.ModelHandle, results);
            }

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
        if (selectedLoadCases == null || selectedLoadCases.Count == 0)
        {
            FemModelParameters.LoadCaseCombinations?.ToList()?.ForEach(lcc => lcc.Include = false);
            return false;
        }

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