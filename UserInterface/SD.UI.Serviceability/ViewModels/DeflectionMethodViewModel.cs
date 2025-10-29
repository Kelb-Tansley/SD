using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Enum;
using System.Collections.ObjectModel;

namespace SD.UI.Serviceability.ViewModels;
public partial class DeflectionMethodViewModel : ObservableObject
{

    [ObservableProperty]
    public ObservableCollection<DeflectionMethod> _options = [DeflectionMethod.Absolute, DeflectionMethod.Relative];

    [ObservableProperty]
    public DeflectionMethod _selected = DeflectionMethod.Absolute;
}
