using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.UI.Models;
public partial class FileHistoryDisplayModel(string fileName, string fileCode, string filePath) : ObservableObject
{
    [ObservableProperty]
    public string _fileName = fileName;
    [ObservableProperty]
    public string _fileCode = fileCode;
    [ObservableProperty]
    public string _filePath = filePath;
}
