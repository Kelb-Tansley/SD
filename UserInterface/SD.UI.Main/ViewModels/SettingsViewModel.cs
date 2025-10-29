using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Element.Design.Interfaces;
using SD.UI.Events;

namespace SD.UI.Main.ViewModels;
public partial class SettingsViewModel : ObservableObject
{
    private readonly IDesignModel _designModel;
    private readonly IFemModel _femModel;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IDataAccessService _dataAccessService;
    private readonly IRegionManager _regionManager;
    private readonly IViewManagementModel _viewManagementModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly RefreshCalculationEvent _refreshCalculationEvent;

    public SettingsViewModel(IDesignModel designModel,
                             IFemModel femModel, 
                             IViewManagementModel viewManagementModel,
                             IFemModelParameters femModelParameters,
                             IRegionManager regionManager,
                             IEventAggregator eventAggregator,
                             IDataAccessService dataAccessService)
    {
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _viewManagementModel = viewManagementModel ?? throw new ArgumentNullException(nameof(viewManagementModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _dataAccessService = dataAccessService ?? throw new ArgumentNullException(nameof(dataAccessService));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

        _refreshCalculationEvent = _eventAggregator.GetEvent<RefreshCalculationEvent>();
    }

    [RelayCommand]
    public async Task SaveSettings()
    {
        _refreshCalculationEvent.Publish();
        await Task.Delay(300);
        _viewManagementModel.IsDrawerOpen = false;
    }
}
