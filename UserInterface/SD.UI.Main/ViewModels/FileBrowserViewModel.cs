using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Data.Interfaces;
using SD.UI.Events;
using SD.UI.Extensions;
using SD.UI.Models;
using System.Collections.ObjectModel;

namespace SD.UI.Main.ViewModels;
public partial class FileBrowserViewModel : ObservableObject
{
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IFemFilePathService _femFilePathService;

    private readonly FileOpeningEvent _fileOpeningEvent;
    private readonly FileOpenedEvent _fileOpenedEvent;
    private readonly FileClosedEvent _fileClosedEvent;

    public FileBrowserViewModel(IEventAggregator eventAggregator,
                                IFemFilePathService femFilePathService,
                                IFemModel femModel)
    {
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _femFilePathService = femFilePathService ?? throw new ArgumentNullException(nameof(femFilePathService));

        _fileOpeningEvent = _eventAggregator.GetEvent<FileOpeningEvent>();
        _fileOpenedEvent = _eventAggregator.GetEvent<FileOpenedEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();

        _fileClosedEvent.Subscribe(FileClosed);
    }

    [ObservableProperty]
    public ObservableCollection<FileHistoryDisplayModel> fileHistories;

    [ObservableProperty]
    public FileHistoryDisplayModel? selectedFile;

    [RelayCommand]
    public void BrowseFile()
    {
        _fileOpeningEvent.Publish();
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

    private void FileClosed()
    {
        SelectedFile = null;
    }
}
