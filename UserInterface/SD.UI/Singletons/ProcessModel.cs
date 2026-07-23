using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.Singletons;
public partial class ProcessModel : ObservableObject, IProcessModel
{
    [ObservableProperty]
    public partial bool IsPrimaryProcessRunning { get; set; } = false;

    [ObservableProperty]
    public partial bool IsDesignWindowOpen { get; set; } = false;

    [ObservableProperty]
    public partial bool IsFemModelLoaded { get; set; } = false;

    [ObservableProperty]
    public partial bool IsRightDrawerProcessRunning { get; set; } = false;
}
