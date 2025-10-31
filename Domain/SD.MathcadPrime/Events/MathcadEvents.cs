using Ptc.MathcadPrime.Automation;
using System.Windows;

namespace SD.MathcadPrime.Events
{
    public class MathcadEvents : IMathcadPrimeEvents2
    {
        public bool ShowEvents = false;

        public void OnWorksheetSaved(string documentFullName)
        {
            if (ShowEvents)
                MessageBox.Show("Worksheet was saved: " + documentFullName);
        }

        public void OnWorksheetClosed(string documentFullName, string documentName)
        {
            if (ShowEvents)
                MessageBox.Show("Worksheet was closed:\n\n\t" + documentFullName + "\n\t" + documentName);
        }

        public void OnWorksheetModified(string documentFullNameArg, string documentNameArg, bool isModifiedArg)
        {
            if (ShowEvents)
                MessageBox.Show("Worksheet was modified:\n\n\t" + documentFullNameArg + "\n\t" + documentNameArg);
        }

        public void OnWorksheetRenamed(string previousFullName, string currentFullName, string previousDocName, string currentDocName)
        {
            if (ShowEvents)
                MessageBox.Show("Worksheet was renamed:\n\nFrom:t" + previousFullName + "\n\nTo:\t" + currentFullName);
        }

        public void OnExit()
        {
            if (ShowEvents)
                MessageBox.Show("Mathcad Prime is closed by User");
        }
    }
}
