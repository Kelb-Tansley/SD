using Hatch.Pdg.SD.ViewModels;
using Prism.Events;
using SD.Core.Shared.Events;
using SD.Element.Design.Interfaces;
using System.Windows;
using System.Windows.Threading;

namespace Hatch.Pdg.SD.Services;
public class SplashService : ISplashService
{
    private readonly AppShutdownEvent _shutdownEvent;

    public SplashService(IEventAggregator eventAggregator)
    {
        _shutdownEvent = eventAggregator.GetEvent<AppShutdownEvent>();
    }

    private Window SplashWindow { get; set; }
    private Dispatcher SplashThreadDispatcher { get; set; }

    public void ShowSplash<T>() where T : Window
    {
        var thread = new Thread(
            new ThreadStart(
                delegate ()
                {
                    SplashWindow = Activator.CreateInstance<T>();
                    SplashWindow.Show();
                    SplashThreadDispatcher = Dispatcher.CurrentDispatcher;
                    Dispatcher.Run();
                }
            ));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Priority = ThreadPriority.Highest;
        thread.IsBackground = true;
        thread.Start();
    }

    public void CloseSplash(bool shutDownApp)
    {
        if (SplashWindow != null && SplashThreadDispatcher != null)
        {
            //Close the window and shutdown the dispatcher.
            SplashThreadDispatcher.Invoke(() =>
            {
                SplashWindow.Close();
                SplashThreadDispatcher.InvokeShutdown();
            });
        }
        else if (SplashWindow != null && SplashThreadDispatcher == null)
            SplashWindow.Close();

        if (shutDownApp)
            _shutdownEvent.Publish();
    }

    public void SetMessageInSplash(string statusMessage)
    {
        if (SplashWindow == null || SplashThreadDispatcher == null) return;
        //use the current dispatcher to set the value in the screen.
        SplashThreadDispatcher.Invoke(() => ShowMessage(statusMessage));
    }

    public string GetStrand7LocationInSplash()
    {
        if (SplashWindow == null || SplashThreadDispatcher == null)
            return string.Empty;

        var location = string.Empty;

        //use the current dispatcher to get the value in the screen.
        SplashThreadDispatcher.Invoke(() =>
        {
            location = ((SplashViewModel)SplashWindow.DataContext).Strand7ApiFolder;
        });

        return location;
    }

    public void SetStrand7LocationErrorInSplash(string location)
    {
        if (SplashWindow == null || SplashThreadDispatcher == null) return;
        //use the current dispatcher to set the value in the screen.
        SplashThreadDispatcher.Invoke(() =>
        {
            if (string.IsNullOrWhiteSpace(((SplashViewModel)SplashWindow.DataContext).Strand7ApiFolder))
                ((SplashViewModel)SplashWindow.DataContext).Strand7ApiFolder = location;

            ((SplashViewModel)SplashWindow.DataContext).HasStrand7LocationError = true;
        });
    }

    private void ShowMessage(string statusMessage)
    {
        ((SplashViewModel)SplashWindow.DataContext).ApplicationStateMessage = statusMessage;
    }

    public void MinimizeSplashWindow()
    {
        if (SplashWindow == null || SplashThreadDispatcher == null)
            return;
        //use the current dispatcher to set the value in the screen.
        SplashThreadDispatcher.Invoke(() => SplashWindow.WindowState = WindowState.Minimized);
    }
}
