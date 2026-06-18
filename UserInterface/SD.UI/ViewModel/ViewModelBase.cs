using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;

namespace SD.UI.ViewModel;
public partial class ViewModelBase(IProcessModel processModel) : ObservableObject
{
    [ObservableProperty]
    public partial IProcessModel ProcessModel { get; set; } = processModel ?? throw new ArgumentNullException(nameof(processModel));

    [ObservableProperty]
    public partial bool IsThisProcessRunning { get; set; } = false;

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
