namespace SD.UI.Services.Models;
public class Notification(string title, string description, WarningLevel warningLevel = WarningLevel.Error)
{
    public string Description { get; set; } = description;
    public string Title { get; set; } = title;
    public WarningLevel WarningLevel { get; set; } = warningLevel;
}