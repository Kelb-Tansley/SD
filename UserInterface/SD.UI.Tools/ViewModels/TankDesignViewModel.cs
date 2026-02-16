using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.ViewModel;
using System.Diagnostics;

namespace SD.UI.Tools.ViewModels;

public partial class TankDesignViewModel(IViewManagementModel viewManagementModel,
                                         ITankDesignService tankDesignService,
                                         IAppSettings appSettings,
                                         INotificationService notificationService) : FemViewModelBase(viewManagementModel)
{
    private readonly IAppSettings _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    private string DumpFolderPath => Environment.ExpandEnvironmentVariables(_appSettings.AppDataLocation);
    private readonly string STRAND_TANK_MODEL_FILE_NAME = "TankModel.st7";
    private string FilePath => DumpFolderPath + STRAND_TANK_MODEL_FILE_NAME;

    private readonly ITankDesignService _tankDesignService = tankDesignService;
    private readonly INotificationService _notificationService = notificationService;

    [ObservableProperty]
    public double _tankDiameter = 0.0;

    [RelayCommand]
    public async Task Generate()
    {
        try
        {
            await _tankDesignService.BuildCircularTankModel(
                FemModels.TankDesignModelId,
                TankDiameter,
                [new(200, 15, 1), new(100, 20, 2)],
                100,
                4,
                20,
                30,
                FilePath);

            Process.Start(new ProcessStartInfo
            {
                FileName = FilePath,
                UseShellExecute = true
            });
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