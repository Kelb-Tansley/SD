namespace SD.Fem.Strand7.Helpers;

internal static class DoubleHelper
{
    public static double ZeroIfTiny(this double x, double tol = 1e-12)
    {
        return Math.Abs(x) < tol ? 0.0 : x;
    }
}
