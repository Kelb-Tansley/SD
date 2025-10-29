using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.ViewModel;

namespace SD.UI.UltimateLimitState.ViewModels;

public partial class FemModelViewModel : FemViewModelBase
{
    private readonly IFemModel _femModel;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IEventAggregator _eventAggregator;
    private readonly INotificationService _notificationService;
    private readonly IProcessModel _processModel;

    private readonly BeamDesignWindowClosedEvent _beamDesignWindowClosedEvent;

    private bool _isLoaded = false;

    public FemModelViewModel(IViewManagementModel viewManagementModel,
                             IFemModel femModel,
                             IFemModelDisplayService femModelDisplayService,
                             IEventAggregator eventAggregator,
                             IProcessModel processModel,
                             INotificationService notificationService) : base(viewManagementModel)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _processModel = processModel ?? throw new ArgumentNullException(nameof(processModel));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        _beamDesignWindowClosedEvent = _eventAggregator.GetEvent<BeamDesignWindowClosedEvent>();
    }

    public void UpdateFemModelView(nint modelHandle)
    {
        try
        {
            _femModel.ModelHandle = modelHandle;
            if (_femModel.FileExists && _isLoaded && _processModel.IsFemModelLoaded)
                _femModelDisplayService.UpdateFemModel(FemModels.DisplayModelId, _femModel.ModelHandle);
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
    }

    private void BeamDesignWindowClosed()
    {
        UpdateFemModelView(_femModel.ModelHandle);
    }

    [RelayCommand]
    public void Loaded()
    {
        _isLoaded = true;
        _beamDesignWindowClosedEvent.Subscribe(BeamDesignWindowClosed);
    }

    [RelayCommand]
    public void Unloaded()
    {
        _isLoaded = false;
        _beamDesignWindowClosedEvent.Unsubscribe(BeamDesignWindowClosed);
    }
}