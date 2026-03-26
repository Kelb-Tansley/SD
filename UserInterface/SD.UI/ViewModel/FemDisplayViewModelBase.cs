using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Element.Design.Interfaces;

namespace SD.UI.ViewModel;

public abstract partial class FemDisplayViewModelBase(IViewManagementModel viewManagementModel,
                                                      IFemModelDisplayService femModelDisplayService,
                                                      int modelId) : FemViewModelBase(viewManagementModel)
{
    protected readonly IFemModelDisplayService _femModelDisplayService = femModelDisplayService;

    [ObservableProperty]
    public bool _isModelOpen = false;

    public required int _modelId = modelId;
    protected string FilePath { get; set; } = string.Empty;

    protected nint ViewHandle { get; set; }
    public bool ViewLoaded { get; set; }

    public void UpdateFemModelView(nint handle)
    {
        ViewHandle = handle;

        if (ViewLoaded)
            UpdateFemModelView();
    }

    [RelayCommand]
    public void Loaded()
    {
        ViewLoaded = true;
    }

    [RelayCommand]
    public void Unloaded()
    {
        ViewLoaded = false;
    }

    protected void UpdateFemModelView()
    {
        if (IsModelOpen)
        {
            _femModelDisplayService.ReloadFemDisplayModel(_modelId, FilePath, true);
            _femModelDisplayService.DisplayFemModel(_modelId, ViewHandle, true);
            _femModelDisplayService.UpdateFemModel(_modelId, ViewHandle);
        }
    }

    protected async Task CloseRightDrawer()
    {
        ViewManagementModel.IsRightDrawerOpen = false;

        await Task.Delay(450);

        ViewManagementModel.IsDrawerOpen = false;
    }
}