using SD.Core.Shared.Models.Core;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SD.Services;

public class WebApiHttpClient : IWebApiHttpClient
{
    private readonly HttpClient _httpClient;

    public WebApiHttpClient(HttpClient httpClient, ApiSettings apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri(apiSettings.BaseAddress ?? throw new ArgumentNullException(nameof(apiSettings.BaseAddress)));
    }

    public async Task<string> GetUserLicense(string bearer)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/License/checkLicenseValidity");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        var response = await _httpClient.SendAsync(request);

        return await response.Content.ReadAsStringAsync();
    }
}
