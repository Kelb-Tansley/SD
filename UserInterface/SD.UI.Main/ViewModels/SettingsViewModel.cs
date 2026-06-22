using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.UI.Events;

namespace SD.UI.Main.ViewModels;

public partial class SettingsViewModel(IViewManagementModel viewManagementModel,
                                       IEventAggregator eventAggregator) : ObservableObject
{
    private readonly RefreshCalculationEvent _refreshCalculationEvent = eventAggregator.GetEvent<RefreshCalculationEvent>();

    [RelayCommand]
    public async Task SaveSettings()
    {
        _refreshCalculationEvent.Publish();
        await Task.Delay(300);
        viewManagementModel.IsDrawerOpen = false;
    }
}
