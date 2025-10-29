using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.UI;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.Core.Shared.Extensions;
using SD.UI.ViewModel;
using SD.Core.Infrastructure.Interfaces;

namespace SD.UI.UltimateLimitState.ViewModels;

[RegionMemberLifetime(KeepAlive = true)]
public partial class BeamFemModelViewModel : FemViewModelBase
{
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly INotificationService _notificationService;
    private readonly BeamDesignWindowClosedEvent _beamDesignWindowClosedEvent;
    private readonly SelectedBeamChangedEvent _selectedBeamChangedEvent;
    private readonly DesignFemResizeEvent _designFemResizeEvent;
    private readonly ShellResizeEvent _shellResizeEvent;
    private readonly FileClosedEvent _fileClosedEvent;

    [ObservableProperty]
    public bool isFemLoaded = false;

    [ObservableProperty]
    private BeamDisplayComponent displayComponent = new();

    private UlsResult? _selectedBeamResult;
    private ZoomLevel _zoomLevel = ZoomLevel.Level1;

    public BeamFemModelViewModel(IViewManagementModel viewManagementModel,
                                 IFemModel femModel,
                                 IEventAggregator eventAggregator,
                                 IFemModelDisplayService femModelDisplayService,
                                 INotificationService notificationService) : base(viewManagementModel)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        _beamDesignWindowClosedEvent = _eventAggregator.GetEvent<BeamDesignWindowClosedEvent>();
        _selectedBeamChangedEvent = _eventAggregator.GetEvent<SelectedBeamChangedEvent>();
        _designFemResizeEvent = _eventAggregator.GetEvent<DesignFemResizeEvent>();
        _shellResizeEvent = _eventAggregator.GetEvent<ShellResizeEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();

        _selectedBeamChangedEvent.Subscribe(SelectedBeamResultChanged);
        _beamDesignWindowClosedEvent.Subscribe(BeamDesignWindowClosed);
        _designFemResizeEvent.Subscribe(Refresh);
        _shellResizeEvent.Subscribe(Refresh);
        _fileClosedEvent.Subscribe(FileClosed);

        LoadFemModel();
    }

    private void LoadFemModel()
    {
        if (!string.IsNullOrWhiteSpace(_femModel.FileName))
        {
            _femModelDisplayService.OpenFemFile(FemModels.DesignModelId, _femModel.FileName, true);
            _ = _femModelDisplayService.OpenFemResultsFile(FemModels.DesignModelId, _femModel.FileName);
        }
    }

    private void BeamDesignWindowClosed()
    {
        _femModelDisplayService.CloseFemResultsFile(FemModels.DesignModelId);

        _beamDesignWindowClosedEvent.Unsubscribe(BeamDesignWindowClosed);
        _selectedBeamChangedEvent.Unsubscribe(SelectedBeamResultChanged);
        _designFemResizeEvent.Unsubscribe(Refresh);
        _shellResizeEvent.Unsubscribe(Refresh);
        _fileClosedEvent.Unsubscribe(FileClosed);

        _selectedBeamResult = null;
    }

    private int _childId = 0;
    private bool _isLoaded;
    private bool _strand7ModelOpened = false;

    private void SelectedBeamResultChanged(UlsResult? result)
    {
        if (result == null || _selectedBeamResult == result)
            return;

        UpdateFemModel(result, false);
    }

    private void UpdateFemModel(UlsResult? result, bool isInitialized)
    {
        try
        {
            IsFemLoaded = false;

            if (result != null && _isLoaded)
            {
                if (!_strand7ModelOpened)
                {
                    _femModelDisplayService.OpenFemFile(FemModels.DesignModelId, _femModel.FileName, true);
                    var resultsFile = _femModelDisplayService.OpenFemResultsFile(FemModels.DesignModelId, _femModel.FileName);
                    _strand7ModelOpened = resultsFile.NumberOfLoadCases > 0;
                }
                _femModelDisplayService.UpdateBeamFemModel(FemModels.DesignModelId, _femModel.DesignModelHandle, result.LoadCaseNumber, ref _childId, isInitialized, _zoomLevel, result.Beam, DisplayComponent);

                IsFemLoaded = true;
            }
        }
        catch (Exception ex)
        {
            IsFemLoaded = false;
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            _selectedBeamResult = result;

            if (!IsFemLoaded)
                IsFemLoaded = _selectedBeamResult != null;
        }
    }

    public void UpdateFemModelView(nint modelHandle)
    {
        _femModel.DesignModelHandle = modelHandle;
        UpdateFemModel(_selectedBeamResult, false);
    }

    [RelayCommand]
    public void Refresh()
    {
        UpdateFemModel(_selectedBeamResult, false);
    }

    [RelayCommand]
    public async Task ZoomOut()
    {
        _zoomLevel = _zoomLevel.ZoomOut();
        await Task.Run(() => UpdateFemModel(_selectedBeamResult, true));
    }

    [RelayCommand]
    public async Task ZoomIn()
    {
        _zoomLevel = _zoomLevel.ZoomIn();
        await Task.Run(() => UpdateFemModel(_selectedBeamResult, true));
    }

    [RelayCommand]
    public void Loaded()
    {
        _isLoaded = true;
    }

    [RelayCommand]
    public void Unloaded()
    {
        _isLoaded = false;
    }
    private void FileClosed()
    {
        _strand7ModelOpened = false;
        _selectedBeamResult = null;
        IsFemLoaded = _selectedBeamResult != null;
    }
}