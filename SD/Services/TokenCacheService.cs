using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Identity.Client;
using SD.Core.Shared.Contracts;
using SD.Element.Design.Interfaces;

namespace Hatch.Pdg.SD.Services;
public class TokenCacheService : ITokenCacheService
{
    public TokenCacheService(IAppSettings appSettings)
    {
        try
        {
            var authLocation = Environment.ExpandEnvironmentVariables(appSettings.AuthLocation);
            Directory.CreateDirectory(authLocation);
            CacheFilePath = Path.Combine(authLocation, ".msalcache.bin3");
        }
        catch (InvalidOperationException)
        {
            CacheFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin3";
        }
    }
    public string CacheFilePath { get; private set; }
    private readonly object FileLock = new object();
    public void BeforeAccessNotification(TokenCacheNotificationArgs args)
    {
        lock (FileLock)
        {
            args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                    ? ProtectedData.Unprotect(File.ReadAllBytes(CacheFilePath),
                                             null,
                                             DataProtectionScope.CurrentUser)
                    : null);
        }
    }

    public void AfterAccessNotification(TokenCacheNotificationArgs args)
    {
        if (args.HasStateChanged)
        {
            lock (FileLock)
            {
                File.WriteAllBytes(CacheFilePath,
                                   ProtectedData.Protect(args.TokenCache.SerializeMsalV3(),
                                                         null,
                DataProtectionScope.CurrentUser)
                                  );
            }
        }
    }
    public void EnableSerialization(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }
    public bool IsTokenEmptyOrInvalid(string token)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        var jwtToken = new JwtSecurityToken(token);
        return jwtToken == null || jwtToken.ValidFrom > DateTime.UtcNow && jwtToken.ValidTo < DateTime.UtcNow;
    }
}