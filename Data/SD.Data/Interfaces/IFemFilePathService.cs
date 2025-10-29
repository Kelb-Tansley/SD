namespace SD.Data.Interfaces;
public interface IFemFilePathService
{
    public string? GetLastStrandApiPath();
    public Task InsertStrandApiPath(string path);
    /// <summary>
    /// Adds or updates the file paths asynchronously. This is used for the Strand7 file paths displayed on the home page.
    /// </summary>
    /// <param name="filePath">The file path to add or update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="FemFile"/>.</returns>
    public Task<List<FemFile>> AddUpdateFilePathsAsync(string filePath);
    public Task<List<FemFile>> GetPreviousFemFiles();
    public void OpenBlobFile(string filePath);
    public IRuntimeAppSettings? GetRuntimeSettings();
    public void SaveRuntimeSettings();
    public Task StoreBlobFileAsync(string fileName, List<string> content);
}
