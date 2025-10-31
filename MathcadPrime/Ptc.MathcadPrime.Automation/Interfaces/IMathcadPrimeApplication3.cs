namespace Ptc.MathcadPrime.Automation
{
    public interface IMathcadPrimeApplication3
    {
        public void Quit(SaveOption saveOption);
        public int InitializeEvents2(IMathcadPrimeEvents2 events, bool someBool);
        public IMathcadPrimeWorksheet3 Open(string templateFilePath);
        public void Activate();
        public bool Visible { get; set; }
        public IMathcadPrimeWorksheet3 ActiveWorksheet { get; set; }
    }
}
