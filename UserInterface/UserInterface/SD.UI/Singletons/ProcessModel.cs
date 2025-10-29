using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.Singletons;
public partial class ProcessModel : ObservableObject, IProcessModel
{
    [ObservableProperty]
    public bool isPrimaryProcessRunning = false;
    [ObservableProperty]
    public bool isDesignWindowOpen = false;
    [ObservableProperty]
    public bool isFemModelLoaded = false;
}
