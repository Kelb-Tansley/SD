using Hatch.Pdg.SD.Constants;
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

namespace Hatch.Pdg.SD.Services;
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

    /// <summary>
    /// Ensure that the application is located on a Hatch machine
    /// </summary>
    /// <returns></returns>
    public Result CertifyApplication()
    {
        _splashService.SetMessageInSplash("Authorising...");
        const string certStoreName = "My";
        const string issuerName = "CN=Hatch Global Issuing CA, OU=Certification Services, O=Hatch Ltd., C=CA";
        const string issuerName2 = "CN=Hatch Global Issuing CA 2, OU=Certification Services, O=Hatch Ltd., C=CA";

        var validCertificate = false;
        foreach (var storeLocation in (StoreLocation[])Enum.GetValues(typeof(StoreLocation)))
        {
            if (!storeLocation.Equals(StoreLocation.CurrentUser))
                continue;
            foreach (var storeName in (StoreName[])Enum.GetValues(typeof(StoreName)))
            {
                var store = new X509Store(storeName, storeLocation);
                if (store.Name != certStoreName)
                    continue;
                try
                {
                    store.Open(OpenFlags.OpenExistingOnly);
                    foreach (var i in store.Certificates)
                    {
                        if (i.IssuerName.Name == issuerName || i.IssuerName.Name == issuerName2)
                        {
                            validCertificate = true;
                            break;
                        }
                    }
                    break;
                }
                catch (CryptographicException ex)
                {
                    return Result.Fail(ex.Message);
                }
            }
            if (validCertificate)
                break;
        }
        if (!validCertificate)
            return Result.Fail("Invalid application certificate.");

        return Result.Ok();
    }

    /// <summary>
    /// Ensure that the current app user is authorised to use this app. This method is temporary and will be replaced with AAD (Azure Active Directory)
    /// </summary>
    /// <returns></returns>
    public Result AuthoriseUser()
    {
        var registry = @"SOFTWARE\StructuralTools";
        var currentUser = WindowsIdentity.GetCurrent()?.Name;
        if (string.IsNullOrWhiteSpace(currentUser))
            return Result.Fail("Invalid user.");

        //Prepare registry for email storage 
        var retrievedSubkey = Registry.CurrentUser.CreateSubKey(registry, true);
        if (retrievedSubkey == null)
            return Result.Fail("Invalid user.");

        retrievedSubkey = Registry.CurrentUser.OpenSubKey(registry, true);
        if (retrievedSubkey == null)
            return Result.Fail("Invalid user.");

        var emailAdress = retrievedSubkey.GetValue(currentUser)?.ToString();
        if (string.IsNullOrWhiteSpace(emailAdress))
        {
            //Get email and network connectivity status
            var emailConnectionResult = NetworkEmailConnection();
            if (emailConnectionResult.IsFailure || !emailConnectionResult.Value.OnHatchNetwork)
                return Result.Fail($"A Hatch network connection is required to run this application. {emailConnectionResult.Message}");

            var userEmailAddress = emailConnectionResult.Value?.EmailAddress?.ToUpperTrimmed();
            if (string.IsNullOrWhiteSpace(userEmailAddress))
                return Result.Fail("Current user credentials cannot be found. Contact Admin.");

            //Add and retrieve the email address value 
            retrievedSubkey.SetValue(currentUser, userEmailAddress, RegistryValueKind.String);
            emailAdress = retrievedSubkey.GetValue(currentUser)?.ToString();

            if (string.IsNullOrWhiteSpace(emailAdress))
                return Result.Fail("Current user credentials cannot be found. Contact Admin.");
        }
        else
        {
            //Check that email has not been updated
            var emailConnectionResult = NetworkEmailConnection();
            if (emailConnectionResult.IsSuccess && emailConnectionResult.Value.OnHatchNetwork)
            {
                var userEmailAddress = emailConnectionResult.Value?.EmailAddress?.ToUpperTrimmed();
                if (!string.IsNullOrWhiteSpace(userEmailAddress) && !userEmailAddress.Equals(emailAdress, StringComparison.OrdinalIgnoreCase))
                {
                    //Update and retrieve the email address value 
                    retrievedSubkey.SetValue(currentUser, userEmailAddress, RegistryValueKind.String);
                    emailAdress = retrievedSubkey.GetValue(currentUser)?.ToString();
                }
            }
        }
        if (string.IsNullOrWhiteSpace(emailAdress))
            return Result.Fail($"No email address found for {currentUser}.");

        var thisUser = OurUsers.AuthorisedUsers().FirstOrDefault(user => emailAdress.Equals(user, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(thisUser))
            return Result.Fail("You have not been authorised to use this application. Contact Admin.");

        return Result.Ok();
    }
    /// <summary>
    /// Check for network connection using Active Directory
    /// </summary>
    /// <returns></returns>
    private Result<EmailConnection> NetworkEmailConnection()
    {
        var emailConnection = new EmailConnection();

        try
        {
            var ctx = new PrincipalContext(ContextType.Domain);
            if (ctx == null || string.IsNullOrWhiteSpace(ctx.ConnectedServer))
                return Result.Fail<EmailConnection>("No server connection found.");

            var isKnownServer = ctx.ConnectedServer.ToLower().Contains("hatchglobal") || ctx.ConnectedServer.ToLower().Contains("infosys");
            if (Convert.ToBoolean(isKnownServer))
            {
                var user = UserPrincipal.FindByIdentity(ctx, Environment.UserName);
                if (user != null)
                {
                    emailConnection.OnHatchNetwork = true;
                    emailConnection.EmailAddress = user.EmailAddress;
                    _splashService.SetMessageInSplash($"Account: {emailConnection.EmailAddress} located...");
                    user.Dispose();
                }
                else
                    return Result.Fail<EmailConnection>($"Could not find identity details for {Environment.UserName}.");
            }
            else
                return Result.Fail<EmailConnection>("The application is located on an unknown server.");
        }
        catch (Exception ex)
        {
            return Result.Fail<EmailConnection>(ex.Message);
        }

        return Result.Ok(emailConnection);
    }
    public class EmailConnection
    {
        public string EmailAddress { get; set; }
        public bool OnHatchNetwork { get; set; } = false;
    }
}

