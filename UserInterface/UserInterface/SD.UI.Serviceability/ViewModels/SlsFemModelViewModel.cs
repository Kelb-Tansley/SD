using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Serviceability.Events;
using SD.UI.Serviceability.Models;
using SD.UI.ViewModel;

namespace SD.UI.Serviceability.ViewModels;
public partial class SlsFemModelViewModel : FemViewModelBase
{
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly INotificationService _notificationService;

    private readonly SelectedLoadCombinationsChangedEvent _selectedLoadCombinationsChangedEvent;

    public SlsFemModelViewModel(IViewManagementModel viewManagementModel,
                                IFemModel femModel,
                                IEventAggregator eventAggregator,
                                IFemModelDisplayService femModelDisplayService,
                                INotificationService notificationService) : base(viewManagementModel)
    {
        _femModel = femModel;
        _eventAggregator = eventAggregator;
        _femModelDisplayService = femModelDisplayService;
        _notificationService = notificationService;

        _selectedLoadCombinationsChangedEvent = _eventAggregator.GetEvent<SelectedLoadCombinationsChangedEvent>();
        _selectedLoadCombinationsChangedEvent.Subscribe(async (data) => await SelectedLoadCombinationsChanged(data));
    }

    private bool ViewLoaded { get; set; }
    private nint ViewHandle { get; set; }
    public void UpdateFemModelView(nint handle)
    {
        ViewHandle = handle;

        if (ViewLoaded)
            UpdateFemModelView();
    }

    private void UpdateFemModelView()
    {
        _femModelDisplayService.ReloadFemDisplayModel(FemModels.ServiceabilityModelId, _femModel.FileName, false);
    }

    [RelayCommand]
    public void Loaded()
    {
        ViewLoaded = true;
    }

    [RelayCommand]
    public void Unloaded()
    {
        ViewLoaded = false;
    }

    private async Task SelectedLoadCombinationsChanged(CalculateEventModel calculateEvent)
    {
        try
        {
            if (calculateEvent.DeflectionResults != null && calculateEvent.DeflectionResults.Any())
                await _femModelDisplayService.DisplayDeflectionContours(FemModels.ServiceabilityModelId,
                                                                        ViewHandle,
                                                                        calculateEvent.MinDeflectionRatio,
                                                                        calculateEvent.DeflectionAxis,
                                                                        calculateEvent.DeflectionResults);
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
    }
}