using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models;
public class Notification(string title, string description, WarningLevel warningLevel = WarningLevel.Error, int timer = 3000)
{
    public string Description { get; set; } = description;
    public string Title { get; set; } = title;
    public WarningLevel WarningLevel { get; set; } = warningLevel;
    public int Timer { get; set; } = timer;
}