using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.Singletons;
public partial class ViewManagementModel : ObservableObject, IViewManagementModel
{
    [ObservableProperty]
    public bool isDialogOpen = false;

    [ObservableProperty]
    public bool isDrawerOpen = false;

    [ObservableProperty]
    public bool isRightDrawerOpen = false;
}
