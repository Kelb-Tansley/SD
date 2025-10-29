using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.UI.Enums;
using SD.UI.Events;
using SD.UI.Models;

namespace SD.UI.Main.ViewModels;
public partial class NotificationViewModel : ObservableObject
{
    private readonly IEventAggregator _eventAggregator;
    private readonly DialogOpenedEvent _dialogOpenedEvent;

    [ObservableProperty]
    public Notification? notification;

    [ObservableProperty]
    public bool cancelCommandVisible = false;
    
    [ObservableProperty]
    public bool noCommandVisible = false;
    
    [ObservableProperty]
    public bool yesCommandVisible = false;
    
    [ObservableProperty]
    public bool okCommandVisible = false;

    public NotificationViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

        _dialogOpenedEvent = _eventAggregator.GetEvent<DialogOpenedEvent>();
        _dialogOpenedEvent.Subscribe(DialogOpened);
    }

    private void DialogOpened(Notification notification)
    {
        Notification = notification;
        switch (notification.WarningLevel)
        {
            case WarningLevel.Info:
                YesCommandVisible = true;
                NoCommandVisible = true;
                OkCommandVisible = false;
                CancelCommandVisible = true;
                break;
            case WarningLevel.Warning:
                YesCommandVisible = false;
                NoCommandVisible = false;
                OkCommandVisible = true;
                CancelCommandVisible = true;
                break;
            case WarningLevel.Error:
                YesCommandVisible = false;
                NoCommandVisible = false;
                OkCommandVisible = true;
                CancelCommandVisible = true;
                break;
        }
    }

    [RelayCommand]
    public void Cancel(Notification notification)
    {
        _eventAggregator.GetEvent<DialogClosedEvent>().Publish();
    }
    
    [RelayCommand]
    public void No(Notification notification)
    {
        _eventAggregator.GetEvent<DialogClosedEvent>().Publish();
    }

    [RelayCommand]
    public void Yes(Notification notification)
    {
        _eventAggregator.GetEvent<DialogClosedEvent>().Publish();
    }

    [RelayCommand]
    public void Ok(Notification notification)
    {
        _eventAggregator.GetEvent<DialogClosedEvent>().Publish();
    }

    [RelayCommand]
    public void CopyNotification(Notification notification)
    {
        _eventAggregator.GetEvent<DialogClosedEvent>().Publish();
    }

    [RelayCommand]
    public void Closing()
    {
        _dialogOpenedEvent?.Unsubscribe(DialogOpened);
    }
}
