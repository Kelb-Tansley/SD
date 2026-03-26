using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.UI.Constants;
using SD.UI.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace SD.UI.Tools.ViewModels;

public partial class TankDesignViewModel : FemDisplayViewModelBase
{
    private readonly IAppSettings _appSettings;
    private readonly ITankDesignService _tankDesignService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    public double _tankDiameter = 12400;

    [ObservableProperty]
    public double _baseThickness = 12;

    [ObservableProperty]
    public double _roofThickness = 8;

    [ObservableProperty]
    public double _meshElementSize = 250;

    [ObservableProperty]
    public ObservableCollection<HeightSegment> _segments = [];

    [ObservableProperty]
    public string _plateElementCount = "4 Nodes";

    private readonly string STRAND_TANK_MODEL_FILE_NAME = "TankModel.st7";

    public TankDesignViewModel(IViewManagementModel viewManagementModel,
                               ITankDesignService tankDesignService,
                               IAppSettings appSettings,
                               IFemModelDisplayService femModelDisplayService,
                               INotificationService notificationService) 
        : base(viewManagementModel, femModelDisplayService, FemModels.TankDesignDisplayModelId)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _tankDesignService = tankDesignService ?? throw new ArgumentNullException(nameof(tankDesignService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        var DumpFolderPath = Environment.ExpandEnvironmentVariables(_appSettings.AppDataLocation);
        FilePath = DumpFolderPath + STRAND_TANK_MODEL_FILE_NAME;
    }

    [RelayCommand]
    public async Task Generate()
    {
        try
        {
            await _tankDesignService.BuildCircularTankModel(FemModels.TankDesignModelId,
                                                            TankDiameter,
                                                            [.. Segments],
                                                            MeshElementSize,
                                                            GetPlateNodeCount(),
                                                            RoofThickness,
                                                            BaseThickness,
                                                            FilePath);

            IsModelOpen = true;

            UpdateFemModelView();
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
    public async Task Save()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Strand7 Files (*.st7)|*.st7",
            FileName = STRAND_TANK_MODEL_FILE_NAME,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            File.Copy(FilePath, saveFileDialog.FileName, true);

            Process.Start(new ProcessStartInfo
            {
                FileName = saveFileDialog.FileName,
                UseShellExecute = true
            });
        }
    }

    private int GetPlateNodeCount()
    {
        if (PlateElementCount.Contains("4 Node"))
            return 4;

        return 0;
    }
}