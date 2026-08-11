using SD.Core.Shared.Models.BeamModels;
using System.Reflection;

namespace SD.Tests.Strand7.Helpers;

public static class SectionAssertions
{
    public static void AssertSectionsAreEqual(SectionProperties a, SectionProperties b, double tolerance)
    {
        var props = typeof(SectionProperties)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(double));

        Console.WriteLine();
        Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Property                            A Value                B Value                   Diff                    Tol ");
        Console.WriteLine("----------------------------------------------------------------------------------------------------------------");

        foreach (var prop in props)
        {
            double av = (double)prop.GetValue(a)!;
            double bv = (double)prop.GetValue(b)!;
            double diff = Math.Abs(av - bv);
            var tol = Math.Min(Math.Abs(av) * tolerance, Math.Abs(bv) * tolerance);

            string avStr = FormatSmart(av);
            string bvStr = FormatSmart(bv);
            string diffStr = FormatSmart(diff);
            string tolStr = FormatSmart(tol);
            Console.WriteLine($"{prop.Name,-20} {avStr,22} {bvStr,22} {diffStr,22} {tolStr,22}");

            if (av == 0 || bv == 0)
                continue;

            diff.Should().BeLessThanOrEqualTo(tol, $"Property {prop.Name} should match within tolerance.");
        }
    }

    private static string FormatSmart(double value)
    {
        // If integer, show no decimals
        if (Math.Abs(value % 1) < 1e-9)
            return value.ToString("0");

        // Otherwise show up to 6 decimals, trimming trailing zeros
        return value.ToString("0.######");
    }
}