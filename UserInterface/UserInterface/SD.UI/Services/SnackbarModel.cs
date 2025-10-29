using CommunityToolkit.Mvvm.ComponentModel;
using SD.Element.Design.Interfaces;

namespace SD.UI.Services;

public partial class SnackbarModel : ObservableObject, ISnackbarModel
{
    [ObservableProperty]
    public string message = string.Empty;

    [ObservableProperty]
    public bool isActive = false;

    public void ShowMessage(string message, int delay)
    {
        Message = message;
        IsActive = true;
        CloseAfterDelay(delay);
    }

    private async void CloseAfterDelay(int delay)
    {
        await Task.Delay(delay);
        lock (this)
        {
            if (IsActive)
                IsActive = false;
        }
    }
}
