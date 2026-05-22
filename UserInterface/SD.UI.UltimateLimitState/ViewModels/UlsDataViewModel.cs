using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Sans;
using SD.Element.Design.Interfaces;
using SD.UI.Events;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SD.UI.UltimateLimitState.ViewModels;

public partial class UlsDataViewModel : ObservableObject
{
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IUlsDataExportService _ulsDdataExportService;

    private readonly LoadCaseChangedEvent _loadCaseChangedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;
    private readonly RefreshCalculationEvent _refreshCalculationEvent;
    private readonly SelectUlsTabEvent _selectTabEvent;

    private bool _isRefreshing;
    private HashSet<int> _selectedBeamNumbers = [];
    private HashSet<int> _selectedLoadCases = [];
    private HashSet<string> _selectedSections = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _selectedReasons = new(StringComparer.OrdinalIgnoreCase);

    private ICollectionView? _sansRowsView;
    public ICollectionView? SansRowsView
    {
        get => _sansRowsView;
        private set => SetProperty(ref _sansRowsView, value);
    }

    [ObservableProperty]
    public partial SansUlsResult? SelectedSansRow { get; set; }

    [ObservableProperty]
    private partial List<SansUlsResult> SansRows { get; set; } = [];

    // Filter popup open states
    [ObservableProperty]
    public partial bool IsBeamFilterOpen { get; set; }

    [ObservableProperty]
    public partial bool IsLoadCaseFilterOpen { get; set; }

    [ObservableProperty]
    public partial bool IsSectionFilterOpen { get; set; }

    [ObservableProperty]
    public partial bool IsReasonFilterOpen { get; set; }

    // Active filter indicators (true = filter is narrowing results)
    [ObservableProperty]
    public partial bool HasBeamFilter { get; set; }
    [ObservableProperty]
    public partial bool HasLoadCaseFilter { get; set; }

    [ObservableProperty]
    public partial bool HasSectionFilter { get; set; }

    [ObservableProperty]
    public partial bool HasReasonFilter { get; set; }

    private List<ColumnFilterOption<int>> BeamFilterOptions { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ColumnFilterOption<int>> LoadCaseFilterOptions { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ColumnFilterOption<string>> SectionFilterOptions { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ColumnFilterOption<string>> ReasonFilterOptions { get; set; } = [];

    [ObservableProperty]
    public partial BeamFilterViewModel BeamFilterViewModel { get; set; }

    public bool HasData => SansRows is not null && SansRows.Count > 0;
    public int DisplayedRowCount => SansRowsView?.Cast<object>().Count() ?? 0;

    public UlsDataViewModel(IFemModel femModel,
                            IFemModelDisplayService femModelDisplayService,
                            IFemModelParameters femModelParameters,
                            INotificationService notificationService,
                            IUlsDesignResults ulsDesignResults,
                            IEventAggregator eventAggregator,
                            IUlsDataExportService ulsDdataExportService)
    {
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _ulsDdataExportService = ulsDdataExportService ?? throw new ArgumentNullException(nameof(ulsDdataExportService));

        _loadCaseChangedEvent = eventAggregator.GetEvent<LoadCaseChangedEvent>();
        _fileClosedEvent = eventAggregator.GetEvent<FileClosedEvent>();
        _designCodeChangedEvent = eventAggregator.GetEvent<DesignCodeChangedEvent>();
        _refreshCalculationEvent = eventAggregator.GetEvent<RefreshCalculationEvent>();
        _selectTabEvent = eventAggregator.GetEvent<SelectUlsTabEvent>();

        _loadCaseChangedEvent.Subscribe(async () => await RefreshRowsAsync());
        _designCodeChangedEvent.Subscribe(async () => await RefreshRowsAsync());
        _refreshCalculationEvent.Subscribe(async () => await RefreshRowsAsync());
        _fileClosedEvent.Subscribe(ClearRows);

        eventAggregator.GetEvent<SelectDataTabEvent>().Subscribe(SetBeamResult);

        BeamFilterViewModel = new BeamFilterViewModel(femModel,
                                                      femModelDisplayService,
                                                      notificationService,
                                                      ulsDesignResults,
                                                      femModelParameters,
                                                      SetBeamNumberCommand,
                                                      BeamSelectionTypeChangedCommand);

        Task.Run(RefreshRowsAsync).Await();
    }

    private void SetBeamResult(UlsResult result)
    {
        if (result is not null)
            SelectedSansRow = SansRows?.FirstOrDefault(r => r.Beam.Number == result.Beam.Number && r.LoadCaseNumber == result.LoadCaseNumber);
    }

    partial void OnSansRowsChanged(List<SansUlsResult> value)
    {
        SansRowsView = CollectionViewSource.GetDefaultView(value);
        if (SansRowsView != null)
            SansRowsView.Filter = FilterSansRow;

        OnPropertyChanged(nameof(HasData));
        OnPropertyChanged(nameof(DisplayedRowCount));
        // RefreshFilters is called once at the end of RefreshRowsAsync after both rows
        // and filter hash-sets are fully rebuilt — not here to avoid double-firing.
    }

    private void RefreshFilters()
    {
        OnSansRowsChanged(SansRows);
        SansRowsView?.Refresh();

        HasBeamFilter = BeamFilterOptions.Any(o => !o.IsSelected);
        HasLoadCaseFilter = LoadCaseFilterOptions.Any(o => !o.IsSelected);
        HasSectionFilter = SectionFilterOptions.Any(o => !o.IsSelected);
        HasReasonFilter = ReasonFilterOptions.Any(o => !o.IsSelected);
    }

    private void RebuildSelectedSets()
    {
        _selectedBeamNumbers = BeamFilterOptions.Where(o => o.IsSelected).Select(o => o.Value).ToHashSet();
        _selectedLoadCases = LoadCaseFilterOptions.Where(o => o.IsSelected).Select(o => o.Value).ToHashSet();
        _selectedSections = SectionFilterOptions.Where(o => o.IsSelected).Select(o => o.Value ?? string.Empty).ToHashSet(StringComparer.OrdinalIgnoreCase);
        _selectedReasons = ReasonFilterOptions.Where(o => o.IsSelected).Select(o => o.Value ?? string.Empty).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private void BuildFilterOptions()
    {
        var loadCases = SansRows.Select(r => r.LoadCaseNumber).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<int>(v, v.ToString()) { IsSelected = !HasLoadCaseFilter || _selectedLoadCases.Count == 0 || _selectedLoadCases.Contains(v) })
            .ToList();

        var sections = SansRows.Select(r => r.Beam.Section.DisplayName).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<string>(v, v) { IsSelected = !HasSectionFilter || _selectedSections.Count == 0 || _selectedSections.Contains(v) })
            .ToList();

        var reasons = SansRows.Select(r => r.Utilization.MaxUtilizationDescription).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<string>(v, v) { IsSelected = !HasReasonFilter || _selectedReasons.Count == 0 || _selectedReasons.Contains(v) })
            .ToList();

        SetBeamFilterOptions();
        LoadCaseFilterOptions = [.. loadCases];
        SectionFilterOptions = [.. sections];
        ReasonFilterOptions = [.. reasons];
    }

    [RelayCommand]
    private async Task SetBeamNumber(double beamNumber)
    {
        BeamFilterOptions.ForEach(o => o.IsSelected = false);
        BeamFilterOptions.FirstOrDefault(o => o.Value == beamNumber)?.IsSelected = true;

        ApplyFilters();
    }

    [RelayCommand]
    private async Task BeamSelectionTypeChanged(bool showLoader = true)
    {
        SetBeamFilterOptions();

        ApplyFilters();
    }

    private void SetBeamFilterOptions()
    {
        var displayedBeams = BeamFilterViewModel.FilterUlsDisplayedResults()
                                                .Select(r => r.Beam.Number)
                                                .Distinct()
                                                .ToHashSet();

        var beams = SansRows.Select(r => r.Beam.Number).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<int>(v, v.ToString()) { IsSelected = displayedBeams.Contains(v) })
            .ToList();

        BeamFilterOptions.SetRange(beams);
    }

    [RelayCommand]
    private void SelectAllLoadCaseFilters()
    {
        LoadCaseFilterOptions.ToList().ForEach(o => o.IsSelected = true);
    }

    [RelayCommand]
    private void ClearLoadCaseFilters()
    {
        LoadCaseFilterOptions.ToList().ForEach(o => o.IsSelected = false);
    }

    [RelayCommand]
    private void SelectAllSectionFilters()
    {
        SectionFilterOptions.ToList().ForEach(o => o.IsSelected = true);
    }

    [RelayCommand]
    private void ClearSectionFilters()
    {
        SectionFilterOptions.ToList().ForEach(o => o.IsSelected = false);
    }

    [RelayCommand]
    private void SelectAllReasonFilters()
    {
        ReasonFilterOptions.ToList().ForEach(o => o.IsSelected = true);
    }

    [RelayCommand]
    private void ClearReasonFilters()
    {
        ReasonFilterOptions.ToList().ForEach(o => o.IsSelected = false);
    }

    [RelayCommand]
    private void ApplyFilters()
    {
        RebuildSelectedSets();
        RefreshFilters();

        // Close all filter popups
        IsBeamFilterOpen = false;
        IsLoadCaseFilterOpen = false;
        IsSectionFilterOpen = false;
        IsReasonFilterOpen = false;
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        if (SansRowsView == null)
            return;

        _ulsDdataExportService.ExportToExcel(SansRowsView.Cast<SansUlsResult>());
    }

    [RelayCommand]
    private void ClearAllFilterIcons()
    {
        BeamFilterOptions.ToList().ForEach(o => o.IsSelected = true);
        LoadCaseFilterOptions.ToList().ForEach(o => o.IsSelected = true);
        SectionFilterOptions.ToList().ForEach(o => o.IsSelected = true);
        ReasonFilterOptions.ToList().ForEach(o => o.IsSelected = true);

        RebuildSelectedSets();
        RefreshFilters();

        IsBeamFilterOpen = false;
        IsLoadCaseFilterOpen = false;
        IsSectionFilterOpen = false;
        IsReasonFilterOpen = false;
    }

    [RelayCommand]
    private void ShowInUlsView(SansUlsResult result)
    {
        _selectTabEvent.Publish(result);
    }

    private bool FilterSansRow(object obj)
    {
        if (obj is not SansUlsResult row)
            return false;

        return _selectedBeamNumbers.Contains(row.Beam.Number)
               && _selectedLoadCases.Contains(row.LoadCaseNumber)
               && _selectedSections.Contains(row.Beam.Section.DisplayName ?? string.Empty)
               && _selectedReasons.Contains(row.Utilization.MaxUtilizationDescription ?? string.Empty);
    }

    private async Task RefreshRowsAsync()
    {
        if (_isRefreshing)
            return;

        _isRefreshing = true;
        try
        {
            SansRows = _ulsDesignResults?.SansUlsResults ?? [];

            BuildFilterOptions();
            RebuildSelectedSets();

            RefreshFilters();

            await Task.Yield();
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private void ClearRows()
    {
        SansRows = [];
        BeamFilterOptions = [];
        LoadCaseFilterOptions = [];
        SectionFilterOptions = [];
        ReasonFilterOptions = [];
        IsBeamFilterOpen = false;
        IsLoadCaseFilterOpen = false;
        IsSectionFilterOpen = false;
        IsReasonFilterOpen = false;
        HasBeamFilter = false;
        HasLoadCaseFilter = false;
        HasSectionFilter = false;
        HasReasonFilter = false;
        _selectedBeamNumbers = [];
        _selectedLoadCases = [];
        _selectedSections = new(StringComparer.OrdinalIgnoreCase);
        _selectedReasons = new(StringComparer.OrdinalIgnoreCase);
    }
}

public partial class ColumnFilterOption<T>(T value, string display) : ObservableObject
{
    public T Value { get; } = value;
    public string Display { get; } = display;

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = true;
}