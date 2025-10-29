using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.Loading.Views;
using SD.UI.ViewModel;
using System.Windows.Forms;
using DialogResult = System.Windows.Forms.DialogResult;

namespace SD.UI.Main.ViewModels;
public partial class ToolBarViewModel : ViewModelBase
{
    private readonly IRegionManager _regionManager;
    private readonly IViewManagementModel _viewManagementModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUlsDesignResults _ulsDesignResults;

    private readonly FileOpeningEvent _fileOpeningEvent;
    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;
    private readonly RefreshEvent _refreshEvent;

    [ObservableProperty]
    public required IDesignModel _designModel;

    [ObservableProperty]
    public required IFemModelParameters _femModelParameters;

    [ObservableProperty]
    public bool _femModelOpened;

    [ObservableProperty]
    public bool _useEnvelopeLoadCase;

    public ToolBarViewModel(IRegionManager regionManager,
                            IViewManagementModel viewManagementModel,
                            IFemModel femModel,
                            IDesignModel designModel,
                            IProcessModel processModel,
                            IUlsDesignResults ulsDesignResults,
                            IFemModelParameters femModelParameters,
                            IEventAggregator eventAggregator) : base(processModel)
    {
        _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
        _viewManagementModel = viewManagementModel ?? throw new ArgumentNullException(nameof(viewManagementModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));

        _fileOpeningEvent = _eventAggregator.GetEvent<FileOpeningEvent>();
        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
        _refreshEvent = _eventAggregator.GetEvent<RefreshEvent>();
        _designCodeChangedEvent = _eventAggregator.GetEvent<DesignCodeChangedEvent>();
    }

    [RelayCommand]
    private void FolderBrowse()
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

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        // If a Strand7 file has been selected, close all other opened strand files before loading the new file
        _fileClosedEvent.Publish();

        _femModel.FileName = openFileDialog.FileName;

        _fileOpenedEvent.Publish();

        // Close the dialog
        openFileDialog.Dispose();
    }

    [RelayCommand]
    private void DesignCodeChanged()
    {
        _designCodeChangedEvent?.Publish();

        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        FemModelParameters.Clear();
        _ulsDesignResults.Clear(); //Check if this is correct
        _refreshEvent.Publish();
    }

    [RelayCommand]
    private async Task OpenSettings()
    {
        _viewManagementModel.IsDrawerOpen = true;
        await Task.Delay(200);
        _viewManagementModel.IsDialogOpen = true;
    }

    [RelayCommand]
    public void Loaded()
    {
        _fileOpeningEvent.Subscribe(FolderBrowse);
        _fileClosedEvent.Subscribe(FileClosed);
    }

    [RelayCommand]
    public void Closing()
    {
        _fileOpeningEvent.Unsubscribe(FolderBrowse);
        _fileClosedEvent.Unsubscribe(FileClosed);
    }

    private void FileClosed()
    {
        ProcessModel.IsFemModelLoaded = false;
        FemModelOpened = false;
        _femModel.ClearFile();
        FemModelParameters.Clear();
        _ulsDesignResults.Clear();
    }
}