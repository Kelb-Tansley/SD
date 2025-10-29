using CommunityToolkit.Mvvm.ComponentModel;
using SD.UI.Constants;
using SD.UI.Serviceability.Views;
using SD.UI.UltimateLimitState.Views;

namespace SD.UI.Main.ViewModels;
public partial class NavigationViewModel : ObservableObject
{
    public NavigationViewModel(IRegionManager regionManager)
    {
        regionManager.RegisterViewWithRegion(RegionNames.FemRegion, typeof(UlsDesignView));
        regionManager.RegisterViewWithRegion(RegionNames.DesignRegion, typeof(BeamDesignView));
        //regionManager.RegisterViewWithRegion(RegionNames.FemRegion, typeof(FemModelView));
        //regionManager.RegisterViewWithRegion(RegionNames.TabbedFemRegion, typeof(FemModelView));
        regionManager.RegisterViewWithRegion(RegionNames.SingleElementDesignRegionTabbed, typeof(BeamDesignView));
        regionManager.RegisterViewWithRegion(RegionNames.ServiceabilityDesignRegionTabbed, typeof(ServiceabilityDesignView));
        regionManager.RegisterViewWithRegion(RegionNames.BeamFemModelRegion, typeof(BeamFemModelView));
    }
}