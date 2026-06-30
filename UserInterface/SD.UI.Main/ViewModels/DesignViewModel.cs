using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.UltimateLimitState.ViewModels;
using SD.UI.UltimateLimitState.Views;

namespace SD.UI.Main.ViewModels;

public partial class DesignViewModel : ObservableObject
{
    private readonly IUlsDesignResults _ulsDesignResults;

    [ObservableProperty]
    public partial bool IsNavigationPanelCollapsed { get; set; }

    [ObservableProperty]
    public partial bool HasUlsData { get; set; }

    [ObservableProperty]
    public partial int SelectedTabIndex { get; set; }

    public DesignViewModel(IRegionManager regionManager,
                           IContainerProvider containerProvider,
                           IUlsDesignResults ulsDesignResults,
                           IEventAggregator eventAggregator)
    {
        _ulsDesignResults = ulsDesignResults;

        regionManager.RegisterViewWithRegion(RegionNames.NavigationLeftPanelRegion, typeof(CombinationsTableView));
        regionManager.RegisterViewWithRegion(RegionNames.FemRegion, typeof(FemModelView));
        regionManager.RegisterViewWithRegion(RegionNames.SingleElementDesignRegionTabbed, typeof(BeamDesignView));
        regionManager.RegisterViewWithRegion(RegionNames.UlsDataRegion, typeof(UlsDataView));
        regionManager.RegisterViewWithRegion(RegionNames.BeamFemModelRegion, typeof(BeamFemModelView));

        eventAggregator.GetEvent<LoadCaseChangedEvent>().Subscribe(UpdateHasUlsData);
        eventAggregator.GetEvent<RefreshCalculationEvent>().Subscribe(UpdateHasUlsData);
        eventAggregator.GetEvent<DesignCodeChangedEvent>().Subscribe(UpdateHasUlsData);
        eventAggregator.GetEvent<FileClosedEvent>().Subscribe(() => HasUlsData = false);
        eventAggregator.GetEvent<SelectUlsTabEvent>().Subscribe(SelectUlsTab);
        eventAggregator.GetEvent<SelectDataTabEvent>().Subscribe(SelectDataTab);

        containerProvider.Resolve<FemModelViewModel>();
        containerProvider.Resolve<CombinationsTableViewModel>();
        containerProvider.Resolve<BeamFemModelViewModel>();
        containerProvider.Resolve<BeamDesignViewModel>();

        UpdateHasUlsData();
    }

    private void SelectDataTab(UlsResult result)
    {
        if (result is not null)
            SelectedTabIndex = 3;
    }

    private void SelectUlsTab(UlsResult result)
    {
        if (result is not null)
            SelectedTabIndex = 2;
    }

    private void UpdateHasUlsData()
    {
        HasUlsData = _ulsDesignResults.GetUlsResults()?.Any() == true;
    }
}