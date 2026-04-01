using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SD.Element.Design.Interfaces;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace SD.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly string _appRegClientId = "32631303-9274-4bf0-a8bd-4d1877a7f331";
    private readonly string _appRegTenantId = "2cd924f5-c630-4fca-9460-d4032b489567";
    private readonly string _apiAppRegClientId = "11c7f14f-3903-4948-8fe0-a106b8b001e2";
    //private readonly string _apiAppRegClientId = "d26d523e-f16a-42ab-a50f-fb869bc2b2f5";

    private readonly ITokenCacheService _tokenCacheService;
    private readonly IWebApiHttpClient _webApiHttpClient;
    private readonly IPublicClientApplication _pca;

    public AuthenticationService(ITokenCacheService tokenCacheService,
                                 IWebApiHttpClient webApiHttpClient)
    {
        _tokenCacheService = tokenCacheService;
        _webApiHttpClient = webApiHttpClient;

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

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(accessToken);
        var secret = jwt.Claims.FirstOrDefault(c => c.Type == "oid")?.Value
            ?? throw new SecurityTokenException("Missing oid claim");

        return response.ValidateAndDecodeLicenseToken(secret);
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