using System.Diagnostics;

namespace SD.Data.Services;

public abstract class BlobFileService
{
    protected static string ReadBlobFile(string fileName)
    {
        var content = string.Empty;
        if (File.Exists(fileName))
            content = File.ReadAllText(fileName);

        return content;
    }
    protected static void StoreBlobFile(string fileName, string content)
    {
        var csv = CreateCsvFileAndContent(fileName, [content]);
        File.WriteAllText(fileName, csv.ToString());
    }
    protected static async Task StoreBlobFileAsync(string fileName, string content)
    {
        var csv = CreateCsvFileAndContent(fileName, [content]);
        await File.WriteAllTextAsync(fileName, csv.ToString());
    }
    protected static async Task StoreBlobFileAsync(string fileName, List<string> content)
    {
        var csv = CreateCsvFileAndContent(fileName, content);
        await File.WriteAllTextAsync(fileName, csv.ToString());
    }
    protected static StringBuilder CreateCsvFileAndContent(string fileName, List<string> content)
    {
        CreateFileAndFolderIfNew(fileName);
        var csv = new StringBuilder();
        content.ForEach(line => { csv.AppendLine(line); });
        return csv;
    }
    protected static void CreateFileAndFolderIfNew(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists && fileInfo.Directory != null)
            {
                fileInfo.Directory.Create();

                if (!File.Exists(filePath))
                {
                    var stream = File.Create(filePath);
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    protected static void OpenBlobFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        };

        Process.Start(processStartInfo);
    }
}