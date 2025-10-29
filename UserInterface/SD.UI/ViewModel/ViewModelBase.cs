using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.ViewModel;
public partial class ViewModelBase(IProcessModel processModel) : ObservableObject
{
    [ObservableProperty]
    public IProcessModel _processModel = processModel ?? throw new ArgumentNullException(nameof(processModel));

    [ObservableProperty]
    public bool _isThisProcessRunning = false;

    public async Task SetPrimaryProcess(bool isCompleted = false, bool longerDelay = false, bool thisProcessOnly = false)
    {
        try
        {
            if (!thisProcessOnly)
                ProcessModel.IsPrimaryProcessRunning = true;

            IsThisProcessRunning = true;

            if (isCompleted)
            {
                if (!thisProcessOnly)
                    ProcessModel.IsPrimaryProcessRunning = false;

                IsThisProcessRunning = false;
                return;
            }
            if (longerDelay)
                await Task.Delay(400);
            else
                await Task.Delay(200);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
