using CommunityToolkit.Mvvm.ComponentModel;
using SD.UI.Constants;
using SD.UI.UltimateLimitState.Views;

namespace SD.UI.Main.ViewModels;
public partial class NavigationViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isNavigationPanelCollapsed;

    public NavigationViewModel(IRegionManager regionManager)
    {
        regionManager.RegisterViewWithRegion(RegionNames.NavigationLeftPanelRegion, typeof(CombinationsTableView));
        regionManager.RegisterViewWithRegion(RegionNames.FemRegion, typeof(FemModelView));

        regionManager.RegisterViewWithRegion(RegionNames.SingleElementDesignRegionTabbed, typeof(BeamDesignView));
        regionManager.RegisterViewWithRegion(RegionNames.BeamFemModelRegion, typeof(BeamFemModelView));
    }
}