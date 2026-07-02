using SD.Core.Infrastructure.Interfaces;
using SD.Core.Infrastructure.Logging;
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
    private readonly ILoggerService _logger;
    private readonly AppShutdownEvent _appShutdownEvent;
    private readonly FileClosedEvent _fileClosedEvent;

    public NotificationService(IEventAggregator eventAggregator, ISnackbarModel snackbarModel, ILoggerService logger)
    {
        _eventAggregator = eventAggregator;
        _snackbarModel = snackbarModel;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appShutdownEvent = _eventAggregator.GetEvent<AppShutdownEvent>();
        _fileClosedEvent = _eventAggregator.GetEvent<FileClosedEvent>();
    }

    public void NotifyUserOfError(Notification notification)
    {
        _logger.LogError(GetType(), $"User error notification: {notification.Title} - {notification.Description}");
        SDMessageBox.Show(notification.Description, notification.Title, true);
    }

    public void NotifyUserOfErrorAndCloseFile(Notification notification)
    {
        _logger.LogError(GetType(), $"Critical error, closing file: {notification.Title} - {notification.Description}");
        SDMessageBox.Show(notification.Description, notification.Title, true);
        _fileClosedEvent.Publish();
    }

    public void ShowSnackNotification(ShortNotification notification)
    {
        _snackbarModel.ShowMessage(notification.Description, notification.Timer);
    }

    public MessageBoxResult NotifyUserWithYesNoOption(Notification notification)
    {
        return SDMessageBox.Show(notification.Description, notification.Title, MessageBoxButton.YesNo, false);
    }

    public void ShutdownAfterErrorNotice(Notification notification)
    {
        _logger.LogError(GetType(), $"Fatal error, shutting down: {notification.Title} - {notification.Description}");
        var result = SDMessageBox.Show(notification.Description, notification.Title, true);
        if (result == MessageBoxResult.OK)
            _appShutdownEvent.Publish();
    }
}
