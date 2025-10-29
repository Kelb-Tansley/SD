using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.Main.ViewModels;
public partial class DesignSettingsViewModel(IDesignModel designModel) : ObservableObject
{
    [ObservableProperty]
    private IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
}