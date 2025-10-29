using SD.Core.Shared.Models;
using SD.UI.Models;
using System.IO;

namespace SD.UI.Extensions;
public static class FileHistoryExtensions
{
    public static List<FileHistoryDisplayModel> ToFileHistoryDisplayModels(this List<FemFile> femFiles)
    {
        var fileHistoryDisplayModels = new List<FileHistoryDisplayModel>();
        foreach (var femFile in femFiles)
        {
            var fileName = FileNameFromPath(femFile.FemModelFilePath);
            fileHistoryDisplayModels.Add(new FileHistoryDisplayModel(
                fileName,
                fileName[0].ToString(),
                femFile.FemModelFilePath));
        }
        return fileHistoryDisplayModels;
    }
    private static string FileNameFromPath(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return fileInfo.Name;
    }
}
