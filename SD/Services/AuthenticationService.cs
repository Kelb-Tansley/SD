using Microsoft.Identity.Client;
using Microsoft.Win32;
using Prism.Events;
using SD.Constants;
using SD.Core.Shared.Events;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.Core;
using SD.Element.Design.Interfaces;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace SD.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApiSettings _apiSettings;
    private readonly ITokenCacheService _tokenCacheService;
    private readonly ISplashService _splashService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IWebApiHttpClient _webApiHttpClient;
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;

    public AuthenticationService(ApiSettings apiSettings,
                                 ITokenCacheService tokenCacheService,
                                 ISplashService splashService,
                                 IEventAggregator eventAggregator,
                                 IWebApiHttpClient webApiHttpClient)
    {
        _apiSettings = apiSettings;
        _tokenCacheService = tokenCacheService;
        _splashService = splashService;
        _eventAggregator = eventAggregator;
        _webApiHttpClient = webApiHttpClient;
        //_pca = PublicClientApplicationBuilder.Create(_apiSettings.AppRegClientId)
        //                    .WithAuthority(AzureCloudInstance.AzurePublic, _apiSettings.AppRegTenantId)
        //    .WithClientId(_apiSettings.AppRegClientId)
        //    //.WithRedirectUri(@"https://localhost:5003/.auth/login/aad/callback")
        //    .WithRedirectUri("http://localhost")
        //                    //.WithDefaultRedirectUri()
        //                    .Build();

        _pca = PublicClientApplicationBuilder.Create(_apiSettings.AppRegClientId)
           .WithAuthority(AzureCloudInstance.AzurePublic, _apiSettings.AppRegTenantId)
           .WithRedirectUri("http://localhost")
           .Build();


        _tokenCacheService.EnableSerialization(_pca.UserTokenCache);
        //_scopes = new[] { $"api://{_apiSettings.ApiAppRegClientId}/.default" };
        _scopes = new[] { $"{_apiSettings.ApiAppRegClientId}/.default" };
    }
    public async Task<string> SignInAndGetTokenAsync()
    {
        try
        {
            var accounts = await _pca.GetAccountsAsync();
            var result = await _pca.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                                   .ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                var result = await _pca.AcquireTokenInteractive(_scopes)
                                       .WithPrompt(Prompt.SelectAccount)
                                       .ExecuteAsync();
                return result.AccessToken;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

    private async Task SignOutInvalidAccount()
    {
        var accounts = await _pca.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                await _pca.RemoveAsync(accounts.FirstOrDefault());
            }
            catch (MsalException)
            {

            }
        }
    }

    private async Task<string> GetSignedInUserAccessToken(bool isFirstTimeSignIn = false)
    {
        var scopes = new List<string>() { $"{_apiSettings.AppRegClientId}/Read" };
        //var scopes = new string[] { "user.read" };
        var firstAccount = (await _pca.GetAccountsAsync())?.FirstOrDefault();

        try
        {
            return (await _pca.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync()).AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                var result = await _pca.AcquireTokenInteractive(scopes)
                    .WithAccount(firstAccount)
                    //.WithParentActivityOrWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
                return result.AccessToken;
            }
            catch (MsalException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            throw;
        }
    }

    public async Task<bool> IsUserValid()
    {
        try
        {
            var accessToken = await SignInAndGetTokenAsync();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);

            var secret = jwt.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;

            var response = await _webApiHttpClient.GetUserLicense(accessToken);

            var valid = response.ValidateAndDecodeLicenseToken(secret);
            return valid;
            //return true; // TODO: Awaiting approval by Andrew Burt before we can implement this auth method
            //var userSignInToken = await GetSignedInUserAccessToken();

            //if (_tokenCacheService.IsTokenEmptyOrInvalid(userSignInToken))
            //{
            //    await SignOutInvalidAccount();
            //    return await IsUserValid();
            //}
            //else
            //    return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}