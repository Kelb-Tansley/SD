namespace Ptc.MathcadPrime.Automation
{
    public class ApplicationCreator : IMathcadPrimeApplication3
    {
        public bool Visible { get; set; }
        public IMathcadPrimeWorksheet3 ActiveWorksheet { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public int InitializeEvents2(IMathcadPrimeEvents2 events, bool someBool)
        {
            throw new NotImplementedException();
        }

        public IMathcadPrimeWorksheet3 Open(string templateFilePath)
        {
            throw new NotImplementedException();
        }

        public void Quit(SaveOption saveOption)
        {
            throw new NotImplementedException();
        }
    }
}
