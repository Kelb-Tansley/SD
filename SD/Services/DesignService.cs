using SD.ViewModels;
using SD.Views;
using SD.Core.Shared.Contracts;
using SD.UI.Events;

namespace SD.Services;

public class DesignService(IContainerProvider containerProvider,
                           IProcessModel processModel,
                           IEventAggregator eventAggregator) : IDesignService
{
    private readonly IProcessModel _processModel = processModel ?? throw new ArgumentNullException(nameof(processModel));
    private readonly IContainerProvider _containerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
    private readonly IEventAggregator _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

    private SideWindow? BeamWindow { get; set; }

    public void ShowDesignWindow()
    {
        _processModel.IsDesignWindowOpen = true;
        BeamWindow ??= _containerProvider.Resolve<SideWindow>();
        if (BeamWindow is null)
            return;

        BeamWindow.Closing += BeamWindow_Closing;
        if (BeamWindow.DataContext == null)
            BeamWindow.DataContext = _containerProvider.Resolve<DesignViewModel>();
        if (!BeamWindow.IsActive)
            BeamWindow.Show();
        if (!BeamWindow.IsFocused)
            BeamWindow.Focus();
    }

    public void CloseDesignWindow()
    {
        _processModel.IsDesignWindowOpen = false;
        BeamWindow = _containerProvider.Resolve<SideWindow>();
        System.Windows.Application.Current.MainWindow.Focus();
        BeamWindow.Closing -= BeamWindow_Closing;
    }

    private void BeamWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _processModel.IsDesignWindowOpen = false;
        var closeEvent = _eventAggregator.GetEvent<BeamDesignWindowClosedEvent>();
        closeEvent.Publish();
    }
}

