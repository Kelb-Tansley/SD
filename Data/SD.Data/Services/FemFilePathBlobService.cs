using Newtonsoft.Json;
using SD.Core.Shared.Models.Core;
using SD.Data.Interfaces;

namespace SD.Data.Services;

public partial class FemFilePathBlobService(IAppSettings appSettings, IRuntimeAppSettings runtimeAppSettings) : BlobFileService, IFemFilePathBlobService
{
    private readonly IAppSettings _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly IRuntimeAppSettings _runtimeAppSettings = runtimeAppSettings ?? throw new ArgumentNullException(nameof(runtimeAppSettings));

    private readonly string BLOB_FILE_NAME = "FemFilePaths.csv";
    private readonly string STRAND_CONNECT_FILE_NAME = "StrandConnect.csv";
    private readonly string RUNTIME_SETTINGS_FILE_NAME = "RuntimeSettings.csv";

    private string DumpFolderPath => Environment.ExpandEnvironmentVariables(_appSettings.AppDataLocation);
    private string CsvFilePath => DumpFolderPath + BLOB_FILE_NAME;
    private string Strand7ConnectFilePath => DumpFolderPath + STRAND_CONNECT_FILE_NAME;

    public IRuntimeAppSettings? GetRuntimeSettings()
    {
        var text = ReadBlobFile(GetFilePath(RUNTIME_SETTINGS_FILE_NAME));
        if (string.IsNullOrEmpty(text))
            return null;

        var json = JsonConvert.DeserializeObject<RuntimeAppSettings>(text);
        return json;
    }

    public void SaveRuntimeSettings()
    {
        var jsonString = JsonConvert.SerializeObject(_runtimeAppSettings);
        StoreBlobFile(GetFilePath(RUNTIME_SETTINGS_FILE_NAME), jsonString);
    }

    public string? GetLastStrandApiPath()
    {
        var text = ReadBlobFile(GetFilePath(STRAND_CONNECT_FILE_NAME));
        if (string.IsNullOrEmpty(text))
            return null;

        text = text.Trim()?
            .Replace(@"\r", string.Empty)?
            .Replace(@"\n", string.Empty);
        return text;
    }

    private string GetFilePath(string fileName)
    {
        return DumpFolderPath + fileName;
    }

    public async Task InsertStrandApiPath(string path)
    {
        await StoreBlobFileAsync(Strand7ConnectFilePath, path);
    }

    public async Task<List<FemFile>> GetPreviousFemFiles()
    {
        try
        {
            CreateFileAndFolderIfNew(CsvFilePath);
            return await GetAllFemFiles(CsvFilePath);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<FemFile>> AddUpdateFilePathsAsync(string filePath)
    {
        var filePaths = new List<string>();
        try
        {
            var existingFiles = await GetPreviousFemFiles();

            var femFile = existingFiles.FirstOrDefault(ef => ef.FemModelFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            if (femFile != null)
                existingFiles.Remove(femFile);
            else
                femFile = new FemFile() { FemModelFilePath = filePath };

            existingFiles.Insert(0, femFile);

            await AddAllFemFiles(CsvFilePath, existingFiles);

            return existingFiles;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task AddAllFemFiles(string filePath, List<FemFile> existingFiles)
    {
        var csv = new StringBuilder();
        existingFiles.ForEach(file => { csv.AppendLine(file.FemModelFilePath); });
        await File.WriteAllTextAsync(filePath, csv.ToString());
    }


    private static async Task<List<FemFile>> GetAllFemFiles(string filePath)
    {
        var files = await File.ReadAllLinesAsync(filePath);
        var femFiles = new List<FemFile>();
        foreach (var file in files)
            femFiles.Add(new FemFile() { FemModelFilePath = file });

        return femFiles;
    }

    public void OpenCsvResultsFile(string filePath)
    {
        OpenBlobFile(filePath);
    }

    public async Task StoreCsvResultsFileAsync(string fileName, List<string> content)
    {
        await StoreBlobFileAsync(fileName, content);
    }
}