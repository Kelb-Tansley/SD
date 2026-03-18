using SD.Core.Shared.Models;
using System.Windows;

namespace SD.Core.Infrastructure.Interfaces;
public interface INotificationService
{
    public void ShowSnackNotification(ShortNotification notification);
    public void ShutdownAfterErrorNotice(Notification notification);
    public void NotifyUserOfError(Notification notification);
    public void NotifyUserOfErrorAndCloseFile(Notification notification);
    MessageBoxResult NotifyUserWithYesNoOption(Notification notification);
}