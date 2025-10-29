using Microsoft.Identity.Client;

namespace SD.Element.Design.Interfaces;
public interface ITokenCacheService
{
    public void EnableSerialization(ITokenCache tokenCache);
    public bool IsTokenEmptyOrInvalid(string token);
}
