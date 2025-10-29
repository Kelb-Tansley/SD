namespace SD.Element.Design.Interfaces;
public interface IConnectionService
{
    public bool IsApiConnected { get; }
    public string GetScratchLocation();
    public bool ConnectToStrand7Api();
    void ReleaseStrand7Api();
    public string? GetUserCurrentCountry();
    public Task<string?> GetPublicIpAddressAsync();
    public Task<string?> GetCountryByIpAsync(string? ipAddress);
}