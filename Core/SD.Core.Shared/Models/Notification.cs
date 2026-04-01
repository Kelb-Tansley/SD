using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models;

public class Notification(string title, string description, WarningLevel warningLevel = WarningLevel.Error, int timer = 3000)
    : ShortNotification(description, timer)
{
    public string Title { get; set; } = title;
    public WarningLevel WarningLevel { get; set; } = warningLevel;
}

public class ShortNotification(string description, int timer = 3000)
{
    public string Description { get; set; } = description;
    public int Timer { get; set; } = timer;
}