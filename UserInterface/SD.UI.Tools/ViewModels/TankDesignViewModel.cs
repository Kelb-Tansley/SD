using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.Fem.Strand7.Services;
using SD.UI.Constants;
using SD.UI.Singletons;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SD.UI.Tools.ViewModels;

public partial class TankDesignViewModel(IViewManagementModel viewManagementModel,
                                         ITankDesignService tankDesignService,
                                         IAppSettings appSettings,
                                IFemModelDisplayService femModelDisplayService,
                                         INotificationService notificationService) : FemViewModelBase(viewManagementModel)
{
    private readonly IAppSettings _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly IFemModelDisplayService _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
    private string DumpFolderPath => Environment.ExpandEnvironmentVariables(_appSettings.AppDataLocation);
    private readonly string STRAND_TANK_MODEL_FILE_NAME = "TankModel.st7";
    private string FilePath => DumpFolderPath + STRAND_TANK_MODEL_FILE_NAME;

    private readonly ITankDesignService _tankDesignService = tankDesignService;
    private readonly INotificationService _notificationService = notificationService;

    [ObservableProperty]
    public double _tankDiameter = 10000;

    [ObservableProperty]
    public double _meshElementSize = 100;

    [ObservableProperty]
    public bool _isModelOpen = false;

    [ObservableProperty]
    public ObservableCollection<HeightSegment> _segments = [];

    [ObservableProperty]
    public string _plateElementCount = "4 Nodes";

    [RelayCommand]
    public async Task Generate()
    {
        try
        {
            await _tankDesignService.BuildCircularTankModel(
                FemModels.TankDesignModelId,
                TankDiameter,
                [.. Segments],
               MeshElementSize,
                 GetPlateNodeCount(),
                20,
                30,
                FilePath);

            IsModelOpen = true;

            UpdateFemModelView();

            //Process.Start(new ProcessStartInfo
            //{
            //    FileName = FilePath,
            //    UseShellExecute = true
            //});
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfError(new Notification("Error", ex.Message));
        }
    }

    [RelayCommand]
    public void AddHeightSegment()
    {
        Segments.Add(new HeightSegment(1000, 10, Segments.Count + 1));
    }

    [RelayCommand]
    public async Task Cancel()
    {
        IsModelOpen = false;
        await CloseRightDrawer();
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

    private nint ViewHandle { get; set; }
    public bool ViewLoaded { get; set; }

    public void UpdateFemModelView(nint handle)
    {
        ViewHandle = handle;

        if (ViewLoaded)
            UpdateFemModelView();
    }
    private void UpdateFemModelView()
    {
        if (IsModelOpen)
        {
            _femModelDisplayService.ReloadFemDisplayModel(FemModels.TankDesignDisplayModelId, FilePath, true);
            _femModelDisplayService.DisplayFemModel(FemModels.TankDesignDisplayModelId, ViewHandle, true);
            _femModelDisplayService.UpdateFemModel(FemModels.TankDesignDisplayModelId, ViewHandle);
        }
    }

    private int GetPlateNodeCount()
    {
        if (PlateElementCount.Contains("4 Node"))
            return 4;

        return 0;
    }
}