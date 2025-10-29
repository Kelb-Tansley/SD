namespace SD.Core.Infrastructure.Logging;

/// <summary>
/// The logger interface to log messages and exceptions.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// All Log level information are logged. 
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    /// <param name="level"></param>
    /// <param name="exception"></param>
    public void Log(Type category, string message, LogLevel level, Exception exception = null);
    public void LogInfo(Type category, string message);
    public void LogError(Type category, string message, Exception exception = null);

    public string GetLogFileLocation();
}
