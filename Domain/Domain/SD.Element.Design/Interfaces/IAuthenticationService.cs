using SD.Core.Shared.Models;

namespace SD.Element.Design.Interfaces;
public interface IAuthenticationService
{
    public Task<bool> IsUserValid();
    public Result CertifyApplication();
    public Result AuthoriseUser();
}
