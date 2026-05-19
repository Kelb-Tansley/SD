using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Sans;
using SD.UI.Events;
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
    private bool _isRefreshing;


    [ObservableProperty]
    private IList<SansUlsResult> sansRows = [];

    private ICollectionView? _sansRowsView;
    public ICollectionView? SansRowsView
    {
        get => _sansRowsView;
        private set => SetProperty(ref _sansRowsView, value);
    }

    [ObservableProperty]
    private bool isLoading;

    // Filter popup open states
    [ObservableProperty]
    private bool isBeamFilterOpen;

    [ObservableProperty]
    private bool isLoadCaseFilterOpen;

    [ObservableProperty]
    private bool isSectionFilterOpen;

    [ObservableProperty]
    private bool isReasonFilterOpen;

    // Active filter indicators (true = filter is narrowing results)
    [ObservableProperty]
    private bool hasBeamFilter;

    [ObservableProperty]
    private bool hasLoadCaseFilter;

    [ObservableProperty]
    private bool hasSectionFilter;

    [ObservableProperty]
    private bool hasReasonFilter;


    [ObservableProperty]
    private ObservableCollection<ColumnFilterOption<int>> beamFilterOptions = [];

    [ObservableProperty]
    private ObservableCollection<ColumnFilterOption<int>> loadCaseFilterOptions = [];

    [ObservableProperty]
    private ObservableCollection<ColumnFilterOption<string>> sectionFilterOptions = [];

    [ObservableProperty]
    private ObservableCollection<ColumnFilterOption<string>> reasonFilterOptions = [];

    private HashSet<int> _selectedBeamNumbers = [];
    private HashSet<int> _selectedLoadCases = [];
    private HashSet<string> _selectedSections = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _selectedReasons = new(StringComparer.OrdinalIgnoreCase);

    public bool HasData => SansRows.Count > 0;
    public int DisplayedRowCount => SansRowsView?.Cast<object>().Count() ?? 0;

    public UlsDataViewModel(IUlsDesignResults ulsDesignResults, IEventAggregator eventAggregator, IUlsDataExportService ulsDdataExportService)
    {
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _ulsDdataExportService = ulsDdataExportService ?? throw new ArgumentNullException(nameof(ulsDdataExportService));

        _loadCaseChangedEvent = eventAggregator.GetEvent<LoadCaseChangedEvent>();
        _fileClosedEvent = eventAggregator.GetEvent<FileClosedEvent>();
        _designCodeChangedEvent = eventAggregator.GetEvent<DesignCodeChangedEvent>();
        _refreshCalculationEvent = eventAggregator.GetEvent<RefreshCalculationEvent>();

        _loadCaseChangedEvent.Subscribe(() => RefreshRowsAsync());
        _designCodeChangedEvent.Subscribe(() => RefreshRowsAsync());
        _refreshCalculationEvent.Subscribe(() => RefreshRowsAsync());
        _fileClosedEvent.Subscribe(ClearRows);

        RefreshRowsAsync();
    }

    partial void OnSansRowsChanged(IList<SansUlsResult> value)
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
        SansRowsView?.Refresh();
        OnPropertyChanged(nameof(DisplayedRowCount));
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
        var beams = SansRows.Select(r => r.Beam.Number).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<int>(v, v.ToString()) { IsSelected = _selectedBeamNumbers.Count == 0 || _selectedBeamNumbers.Contains(v) })
            .ToList();

        var loadCases = SansRows.Select(r => r.LoadCaseNumber).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<int>(v, v.ToString()) { IsSelected = _selectedLoadCases.Count == 0 || _selectedLoadCases.Contains(v) })
            .ToList();

        var sections = SansRows.Select(r => r.Beam.Section.DisplayName).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<string>(v, v) { IsSelected = _selectedSections.Count == 0 || _selectedSections.Contains(v) })
            .ToList();

        var reasons = SansRows.Select(r => r.Utilization.MaxUtilizationDescription).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().OrderBy(v => v)
            .Select(v => new ColumnFilterOption<string>(v, v) { IsSelected = _selectedReasons.Count == 0 || _selectedReasons.Contains(v) })
            .ToList();

        BeamFilterOptions = [.. beams];
        LoadCaseFilterOptions = [.. loadCases];
        SectionFilterOptions = [.. sections];
        ReasonFilterOptions = [.. reasons];
    }

    [RelayCommand]
    private void SelectAllBeamFilters()
    {
        BeamFilterOptions.ToList().ForEach(o => o.IsSelected = true);
    }

    [RelayCommand]
    private void ClearBeamFilters()
    {
        BeamFilterOptions.ToList().ForEach(o => o.IsSelected = false);
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

        HasBeamFilter = BeamFilterOptions.Any(o => !o.IsSelected);
        HasLoadCaseFilter = LoadCaseFilterOptions.Any(o => !o.IsSelected);
        HasSectionFilter = SectionFilterOptions.Any(o => !o.IsSelected);
        HasReasonFilter = ReasonFilterOptions.Any(o => !o.IsSelected);

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

        HasBeamFilter = false;
        HasLoadCaseFilter = false;
        HasSectionFilter = false;
        HasReasonFilter = false;

        IsBeamFilterOpen = false;
        IsLoadCaseFilterOpen = false;
        IsSectionFilterOpen = false;
        IsReasonFilterOpen = false;
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

    private async void RefreshRowsAsync()
    {
        if (_isRefreshing)
            return;

        _isRefreshing = true;
        try
        {
            IsLoading = true;
            await Task.Yield();

            var sansResults = _ulsDesignResults.SansUlsResults ?? [];

            SansRows = sansResults;

            BuildFilterOptions();
            RebuildSelectedSets();

            RefreshFilters();
        }
        finally
        {
            IsLoading = false;
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
        IsLoading = false;
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
    private bool isSelected = true;
}
