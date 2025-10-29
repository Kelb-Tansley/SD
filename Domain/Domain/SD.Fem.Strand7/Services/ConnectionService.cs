using Newtonsoft.Json.Linq;
using SD.Core.Infrastructure.Logging;
using SD.Data.Interfaces;
using System.Diagnostics;
using System.Net.Http;
using SD.Core.Shared.Events;

namespace SD.Fem.Strand7.Services;
public class ConnectionService(
    IFemFilePathService femFilePathService,
    IRuntimeAppSettings runtimeAppSettings,
    IAppSettings appSettings,
    ISplashService splashService,
    ILoggerService loggerService,
    IEventAggregator eventAggregator)
    : IConnectionService
{
    private readonly AppRestartEvent _appRestartEvent = eventAggregator.GetEvent<AppRestartEvent>();

    public bool IsApiConnected { get; set; }

    public bool ConnectToStrand7Api()
    {
        ArgumentNullException.ThrowIfNull(appSettings.Strand7Api?.Paths);

        splashService.SetMessageInSplash("Locating Strand7 API and licensing information...");

        var lastLocation = femFilePathService.GetLastStrandApiPath();
        if (!string.IsNullOrEmpty(lastLocation))
            return ConnectToStrand7Api(lastLocation);

        //First attempt to connect to the default installation location
        var defaultLocation = appSettings.Strand7Api.Paths.FirstOrDefault(path => !string.IsNullOrEmpty(path.Location) && path.Location.Equals("Default"))?.Value;
        var defaultConnect = ConnectToStrand7Api(defaultLocation, false);
        if (defaultConnect)
            return true;

        //If the default location did not work then use a default server location and restart the app
        var serverLocation = Task.Run(GetLocationByCountryIp).GetAwaiter().GetResult();
        if (!string.IsNullOrEmpty(serverLocation))
            femFilePathService.InsertStrandApiPath(serverLocation);

        RestartApp();

        return false;
    }

    private void RestartApp()
    {
        runtimeAppSettings.RequiresRestart = true;
        _appRestartEvent.Publish();
    }

    public string? GetUserCurrentCountry()
    {
        var ipAddressTask = Task.Run(() => GetPublicIpAddressAsync());
        ipAddressTask.Wait();
        var ipAddress = ipAddressTask.Result;

        var countryTask = Task.Run(() => GetCountryByIpAsync(ipAddress));
        countryTask.Wait();
        return countryTask.Result;
    }

    private async Task<string?> GetLocationByCountryIp()
    {
        var myCountry = string.Empty;

        var ipAddress = await GetPublicIpAddressAsync();
        if (!string.IsNullOrEmpty(ipAddress))
            myCountry = await GetCountryByIpAsync(ipAddress);

        return appSettings.Strand7Api?.Paths.FirstOrDefault(path =>
                !string.IsNullOrEmpty(path.Location) &&
                path.Location.Equals(myCountry, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public async Task<string?> GetCountryByIpAsync(string? ipAddress)
    {
        try
        {
            var client = new HttpClient();
            var url = $"https://ipinfo.io/{ipAddress}/json";
            loggerService.LogInfo(GetType(), $"Fetching country name from: {url}");

            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);
            return json["country"]?.ToString();
        }
        catch (Exception ex)
        {
            loggerService.LogError(GetType(), $"Country name fetch failed: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetPublicIpAddressAsync()
    {
        try
        {
            var client = new HttpClient();
            const string url = "https://api.ipify.org";
            loggerService.LogInfo(GetType(), $"Fetching IP address from: {url}");

            var response = await client.GetStringAsync(url);
            return response;
        }
        catch (Exception ex)
        {
            loggerService.LogError(GetType(), $"IP address fetch failed: {ex.Message}");
            return null;
        }
    }

    public bool ConnectToStrand7Api(string? location, bool freezeOnFail = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(location);

        var directorySwitched = false;
        var isApiConnected = false;

        try
        {
            directorySwitched = SetToStrand7ApiDirectory(location);

            ReleaseStrand7Api();

            St7.St7Init().ThrowIfFails();
            isApiConnected = true;
        }
        catch (Exception ex)
        {
            loggerService.LogError(GetType(), $"Failed to initialize Strand7 api from {location}: {ex.Message}");
        }

        if (directorySwitched)
            SetBackToAppDirectory();

        if (!isApiConnected && freezeOnFail)
        {
            splashService.SetMessageInSplash($"Error! Strand7 API could not be initialized from {location}. " +
                                             $"\n\nTry selecting another location. If you are certain that Strand7 is able to launch from that location and an application restart did not work, then try restarting your machine.");
            splashService.SetStrand7LocationErrorInSplash(location);

            Task.Run(async () => await Task.Delay(300000)).GetAwaiter().GetResult();
            RestartApp();
        }
        else
        {
            femFilePathService.InsertStrandApiPath(location);
            isApiConnected = true;
        }

        IsApiConnected = isApiConnected;
        return IsApiConnected;
    }

    private static bool SetToStrand7ApiDirectory(string location)
    {
        var directoryExists = Directory.Exists(location);
        if (!directoryExists)
            return false;

        if (string.IsNullOrWhiteSpace(location))
            return false;

        try { Directory.SetCurrentDirectory(location); } catch (Exception) { return false; }
        return true;
    }

    private static void SetBackToAppDirectory()
    {
        var startupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
        if (string.IsNullOrWhiteSpace(startupPath))
            return;

        Directory.SetCurrentDirectory(startupPath);
    }

    public string GetScratchLocation()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Strand7 R31\\Tmp";
    }

    public void ReleaseStrand7Api()
    {
        St7.St7Release().HandleApiError();
    }
}