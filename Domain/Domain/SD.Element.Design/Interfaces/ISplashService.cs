using System.Windows;

namespace SD.Element.Design.Interfaces;
public interface ISplashService
{
    void ShowSplash<T>() where T : Window;
    void CloseSplash(bool shutDownApp);
    void SetMessageInSplash(string statusMessage);
    string GetStrand7LocationInSplash();
    void SetStrand7LocationErrorInSplash(string location);
    void MinimizeSplashWindow();
}
