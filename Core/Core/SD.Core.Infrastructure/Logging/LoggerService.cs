using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System.IO;
using System.Reflection;

namespace SD.Core.Infrastructure.Logging;

public class LoggerService : ILoggerService
{
    public LoggerService()
    {
        var logRepository = SetLogRepository();
        XmlConfigurator.Configure(logRepository, new FileInfo("Logger.config"));
    }
    
    public LoggerService(string logFileName)
    {
        var logRepository = SetLogRepository();
        XmlConfigurator.Configure(logRepository, new FileInfo(logFileName));
    }

    private static ILoggerRepository SetLogRepository()
    {
        var userprofile = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        GlobalContext.Properties["UserProfilePath"] = userprofile;
        return logRepository;
    }

    /// <summary>
    /// Log information is logged to configured location. 
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    public void LogInfo(Type category, string message)
    {
        var log = LogManager.GetLogger(category);
        if (log.IsInfoEnabled)
            log.Info(message);
    }

    /// <summary>
    /// Log error is logged to configured location. 
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>

    public void LogError(Type category, string message, Exception exception = null)
    {
        var log = LogManager.GetLogger(category);
        if (log.IsErrorEnabled) log.Error(message, exception);
    }

    /// <summary>
    /// Log level information is logged to configured location. 
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    /// <param name="level"></param>
    /// <param name="exception"></param>
    public void Log(Type category, string message, LogLevel level, Exception exception = null)
    {
        var log = LogManager.GetLogger(category);
        switch (level)
        {
            case LogLevel.FATAL:
                if (log.IsFatalEnabled) log.Fatal(message, exception);
                break;
            case LogLevel.ERROR:
                if (log.IsErrorEnabled) log.Error(message, exception);
                break;
            case LogLevel.WARN:
                if (log.IsWarnEnabled) log.Warn(message);
                break;
            case LogLevel.INFO:
                if (log.IsInfoEnabled) log.Info(message);
                break;
            case LogLevel.DEBUG:
                if (log.IsDebugEnabled) log.Debug(message);
                break;
        }
    }

    /// <summary>
    /// Assists by returning the current app configured log location.
    /// </summary>
    /// <returns></returns>
    public string GetLogFileLocation()
    {
        var rootAppender = LogManager.GetRepository(Assembly.GetEntryAssembly())
                                     .GetAppenders()
                                     .OfType<RollingFileAppender>()
                                     .FirstOrDefault();
        return rootAppender?.File ?? string.Empty;
    }
}
