using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Constants;
public static class DesignCodes
{
    public const string SansDesignCode = DesignServiceTypes.SansDesign;
    public const string ASDesignCode = DesignServiceTypes.ASDesign;
    public static string[] AllDesignCodes => [SansDesignCode, ASDesignCode];

    public static bool IsAs(string code) => code == ASDesignCode || code == DesignCode.AS.ToString();
    public static bool IsSans(string code) => code == SansDesignCode || code == DesignCode.SANS.ToString();

    public static DesignCode ToDesignCodeEnum(this string designCode)
    {
        return IsAs(designCode) ? DesignCode.AS : DesignCode.SANS;
    }
}
