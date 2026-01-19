using SD.Constants;
using Microsoft.Identity.Client;
using Microsoft.Win32;
using SD.Element.Design.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Principal;
using Prism.Events;
using System.DirectoryServices.AccountManagement;
using SD.Core.Shared.Models;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models.Core;
using SD.Core.Shared.Events;

namespace SD.Services;
public class AuthenticationService : IAuthenticationService
{
    private readonly ApiSettings _apiSettings;
    private readonly ITokenCacheService _tokenCacheService;
    private readonly ISplashService _splashService;
    private readonly IEventAggregator _eventAggregator;
    private readonly IPublicClientApplication _signInClient;

    public AuthenticationService(ApiSettings apiSettings, ITokenCacheService tokenCacheService, ISplashService splashService, IEventAggregator eventAggregator)
    {
        _apiSettings = apiSettings;
        _tokenCacheService = tokenCacheService;
        _splashService = splashService;
        _eventAggregator = eventAggregator;
        _signInClient = PublicClientApplicationBuilder.Create(_apiSettings.AppRegClientId)
                            .WithAuthority(AzureCloudInstance.AzurePublic, _apiSettings.AppRegTenantId)
            .WithClientId(_apiSettings.AppRegClientId)
            //.WithRedirectUri(@"https://localhost:5003/.auth/login/aad/callback")
            .WithRedirectUri("http://localhost")
                            //.WithDefaultRedirectUri()
                            .Build();

        _tokenCacheService.EnableSerialization(_signInClient.UserTokenCache);
    }

    private async Task SignOutInvalidAccount()
    {
        var accounts = await _signInClient.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                await _signInClient.RemoveAsync(accounts.FirstOrDefault());
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
        var firstAccount = (await _signInClient.GetAccountsAsync())?.FirstOrDefault();

        try
        {
            return (await _signInClient.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync()).AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                var result = await _signInClient.AcquireTokenInteractive(scopes)
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
            return true; // TODO: Awaiting approval by Andrew Burt before we can implement this auth method
            var userSignInToken = await GetSignedInUserAccessToken();

            if (_tokenCacheService.IsTokenEmptyOrInvalid(userSignInToken))
            {
                await SignOutInvalidAccount();
                return await IsUserValid();
            }
            else
                return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

