namespace Ptc.MathcadPrime.Automation
{
    public interface IMathcadPrimeWorksheet3
    {
        public bool SaveAs(string filePath);
        public bool Save();
        public bool Close(SaveOption saveOption);
        public RealValue OutputGetRealValue(string input);
        public void SetRealValue(string input, double value, string unit);
        public void PauseCalculation();
        public void ResumeCalculation();
    }
}
