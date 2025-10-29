using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Extensions;
public static class ZoomLevelExtension
{
    public static ZoomLevel ZoomIn(this ZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            ZoomLevel.Level0 => ZoomLevel.Level1,
            ZoomLevel.Level1 => ZoomLevel.Level2,
            ZoomLevel.Level2 => ZoomLevel.Level3,
            ZoomLevel.Level3 => ZoomLevel.Level4,
            ZoomLevel.Level4 => ZoomLevel.Level5,
            ZoomLevel.Level5 => ZoomLevel.Level5,
            _ => ZoomLevel.Level2,
        };
    }
    public static ZoomLevel ZoomOut(this ZoomLevel zoomLevel)
    {
        return zoomLevel switch
        {
            ZoomLevel.Level0 => ZoomLevel.Level0,
            ZoomLevel.Level1 => ZoomLevel.Level0,
            ZoomLevel.Level2 => ZoomLevel.Level1,
            ZoomLevel.Level3 => ZoomLevel.Level2,
            ZoomLevel.Level4 => ZoomLevel.Level3,
            ZoomLevel.Level5 => ZoomLevel.Level4,
            _ => ZoomLevel.Level2,
        };
    }
}
