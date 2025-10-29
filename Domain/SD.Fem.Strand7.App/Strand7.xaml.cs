using Microsoft.Extensions.Configuration;
using SD.Core.Infrastructure.Logging;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.Core;
using SD.Data.Services;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SD.Fem.Strand7.App;
public partial class Strand7
{
    private readonly LoggerService _loggerService;
    private readonly FemFilePathService _femFilePathService;
    private IAppSettings _appSettings;

    public Strand7()
    {
        RegisterConfigSettings();

        ArgumentNullException.ThrowIfNull(_appSettings);
        _femFilePathService = new FemFilePathService(_appSettings, new RuntimeAppSettings());
        _loggerService = new LoggerService("Strand7Logger.config");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var firstArg = e.Args?.FirstOrDefault();
        if (firstArg != null)
            _loggerService.LogInfo(GetType(), $"Args received: {firstArg}");

        try
        {
            var parsed = int.TryParse(firstArg, out var delay);
            if (!parsed)
                delay = 5000;

            var settings = _femFilePathService.GetRuntimeSettings();
            if (settings == null)
            {
                _loggerService.LogError(GetType(), $"No application runtime settings found.");
                return;
            }

            _loggerService.LogInfo(GetType(), $"Requires restart: {settings.RequiresRestart}");
            if (settings.RequiresRestart)
            {
                //Allow some time for old processes to die
                Task.Delay(delay).GetAwaiter().GetResult();

                Process.Start(new ProcessStartInfo($"Hatch.Pdg.SD.exe")
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }

            settings.RequiresRestart = false;
            _femFilePathService.SaveRuntimeSettings();
        }
        catch (Exception ex)
        {
            _loggerService.LogError(GetType(), $"Error occured: {ex.Message}");
        }

        Environment.Exit(0);
    }

    private void RegisterConfigSettings()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var appSettings = new AppSettings();
        configuration.GetSection("App").Bind(appSettings);
        appSettings.Initialize();
        _appSettings = appSettings;
    }
}
