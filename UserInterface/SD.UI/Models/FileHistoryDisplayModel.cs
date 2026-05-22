using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.UI.Models;
public partial class FileHistoryDisplayModel(string fileName, string fileCode, string filePath) : ObservableObject
{
    [ObservableProperty]
    public partial string FileName { get; set; } = fileName;

    [ObservableProperty]
    public partial string FileCode { get; set; } = fileCode;

    [ObservableProperty]
    public partial string FilePath { get; set; } = filePath;
}
