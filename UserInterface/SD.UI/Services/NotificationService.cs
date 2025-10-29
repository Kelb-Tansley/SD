using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Events;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.UI.Events;
using SD.UI.Views;
using System.Windows;

namespace SD.UI.Services;
public class NotificationService : INotificationService
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ISnackbarModel _snackbarModel;
    private readonly AppShutdownEvent _appShutdownEvent;
    private readonly FileClosedEvent _fileClosedEvent;

    public NotificationService(IEventAggregator eventAggregator, ISnackbarModel snackbarModel)
    {
        _eventAggregator = eventAggregator;
        _snackbarModel = snackbarModel;
        _appShutdownEvent = _eventAggregator.GetEvent<AppShutdownEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
    }

    public void NotifyUserOfError(Notification notification)
    {
        SDMessageBox.Show(notification.Description, notification.Title, true);
    }

    public void NotifyUserOfErrorAndCloseFile(Notification notification)
    {
        SDMessageBox.Show(notification.Description, notification.Title, true);
        _fileClosedEvent.Publish();
    }

    public void ShowSnackNotification(Notification notification)
    {
        _snackbarModel.ShowMessage(notification.Description, notification.Timer);
    }

    public MessageBoxResult NotifyUserWithYesNoOption(Notification notification)
    {
        return SDMessageBox.Show(notification.Description, notification.Title, MessageBoxButton.YesNo, false);
    }

    public void ShutdownAfterErrorNotice(Notification notification)
    {
        var result = SDMessageBox.Show(notification.Description, notification.Title, true);
        if (result == MessageBoxResult.OK)
            _appShutdownEvent.Publish();
    }
}
