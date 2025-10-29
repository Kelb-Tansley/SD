using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.ViewModel;

namespace SD.UI.Tools.ViewModels;

public partial class BucklingAnalysisViewModel(IViewManagementModel viewManagementModel,
                                               IBucklingAnalysisService bucklingAnalysisService,
                                               IFemModel femModel,
                                               INotificationService notificationService) : FemViewModelBase(viewManagementModel)
{
    private readonly IBucklingAnalysisService _bucklingAnalysisService = bucklingAnalysisService;
    private readonly IFemModel _femModel = femModel;
    private readonly INotificationService _notificationService = notificationService;

    [ObservableProperty]
    public int _fixedLoadCase = 0;

    [ObservableProperty]
    public int _numberOfModes = 10;

    [ObservableProperty]
    public int _startingLoadCase = 1;

    [ObservableProperty]
    public int _endLoadCase = 1;

    [ObservableProperty]
    public bool _includeEndLoadCaseChecked = true;

    [RelayCommand]
    public async Task Solve()
    {
        try
        {
            await _bucklingAnalysisService.SolveModelAsync(FemModels.GeneralToolsModelId,
                                                           _femModel.FileName,
                                                           NumberOfModes,
                                                           StartingLoadCase,
                                                           EndLoadCase,
                                                           FixedLoadCase,
                                                           true,
                                                           IncludeEndLoadCaseChecked);

        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfError(new Notification("Error", ex.Message));
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await CloseRightDrawer();
    }
}