using SD.Core.Shared.Models;
using System.Windows;

namespace SD.Core.Infrastructure.Interfaces;
public interface INotificationService
{
    public void ShowSnackNotification(Notification notification);
    public void ShutdownAfterErrorNotice(Notification notification);
    public void NotifyUserOfError(Notification notification);
    public void NotifyUserOfErrorAndCloseFile(Notification notification);
    MessageBoxResult NotifyUserWithYesNoOption(Notification notification);
}