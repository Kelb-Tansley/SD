using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SD.Element.Design.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace SD.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly string _apiAppRegClientId = "6a75416f-b00c-46a6-aa83-ac75cf5b6b93";

    private readonly string _appRegClientId = "32631303-9274-4bf0-a8bd-4d1877a7f331";
    private readonly string _appRegTenantId = "2cd924f5-c630-4fca-9460-d4032b489567";

    private readonly ITokenCacheService _tokenCacheService;
    private readonly IWebApiHttpClient _webApiHttpClient;
    private readonly IInstallKeyFileService _installKeyFileService;
    private readonly IPublicClientApplication _pca;

    public AuthenticationService(ITokenCacheService tokenCacheService,
                                 IWebApiHttpClient webApiHttpClient,
                                 IInstallKeyFileService installKeyFileService)
    {
        _tokenCacheService = tokenCacheService;
        _webApiHttpClient = webApiHttpClient;
        _installKeyFileService = installKeyFileService;

        _pca = PublicClientApplicationBuilder.Create(_appRegClientId)
           .WithAuthority(AzureCloudInstance.AzurePublic, _appRegTenantId)
           .WithRedirectUri("http://localhost")
           .Build();

        _tokenCacheService.EnableSerialization(_pca.UserTokenCache);
    }

    public async Task<bool> IsUserValid()
    {
        var accessToken = await SignInAndGetTokenAsync();

        var response = await _webApiHttpClient.GetUserLicense(accessToken);
        var responseValue = ExtractResponseValue(response);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(accessToken);
        var secret = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
            ?? throw new SecurityTokenException("Missing oid claim");

        var license = ParseLicenseResponse(responseValue, secret);
        if (!license.IsLicensed)
            return false;

        if (string.IsNullOrWhiteSpace(license.InstallKey))
            throw new SecurityTokenException("Missing install key in license response.");

        if (!_installKeyFileService.TryReadInstallKey(out var localInstallKey))
        {
            _installKeyFileService.SaveInstallKey(license.InstallKey);
            return true;
        }

        return KeysEqual(localInstallKey, license.InstallKey);
    }

    private static LicenseValidationResult ParseLicenseResponse(string responseValue, string secret)
    {
        try
        {
            return responseValue.ValidateAndDecodeLicenseTokenDetails(secret);
        }
        catch (SecurityTokenException)
        {
            return new LicenseValidationResult
            {
                IsLicensed = !string.IsNullOrWhiteSpace(responseValue),
                InstallKey = responseValue
            };
        }
    }

    private static string ExtractResponseValue(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        var trimmed = response.Trim();
        if (!trimmed.StartsWith("\"", StringComparison.Ordinal) || !trimmed.EndsWith("\"", StringComparison.Ordinal))
            return trimmed;

        try
        {
            var value = JsonSerializer.Deserialize<string>(trimmed);
            return value?.Trim() ?? string.Empty;
        }
        catch (JsonException)
        {
            return trimmed.Trim('"');
        }
    }

    private static bool KeysEqual(string? localInstallKey, string remoteInstallKey)
    {
        if (string.IsNullOrWhiteSpace(localInstallKey) || string.IsNullOrWhiteSpace(remoteInstallKey))
            return false;

        var left = System.Text.Encoding.UTF8.GetBytes(localInstallKey.Trim());
        var right = System.Text.Encoding.UTF8.GetBytes(remoteInstallKey.Trim());
        return CryptographicOperations.FixedTimeEquals(left, right);
    }

    private async Task<string> SignInAndGetTokenAsync()
    {
        var scopes = new[] { $"{_apiAppRegClientId}/.default" };
        try
        {
            var accounts = await _pca.GetAccountsAsync();

            var result = await _pca.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                var result = await _pca.AcquireTokenInteractive(scopes)
                    .WithLoginHint(Environment.UserName)
                    .WithTenantId(_appRegTenantId)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
                return result.AccessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}