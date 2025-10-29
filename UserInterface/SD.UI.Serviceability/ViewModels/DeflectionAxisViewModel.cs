using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Enum;
using System.Collections.ObjectModel;

namespace SD.UI.Serviceability.ViewModels;
public partial class DeflectionAxisViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<DeflectionAxis> _options = [DeflectionAxis.X, DeflectionAxis.Y, DeflectionAxis.Z];

    [ObservableProperty]
    public DeflectionAxis _selected = DeflectionAxis.Y;
}
