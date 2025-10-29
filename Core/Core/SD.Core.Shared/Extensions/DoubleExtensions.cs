namespace SD.Core.Shared.Extensions;
public static class DoubleExtensions
{
    public static double DegreesToRadians(this double degrees)
    {
        return degrees * Math.PI / 180D;
    }
    public static double RadiansToDegrees(this double radians)
    {
        return radians * 180D / Math.PI;
    }
    public static double GetMinimumNonZero(double a, double b, double? c = null)
    {
        var min = double.MaxValue;

        // Compare each property, ignoring those that are 0
        if (a != 0 && a < min) min = a;
        if (b != 0 && b < min) min = b;
        if (c != null && c != 0 && c < min) min = (double)c;

        // If min is still double.MaxValue, all values were 0
        return min == double.MaxValue ? 0 : min;
    }
    public static double MinZero(this double value)
    {
        return value == 0 ? double.Epsilon : value;
    }

    public static double Cut(this double value, int decimalPlaces = 2)
    {
        return Math.Round(value, decimalPlaces);
    }
}
