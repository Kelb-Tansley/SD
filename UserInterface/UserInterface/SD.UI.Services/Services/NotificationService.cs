using Prism.Events;
using Prism.Regions;
using SD.Core.Shared.Events;
using SD.UI.Services.Interfaces;
using SD.UI.Services.Models;
using System.Diagnostics;

namespace SD.UI.Services.Services;
public class NotificationService(IRegionManager regionManager, IEventAggregator eventAggregator) : INotificationService
{
    private readonly IEventAggregator _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
    private readonly IEventAggregator _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

    public void NotifyUserOfError(Notification notification)
    {
        _regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
        _eventAggregator.GetEvent<DialogOpenedEvent>().Publish();
        Trace.WriteLine($"Title: {notification.Title}, {notification.WarningLevel}: {notification.Description}");
    }
}
