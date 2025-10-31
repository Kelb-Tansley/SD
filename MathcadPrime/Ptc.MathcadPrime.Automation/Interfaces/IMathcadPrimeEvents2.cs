namespace Ptc.MathcadPrime.Automation
{
    public interface IMathcadPrimeEvents2
    {
        public void OnWorksheetSaved(string documentFullName);
        public void OnWorksheetClosed(string documentFullName, string documentName);
    }
}
