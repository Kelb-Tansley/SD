using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Data.Interfaces;
using SD.UI.Enums;
using SD.UI.Events;
using SD.UI.Extensions;
using SD.UI.Models;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using DialogResult = System.Windows.Forms.DialogResult;

namespace SD.UI.Main.ViewModels;

public partial class BrowserViewModel : ObservableObject
{
    private readonly IViewManagementModel _viewManagementModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IFemFilePathService _femFilePathService;

    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly WindViewLoadEvent _windViewLoadEvent;
    private readonly GeneralToolsViewChangedEvent _generalToolsViewChangedEvent;

    public BrowserViewModel(IViewManagementModel viewManagementModel,
                            IEventAggregator eventAggregator,
                            IFemFilePathService femFilePathService,
                            IFemModel femModel)
    {
        _viewManagementModel = viewManagementModel ?? throw new ArgumentNullException(nameof(viewManagementModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _femFilePathService = femFilePathService ?? throw new ArgumentNullException(nameof(femFilePathService));

        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();

        _windViewLoadEvent = _eventAggregator.GetEvent<WindViewLoadEvent>();
        _generalToolsViewChangedEvent = _eventAggregator.GetEvent<GeneralToolsViewChangedEvent>();

        _fileClosedEvent.Subscribe(FileClosed);
    }

    [ObservableProperty]
    public ObservableCollection<FileHistoryDisplayModel>? fileHistories;

    [ObservableProperty]
    public FileHistoryDisplayModel? selectedFile;

    [RelayCommand]
    public void BrowseFile()
    {
        var fileName = GetStrand7File();
        if (fileName == null)
            return;

        // If a Strand7 file has been selected, close all other opened strand files before loading the new file
        _fileClosedEvent.Publish();

        _femModel.FileName = fileName;

        _fileOpenedEvent.Publish();
    }

    private string? GetStrand7File()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Strand7 files (*.st7)|*.st7",
            InitialDirectory = _femModel.FileName,
            CheckFileExists = true,
            CheckPathExists = true,
            DefaultExt = "st7",
            Multiselect = false,
            Title = "Select a Strand7 file (.st7)",
            ValidateNames = true,
            RestoreDirectory = true
        };

        try
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return null;

            return openFileDialog.FileName;
        }
        catch (Exception) { throw; }
        finally
        {
            openFileDialog.Dispose();
        }
    }

    [RelayCommand]
    public void FileSelected()
    {
        if (SelectedFile == null)
            return;

        _femModel.FileName = SelectedFile.FilePath;
        _fileOpenedEvent.Publish();
    }

    [RelayCommand]
    public async Task Loaded()
    {
        FileHistories = new ObservableCollection<FileHistoryDisplayModel>((await _femFilePathService.GetPreviousFemFiles()).ToFileHistoryDisplayModels());
    }

    [RelayCommand]
    public async Task BeamWindLoad()
    {
        var fileName = GetStrand7File();
        if (fileName == null)
            return;

        _viewManagementModel.IsDrawerOpen = true;

        _generalToolsViewChangedEvent.Publish(GeneralToolsView.WindLoading);
        await Task.Delay(150);

        _windViewLoadEvent.Publish(fileName);

        _viewManagementModel.IsRightDrawerOpen = true;
    }

    [RelayCommand]
    public async Task TankModel()
    {
        _viewManagementModel.IsDrawerOpen = true;

        _generalToolsViewChangedEvent.Publish(GeneralToolsView.TankDesign);
        await Task.Delay(150);

        _viewManagementModel.IsRightDrawerOpen = true;
    }

    private void FileClosed()
    {
        SelectedFile = null;
    }
}