using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.Singletons;
public partial class FemModel : ObservableObject, IFemModel
{
    public nint ModelHandle { get; set; }
    public nint DesignModelHandle { get; set; }

    [ObservableProperty]
    public string fileName = string.Empty;

    [ObservableProperty]
    public bool fileExists = false;

    public void ClearFile()
    {
        FileName = string.Empty;
        FileExists = false;
    }
}