namespace SD.Element.Design.Interfaces;

public interface IAuthenticationService
{
    public Task<bool> IsUserValid();
}