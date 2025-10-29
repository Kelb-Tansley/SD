using CommunityToolkit.Mvvm.ComponentModel;
using SD.UI.Constants;
using SD.UI.UltimateLimitState.Views;

namespace SD.UI.UltimateLimitState.ViewModels;
public partial class UlsDesignViewModel : ObservableObject
{
    public UlsDesignViewModel(IRegionManager regionManager)
    {
        regionManager.RegisterViewWithRegion(RegionNames.UlsLoadCombinationsViewRegion, typeof(UlsLoadCombinationsView));
        regionManager.RegisterViewWithRegion(RegionNames.UlsFemModelViewRegion, typeof(FemModelView));
    }
}