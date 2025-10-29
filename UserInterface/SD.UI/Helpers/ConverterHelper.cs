namespace SD.UI.Helpers;
public static class ConverterHelper
{
    public static string GetHashes(object parameter)
    {
        if (parameter is string parString)
        {
            _ = int.TryParse(parString, out var parInt2);
            if (parInt2 is int decimalPoints && decimalPoints > 0)
            {
                var hashes = string.Empty;
                for (int i = 1; i <= decimalPoints; i++)
                    hashes += "#";

                return hashes;
            }
        }
        return "##";
    }
}
