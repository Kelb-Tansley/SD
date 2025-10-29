namespace SD.Core.Shared.Extensions;
public static class StringExtensions
{
    public static string ToUpperTrimmed(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        else
            return input.ToUpper().Replace(" ", string.Empty);
    }
}
