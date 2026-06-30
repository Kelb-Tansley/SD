using Microsoft.Win32;
using Ptc.MathcadPrime.Automation;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.MathcadPrime.Events;
using SD.MathcadPrime.Interfaces;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using SD.Core.Shared.Contracts;
using SD.Core.Infrastructure.Interfaces;

namespace SD.MathcadPrime.Services;
public abstract class MathcadService(INotificationService notificationService, IAppSettings appSettings) : IMathcadService
{
    protected readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    private readonly IAppSettings _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    protected IMathcadPrimeApplication3? _mathcadPrimeApplication;
    protected IMathcadPrimeWorksheet3? _mathcadPrimeWorksheet;
    protected readonly IMathcadPrimeEvents2 _mathcadPrimeEvents = new MathcadEvents();

    private static int CommonItemCount { get; set; }

    private const string MatchCadFileExtension = ".mcdx";

    public void CloseMathcad()
    {
        // Return if Mathcad is not initialized.
        if (_mathcadPrimeApplication == null)
            return;

        try
        {
            CloseWorksheet();

            _mathcadPrimeApplication.Quit(SaveOption.spDiscardChanges);
            ReleaseComObjects();
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Can not close Mathcad Prime, make sure it is actually running"));
        }
    }

    public void SaveWorksheet()
    {
        if (_mathcadPrimeApplication == null || _mathcadPrimeWorksheet == null)
            return;

        try
        {
            _mathcadPrimeWorksheet.Save();
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Can not save worksheet. If it is 'Untitled' Use SaveAs"));
        }

    }

    public string GetMathCadFileName(SectionType sectionType)
    {
        return sectionType switch
        {
            SectionType.IorH => $"I or H{MatchCadFileExtension}",
            SectionType.LipChannel => $"PFC{MatchCadFileExtension}",
            SectionType.Angle => $"EA or UA{MatchCadFileExtension}",
            SectionType.CircularHollow => $"CHS{MatchCadFileExtension}",
            SectionType.RectangularHollow => $"RHS{MatchCadFileExtension}",
            SectionType.T => $"T{MatchCadFileExtension}",
            _ => throw new NotImplementedException(),
        };
    }

    public SectionCapacity ReadMathcadTensionResults(string mathcadSheet)
    {
        return new SectionCapacity()
        {
            Tr = _mathcadPrimeWorksheet.OutputGetRealValue("Nt").RealResult
        };
    }

    public SectionCapacity ReadMathcadShearResults(string mathcadSheet)
    {
        return new SectionCapacity()
        {
            VrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Vvmx").RealResult,
            VrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Vvmy").RealResult
        };
    }

    public SectionCapacity ReadMathcadCompressionResults(string mathcadSheet)
    {
        return new SectionCapacity()
        {
            Cr = Math.Min(_mathcadPrimeWorksheet.OutputGetRealValue("Ncx").RealResult, _mathcadPrimeWorksheet.OutputGetRealValue("Ncy").RealResult)
        };
    }

    public SectionCapacity ReadMathcadBendingResults(string mathcadSheet)
    {
        return new SectionCapacity()
        {
            MrMajor = _mathcadPrimeWorksheet.OutputGetRealValue("Mbx").RealResult,
            MrMinor = _mathcadPrimeWorksheet.OutputGetRealValue("Msy").RealResult
        };
    }
    protected bool CopyAndOpenTemplateWorksheet(string? templateFile, SectionType sectionType)
    {
        if (string.IsNullOrWhiteSpace(templateFile))
            templateFile = GetMathCadFullFileName(sectionType);

        if (!File.Exists(templateFile))
            return false;

        if (!CopyFile(ref templateFile))
            return false;

        StartMathcad();

        return OpenTemplateWorksheet(templateFile);
    }

    /// <summary>
    /// This method is used to instantiate the Mathcad Prime application COM object.
    /// </summary>
    protected void StartMathcad()
    {
        // Do not continue if Mathcad prime is already initialized
        if (_mathcadPrimeApplication != null)
            return;

        try
        {
            _mathcadPrimeApplication = new ApplicationCreator { Visible = false };

            if (_mathcadPrimeApplication == null)
            {
                _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Problem to connect Mathcad Prime"));
                return;
            }

            var initialized = _mathcadPrimeApplication.InitializeEvents2(_mathcadPrimeEvents, true);
            if (initialized != 0)
                _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Events initialization failed"));
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Problem to connect Mathcad Prime: " + ex.Message));
        }
    }

    protected bool CopyFile(ref string templateFile)
    {
        var guid = Guid.NewGuid();
        var name = templateFile[templateFile.LastIndexOf('\\')..];
        name = name.Insert(name.LastIndexOf(MatchCadFileExtension), $"-{guid}");

        var filePath = Path.Join(_appSettings.MathcadLocation, name);

        File.Copy(templateFile, filePath, true);
        if (!File.Exists(filePath))
            return false;

        templateFile = filePath;
        return true;
    }

    protected bool OpenTemplateWorksheet(string templateFilePath)
    {
        if (_mathcadPrimeApplication == null || string.IsNullOrWhiteSpace(templateFilePath))
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Couldn't open Mathcad Prime template file. Check that Mathcad Prime is running and you have proper permissions."));
            return false;
        }

        try
        {
            _mathcadPrimeWorksheet = _mathcadPrimeApplication.Open(templateFilePath) as IMathcadPrimeWorksheet3;
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Couldn't open Mathcad Prime file. Check that Mathcad Prime is running and you have proper permissions"));
            return false;
        }

        return true;
    }

    protected void ShowMathcad()
    {
        if (_mathcadPrimeApplication == null)
            return;

        try
        {
            _mathcadPrimeApplication.Visible = true;

            // Activate Prime to bring it forward
            _mathcadPrimeApplication.Activate();
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Can not Activate worksheet. Make sure Mathcad Prime is running and worksheet is loaded"));
        }
    }

    private string OpenWorksheet(string? filePath = null)
    {
        if (_mathcadPrimeApplication == null)
            return string.Empty;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            var fileOpenDialog = new OpenFileDialog
            {
                Filter = @"mcdx files (*.mcdx)|.mcdx|All files (*.*)|*.*",
                FilterIndex = 2
            };
            if (fileOpenDialog.ShowDialog() == true)
            {
                filePath = fileOpenDialog.FileName;
            }
        }

        try
        {
            _mathcadPrimeWorksheet = _mathcadPrimeApplication.Open(filePath) as IMathcadPrimeWorksheet3;
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Couldn't open Mathcad Prime file. Check that Mathcad Prime is running and you have proper permissions"));
            return string.Empty;
        }

        return filePath;
    }

    protected string? SaveAsWorksheet(string fileName, bool openAfterSave)
    {
        if (_mathcadPrimeApplication == null || _mathcadPrimeWorksheet == null)
            return null;

        var newFileName = GetNotExistingFileName(fileName);
        try
        {
            _mathcadPrimeWorksheet.SaveAs(newFileName);
        }
        catch
        {
            throw new ArgumentException("Unable to save worksheet. Check permissions");
        }

        if (openAfterSave)
            OpenWorksheet(newFileName);

        _notificationService.ShowSnackNotification(new Notification("Success", $"Worksheet saved as: {newFileName}"));
        return newFileName;
    }

    private static string GetNotExistingFileName(string fileName)
    {
        var notExistingFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);

        // Check if file exists and append with number if necessary
        int count = 1;
        var fileNameOnly = Path.GetFileNameWithoutExtension(notExistingFileName);
        var extension = Path.GetExtension(notExistingFileName);
        var path = Path.GetDirectoryName(notExistingFileName);

        while (!string.IsNullOrWhiteSpace(path) && File.Exists(notExistingFileName))
            notExistingFileName = Path.Combine(path, $"{fileNameOnly} ({count++}){extension}");

        return notExistingFileName;
    }

    private void CloseWorksheet()
    {
        if (_mathcadPrimeApplication == null || _mathcadPrimeWorksheet == null)
            return;

        try
        {
            _mathcadPrimeWorksheet.Close(SaveOption.spDiscardChanges);
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Unable to close worksheet. Check that Mathcad Prime is running"));
        }

        if (_mathcadPrimeWorksheet != null && Marshal.IsComObject(_mathcadPrimeWorksheet))
            Marshal.FinalReleaseComObject(_mathcadPrimeWorksheet);

        _mathcadPrimeWorksheet = null;
    }

    protected string GetMathCadFullFileName(SectionType sectionType)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(thisLocation);

        var fileName = GetMathCadFileName(sectionType);

        return Path.Combine(thisLocation, $@"Templates\{fileName}");
    }

    private void ReleaseComObjects()
    {
        if (_mathcadPrimeWorksheet != null && Marshal.IsComObject(_mathcadPrimeWorksheet))
            Marshal.FinalReleaseComObject(_mathcadPrimeWorksheet);

        _mathcadPrimeWorksheet = null;

        if (_mathcadPrimeApplication != null && Marshal.IsComObject(_mathcadPrimeApplication))
            Marshal.FinalReleaseComObject(_mathcadPrimeApplication);

        _mathcadPrimeApplication = null;
    }

    private static string GetMathCadTemplateName(Beam beam, DesignType designType, ElementClass? sectionClass, bool getPartial = false)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(thisLocation);

        // Determine Mathcad unique file name and input Mathcad file
        var sectionName = string.Empty;
        switch (beam.Section.SectionType) //Determines which section types this program allows to design
        {
            case SectionType.IorH:
                sectionName = "I or H";
                break;
            case SectionType.CircularHollow:
                sectionName = "CHS_";
                break;
            case SectionType.RectangularHollow:
                sectionName = "RHS_";
                break;
            case SectionType.LipChannel:
                sectionName = "C_";
                break;
            case SectionType.Angle:
                sectionName = "EA_";
                break;
            case SectionType.T:
                sectionName = "T_";
                break;
            case SectionType.Unknown:
            default:
                break;
        }

        var classCode = sectionClass != ElementClass.Class4 ? "A" : "B";
        var fileName = $"{sectionName}{(int)designType}_{classCode}";

        return getPartial ? fileName : Path.Combine(thisLocation, $@"Templates\{fileName}{MatchCadFileExtension}");
    }

    private static void CheckFileExists(string OriginalPathResult, string PathResult, string sectionname, out string TruePathResult)
    {
        TruePathResult = OriginalPathResult;
        if (File.Exists(OriginalPathResult))
        {
            CommonItemCount++;
            TruePathResult = TruePathResult.Substring(0, TruePathResult.LastIndexOf("\\")) + TruePathResult.Substring(TruePathResult.LastIndexOf("\\"), TruePathResult.Length - TruePathResult.LastIndexOf("\\") - 7) + "_" + CommonItemCount.ToString() + MatchCadFileExtension;
            CheckFileExists(TruePathResult, PathResult, sectionname, out TruePathResult);
        }
        CommonItemCount = 0;
    }

    #region Not Used
    private void DisableEvents(object sender, RoutedEventArgs e)
    {
        if (_mathcadPrimeEvents != null && _mathcadPrimeEvents is MathcadEvents)
            ((MathcadEvents)_mathcadPrimeEvents).ShowEvents = false;
    }

    private void HideMathcad()
    {
        if (_mathcadPrimeApplication != null)
            try
            {
                _mathcadPrimeApplication.Visible = false;
            }
            catch
            {
                _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Can not make Mathcad Prime visible. Make sure Mathcad Prime is running."));
            }
    }

    private void GetActiveWorksheet()
    {
        if (_mathcadPrimeApplication != null)
            _mathcadPrimeWorksheet = _mathcadPrimeApplication.ActiveWorksheet as IMathcadPrimeWorksheet3;
    }

    private void OpenNewWorksheet()
    {
        if (_mathcadPrimeApplication != null)
            _mathcadPrimeWorksheet = _mathcadPrimeApplication.Open(string.Empty) as IMathcadPrimeWorksheet3;
    }

    private void EnableEvents()
    {
        try
        {
            if (_mathcadPrimeEvents != null && _mathcadPrimeEvents is MathcadEvents)
                ((MathcadEvents)_mathcadPrimeEvents).ShowEvents = true;
        }
        catch
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", "Can not initial events. Make sure Mathcad Prime is running and worksheet is loaded"));
        }
    }

    private static string GetMathCadTemplateName(Beam beam, DesignType designType, int? sectionClass, bool getPartial = false)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(thisLocation);

        // Determine Mathcad unique file name and input Mathcad file
        var sectionName = string.Empty;
        switch (beam.Section.SectionType) //Determines which section types this program allows to design
        {
            case SectionType.IorH:
                sectionName = "I_";
                break;
            case SectionType.CircularHollow:
                sectionName = "CHS_";
                break;
            case SectionType.RectangularHollow:
                sectionName = "RHS_";
                break;
            case SectionType.LipChannel:
                sectionName = "C_";
                break;
            case SectionType.Angle:
                sectionName = "EA_";
                break;
            case SectionType.T:
                sectionName = "T_";
                break;
            case SectionType.Unknown:
            default:
                break;
        }

        var classCode = sectionClass < 4 ? "A" : "B";
        var fileName = $"{sectionName}{(int)designType}_{classCode}";

        return getPartial ? fileName : Path.Combine(thisLocation, $@"Templates\{fileName}{MatchCadFileExtension}");
    }

    private static string GetMathCadFullFileName(Beam beam, DesignType designType, ElementClass sectionClass)
    {
        // Determine Mathcad unique file name and input Mathcad file
        var sectionName = GetMathCadTemplateName(beam, designType, sectionClass, true);

        var fileCount = 1;
        string exportFilePath = $"{sectionName}_{beam.Number}_{fileCount}{MatchCadFileExtension}";

        CheckFileExists(exportFilePath, string.Empty, sectionName, out exportFilePath);

        return exportFilePath;
    }
    #endregion
}