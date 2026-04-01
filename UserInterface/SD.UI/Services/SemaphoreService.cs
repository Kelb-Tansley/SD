namespace SD.UI.Services;

public static class SemaphoreService
{
    public static async Task RunInBackgroundAsync(this SemaphoreSlim semaphore, Action action)
    {
        await semaphore.WaitAsync();
        try
        {
            await Task.Run(action);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
