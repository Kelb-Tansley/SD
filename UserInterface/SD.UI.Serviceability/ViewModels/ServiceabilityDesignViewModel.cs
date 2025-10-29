using CommunityToolkit.Mvvm.ComponentModel;
using SD.UI.Constants;
using SD.UI.Serviceability.Views;

namespace SD.UI.Serviceability.ViewModels;
public partial class ServiceabilityDesignViewModel : ObservableObject
{
    public ServiceabilityDesignViewModel(IRegionManager regionManager)
    {
        regionManager.RegisterViewWithRegion(RegionNames.SlsLoadCombinationsViewRegion, typeof(LoadCombinationsView));
        regionManager.RegisterViewWithRegion(RegionNames.SlsFemModelViewRegion, typeof(SlsFemModelView));
    }
}
