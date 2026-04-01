namespace SD.Services;

public interface IWebApiHttpClient
{
    Task<string> GetUserLicense(string bearer);
}