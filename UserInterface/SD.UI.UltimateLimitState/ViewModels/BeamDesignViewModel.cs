using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.AS;
using SD.Core.Shared.Models.Sans;
using SD.Element.Design.Interfaces;
using SD.MathcadPrime.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SD.UI.UltimateLimitState.ViewModels;

public partial class BeamDesignViewModel : ViewModelBase
{
    private readonly IDesignCodeAdapter _femDesignAdapter;
    private readonly IFemModel _femModel;
    private readonly IDesignModel _designModel;
    private readonly IAsMathcadService _asMathcadService;
    private readonly ISansMathcadService _sansMathcadService;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly INotificationService _notificationService;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IEventAggregator _eventAggregator;

    private readonly BeamDesignWindowClosedEvent _beamDesignWindowClosedEvent;
    private readonly SelectedBeamChangedEvent _selectedBeamChangedEvent;
    private readonly LoadCaseChangedEvent _loadCaseChangedEvent;
    private readonly DesignFemResizeEvent _designFemResizeEvent;
    private readonly SelectDataTabEvent _selectDataTabEvent;

    private BackgroundWorker? _exportWorker;

    [ObservableProperty]
    public partial DesignCode SelectedDesignCode { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<UlsResult> DisplayedResults { get; set; } = [];

    [ObservableProperty]
    public partial UlsResult? SelectedBeamResult { get; set; }

    [ObservableProperty]
    public partial bool HasSelectedLoadCaseCombination { get; set; }

    [ObservableProperty]
    public partial bool HasUlsCalcUpdates { get; set; }

    [ObservableProperty]
    public partial BeamFilterViewModel BeamFilterViewModel { get; set; }

    public BeamDesignViewModel(IFemModel femModel,
                               IDesignModel designModel,
                               IAsMathcadService asMathcadService,
                               ISansMathcadService sansMathcadService,
                               IFemModelDisplayService femModelDisplayService,
                               IFemModelParameters femModelParameters,
                               IProcessModel processModel,
                               IUlsDesignResults ulsDesignResults,
                               INotificationService notificationService,
                               IDesignCodeAdapter femDesignAdapter,
                               IEventAggregator eventAggregator) : base(processModel)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _asMathcadService = asMathcadService ?? throw new ArgumentNullException(nameof(asMathcadService));
        _sansMathcadService = sansMathcadService ?? throw new ArgumentNullException(nameof(sansMathcadService));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _femDesignAdapter = femDesignAdapter ?? throw new ArgumentNullException(nameof(femDesignAdapter));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

        _beamDesignWindowClosedEvent = _eventAggregator.GetEvent<BeamDesignWindowClosedEvent>();
        _selectedBeamChangedEvent = _eventAggregator.GetEvent<SelectedBeamChangedEvent>();
        _loadCaseChangedEvent = _eventAggregator.GetEvent<LoadCaseChangedEvent>();
        _designFemResizeEvent = _eventAggregator.GetEvent<DesignFemResizeEvent>();
        _selectDataTabEvent = _eventAggregator.GetEvent<SelectDataTabEvent>();

        _beamDesignWindowClosedEvent.Subscribe(BeamDesignWindowClosed);
        _loadCaseChangedEvent.Subscribe(async () => await LoadCaseChanged());
        _eventAggregator.GetEvent<FileClosedEvent>().Subscribe(ClearProperties);
        _eventAggregator.GetEvent<DesignCodeChangedEvent>().Subscribe(DesignCodeChanged);
        _eventAggregator.GetEvent<SelectUlsTabEvent>().Subscribe(SetBeamResult);

        InitializeBackgroundWorker();

        BeamFilterViewModel = new BeamFilterViewModel(_femModel,
                                                      femModelDisplayService,
                                                      notificationService,
                                                      ulsDesignResults,
                                                      femModelParameters,
                                                      SetBeamNumberCommand,
                                                      BeamSelectionTypeChangedCommand);
    }

    private void SetBeamResult(UlsResult result)
    {
        if (result != null)
            SelectedBeamResult = DisplayedResults.FirstOrDefault(res => res.Beam.Number == result.Beam.Number && res.LoadCaseNumber == result.LoadCaseNumber);
    }

    private void DesignCodeChanged()
    {
        SelectedDesignCode = _designModel.DesignCode.ToDesignCodeEnum();
        ClearProperties();
    }

    private void BeamDesignWindowClosed()
    {
        _beamDesignWindowClosedEvent.Unsubscribe(BeamDesignWindowClosed);
        _loadCaseChangedEvent.Unsubscribe(async () => await LoadCaseChanged());
    }

    [RelayCommand]
    private void ShowInAnotherView()
    {
        if (SelectedBeamResult != null)
            _selectDataTabEvent.Publish(SelectedBeamResult);
    }

    [RelayCommand]
    private async Task Loaded()
    {
        CheckIfHasSelectedLoadCaseCombination();
        CheckIfHasSelectedBeams();
    }

    [RelayCommand]
    private void GridSizeChanged()
    {
        _designFemResizeEvent.Publish();
    }

    [RelayCommand]
    private async Task SetBeamNumber(double beamNumber)
    {
        await SetPrimaryProcess();

        var displayedBeams = new ObservableCollection<UlsResult>();
        displayedBeams.AddRange(_ulsDesignResults.SansUlsResults?.Where(res => res.Beam.Number == beamNumber).ToList());

        DisplayedResults.SetRange(displayedBeams);

        await SetPrimaryProcess(true);
    }

    private async Task LoadCaseChanged()
    {
        CheckIfHasSelectedLoadCaseCombination();
        await BeamSelectionTypeChanged();
    }

    private void CheckIfHasSelectedLoadCaseCombination()
    {
        HasSelectedLoadCaseCombination = _femModelParameters.LoadCaseCombinations?.FirstOrDefault(lcc => lcc.Include) != null;
    }

    private void CheckIfHasSelectedBeams()
    {
        var calculatedResults = _ulsDesignResults.GetUlsResults();
        BeamFilterViewModel.HasSelectedBeams = calculatedResults == null ? false : calculatedResults.Where(res => res.Beam.IsSelected).ToList().Count > 0;
    }

    [RelayCommand]
    public void SetChainKValues()
    {
        HasUlsCalcUpdates = true;
    }

    [RelayCommand]
    public async Task ReCalculate()
    {
        if (SelectedBeamResult == null)
            return;

        var updates = DisplayedResults.Where(res => res.Beam.BeamChain.ValuesChanged)?.Select(res => res.Beam)?.Distinct()?.ToList();
        if (updates != null && updates.Count > 0)
        {
            await _femDesignAdapter.GetDesignService(_designModel.DesignCode.ToDesignCodeEnum()).RunUlsDesignUpdate(FemModels.ModelId, updates);

            updates.ForEach(update => update.BeamChain.ValuesChanged = false);

            await BeamSelectionTypeChanged(false);
        }
        else
            HasUlsCalcUpdates = false;
    }

    [RelayCommand]
    private void SelectedBeamResultChanged()
    {
        _selectedBeamChangedEvent.Publish(SelectedBeamResult);
    }

    [RelayCommand]
    private async Task BeamSelectionTypeChanged(bool showLoader = true)
    {
        if (showLoader)
            await SetPrimaryProcess(longerDelay: true);

        try
        {
            var displayedBeams = BeamFilterViewModel.FilterUlsDisplayedResults();

            var previousBeam = SelectedBeamResult?.Beam?.Number;
            var previousLoadCase = SelectedBeamResult?.LoadCaseNumber;

            DisplayedResults.SetRange(displayedBeams);

            if (SelectedBeamResult == null && previousBeam != null && previousLoadCase != null)
                SelectedBeamResult = DisplayedResults.FirstOrDefault(res => res.Beam.Number == previousBeam && res.LoadCaseNumber == previousLoadCase);
            else
                SelectedBeamResult ??= DisplayedResults.FirstOrDefault();

            HasUlsCalcUpdates = DisplayedResults.FirstOrDefault(res => res.Beam.BeamChain.ValuesChanged == true) != null;
        }
        catch (ArgumentNullException)
        {
            DisplayedResults.Clear();
        }
        finally
        {
            if (showLoader)
                await SetPrimaryProcess(true);
        }

        SelectedBeamResultChanged();
    }

    private void InitializeBackgroundWorker()
    {
        _exportWorker = new BackgroundWorker();
        _exportWorker.DoWork += MathcadExportWorkerDoWork!;
        _exportWorker.RunWorkerCompleted += MathcadExportWorkerRunWorkerCompleted!;
    }

    [RelayCommand]
    public void MathcadExport()
    {
        if (SelectedBeamResult == null || _exportWorker!.IsBusy)
        {
            _notificationService.ShowSnackNotification(new Notification("Cancelled", "Export process is currently busy..."));
            return;
        }

        _exportWorker.RunWorkerAsync();
    }

    [RelayCommand]
    public void ExcelExport()
    {
        try
        {
            _femDesignAdapter.GetExportResultsService(SelectedDesignCode).ExportToExcel(DisplayedResults.ToList());
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfError(new Notification("Error", ex.Message));
        }
    }

    private void MathcadExportWorkerDoWork(object sender, DoWorkEventArgs e)
    {
        if (SelectedBeamResult == null)
            return;

        _notificationService.ShowSnackNotification(new Notification("Saving", $"Saving Mathcad worksheet..."));

        if (SelectedBeamResult.DesignCode == DesignCode.SANS)
        {
            _sansMathcadService.ExportToMathcadFile(null, (SansUlsResult)SelectedBeamResult, true);
            e.Result = "Exported to SANS Mathcad file";
        }
        else if (SelectedBeamResult.DesignCode == DesignCode.AS)
        {
            _asMathcadService.ExportToMathcadFile(null, (ASUlsResult)SelectedBeamResult, true);
            e.Result = "Exported to AS Mathcad file";
        }
    }

    private void MathcadExportWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
            _notificationService.NotifyUserOfError(new Notification("Error", e.Error.Message));
        else if (e.Cancelled)
            _notificationService.ShowSnackNotification(new Notification("Cancelled", "Export was cancelled."));
    }

    private void ClearProperties()
    {
        DisplayedResults.Clear();
        SelectedBeamResult = null;
        HasSelectedLoadCaseCombination = false;
    }
}