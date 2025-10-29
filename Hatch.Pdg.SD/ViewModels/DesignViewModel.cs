using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.UI.Helpers;

namespace Hatch.Pdg.SD.ViewModels;
public partial class DesignViewModel(IDesignService designService) : ObservableObject
{
    private readonly IDesignService _designService = designService ?? throw new ArgumentNullException(nameof(designService));

    [ObservableProperty]
    public WindowResizer? mainWindowResizer;

    [RelayCommand]
    public void Closing()
    {
        _designService.CloseDesignWindow();
    }
}