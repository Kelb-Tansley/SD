using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.ViewModel;
public partial class FemViewModelBase(IViewManagementModel viewManagementModel) : ObservableObject
{
    [ObservableProperty]
    public IViewManagementModel _viewManagementModel = viewManagementModel;

    protected async Task CloseRightDrawer()
    {
        ViewManagementModel.IsRightDrawerOpen = false;

        await Task.Delay(450);

        ViewManagementModel.IsDrawerOpen = false;
    }
}
