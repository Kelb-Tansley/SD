using SD.Core.Shared.Models;

namespace SD.Element.Design.Interfaces;
public interface IAuthenticationService
{
    Task<string> SignInAndGetTokenAsync();
    public Task<bool> IsUserValid();
}
