using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Models;
using SD.UI.Constants;
using System.Windows.Threading;

namespace SD.UI.ViewModel;

public partial class BeamFilterViewModel : ObservableObject
{
    private readonly IFemModel _femModel;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly INotificationService _notificationService;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IAsyncRelayCommand _setBeamNumber;
    private readonly IAsyncRelayCommand _beamSelectionTypeChanged;

    public BeamFilterViewModel(IFemModel femModel,
                               IFemModelDisplayService femModelDisplayService,
                               INotificationService notificationService,
                               IUlsDesignResults ulsDesignResults,
                               IFemModelParameters femModelParameters,
                               IAsyncRelayCommand setBeamNumber,
                               IAsyncRelayCommand beamSelectionTypeChanged)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _setBeamNumber = setBeamNumber ?? throw new ArgumentNullException(nameof(setBeamNumber));
        _beamSelectionTypeChanged = beamSelectionTypeChanged ?? throw new ArgumentNullException(nameof(beamSelectionTypeChanged));

        InitializeSelectedBeamTimer();
    }

    [ObservableProperty]
    public partial bool AllBeamsChecked { get; set; } = true;

    [ObservableProperty]
    public partial bool DisplayedGroupChecked { get; set; }

    [ObservableProperty]
    public partial bool SelectedBeamsChecked { get; set; }

    [ObservableProperty]
    public partial bool HasSelectedBeams { get; set; }

    [ObservableProperty]
    public partial bool FailedBeamsChecked { get; set; }

    [ObservableProperty]
    public partial double AllowableUlsDesignCapacity { get; set; } = 90;

    [ObservableProperty]
    public partial double BeamNumber { get; set; }

    [RelayCommand]
    private async Task AllBeamsChanged()
    {
        if (!AllBeamsChecked)
            AllBeamsChecked = true;

        SelectedBeamsChecked = !AllBeamsChecked;
        FailedBeamsChecked = !AllBeamsChecked;
        DisplayedGroupChecked = !AllBeamsChecked;

        await _beamSelectionTypeChanged.ExecuteAsync(true);
    }

    [RelayCommand]
    private async Task DisplayedGroupChanged()
    {
        if (!DisplayedGroupChecked)
            DisplayedGroupChecked = true;

        AllBeamsChecked = !DisplayedGroupChecked;
        SelectedBeamsChecked = !DisplayedGroupChecked;
        FailedBeamsChecked = !DisplayedGroupChecked;

        await _beamSelectionTypeChanged.ExecuteAsync(true);
    }

    [RelayCommand]
    private async Task SelectedBeamsChanged()
    {
        if (!SelectedBeamsChecked)
            SelectedBeamsChecked = true;

        AllBeamsChecked = !SelectedBeamsChecked;
        FailedBeamsChecked = !SelectedBeamsChecked;
        DisplayedGroupChecked = !SelectedBeamsChecked;

        await _beamSelectionTypeChanged.ExecuteAsync(true);
    }

    [RelayCommand]
    private async Task FailedBeamsChanged()
    {
        if (!FailedBeamsChecked)
            FailedBeamsChecked = true;

        AllBeamsChecked = !FailedBeamsChecked;
        SelectedBeamsChecked = !FailedBeamsChecked;
        DisplayedGroupChecked = !FailedBeamsChecked;

        await _beamSelectionTypeChanged.ExecuteAsync(true);
    }

    [RelayCommand]
    private async Task SetBeamNumber()
    {
        SelectedBeamsChecked = FailedBeamsChecked = DisplayedGroupChecked = AllBeamsChecked = false;

        await _setBeamNumber.ExecuteAsync(BeamNumber);
    }

    private void InitializeSelectedBeamTimer()
    {
        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler(async (sender, e) => await UpdateFemModelSelectedItems());
        dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
        dispatcherTimer.Start();
    }

    private async Task UpdateFemModelSelectedItems()
    {
        try
        {
            if (_femModel.FileExists)
            {
                var selectionChanged = _femModelDisplayService.SetSelectedBeams(FemModels.DisplayModelId);

                if (selectionChanged && SelectedBeamsChecked)
                    await _beamSelectionTypeChanged.ExecuteAsync(true);
            }
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
    }

    public List<UlsResult> FilterUlsDisplayedResults()
    {
        var displayedBeams = new List<UlsResult>();

        var calculatedResults = _ulsDesignResults.GetUlsResults();
        if (calculatedResults == null)
            return displayedBeams;

        if (AllBeamsChecked)
            displayedBeams.AddRange([.. calculatedResults]);
        else if (SelectedBeamsChecked)
            displayedBeams.AddRange(calculatedResults.Where(res => res.Beam.IsSelected).ToList());
        else if (FailedBeamsChecked)
        {
            var capacity = AllowableUlsDesignCapacity / 100;
            if (capacity >= 0 && calculatedResults != null)
                displayedBeams.AddRange(calculatedResults.Where(res => res.MaxUtilization() != null && res.MaxUtilization() >= capacity));
        }
        else if (DisplayedGroupChecked)
        {
            var displayedByGroup = _femModelDisplayService.GetDisplayedByGroupBeams(FemModels.DisplayModelId, _femModelParameters.Beams)?.Select(bm => bm.Number)?.ToList();
            displayedBeams.AddRange(calculatedResults.Where(res => displayedByGroup != null && displayedByGroup.Contains(res.Beam.Number)));
        }

        return displayedBeams;
    }
}