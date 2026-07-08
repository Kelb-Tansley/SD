using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SD.Services;

public static class ValidateAndDecodeLicenseTokenService
{
    private static readonly string[] InstallKeyClaimTypes =
    [
        "InstallKey",
        "installKey",
        "install_key"
    ];

    public static LicenseValidationResult ValidateAndDecodeLicenseTokenDetails(this string token, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "6a75416f-b00c-46a6-aa83-ac75cf5b6b93",       // must match issuer used when creating
            ValidateAudience = true,
            ValidAudience = "718a047d-f387-4017-8642-e9eba84c68df",  // must match audience used when creating
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero       // no tolerance for expired tokens
        };

        try
        {
            // Validate signature, issuer, audience, and expiration
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Extract claims
            var licensedClaim = principal.Claims.FirstOrDefault(c => c.Type == "Licensed")?.Value ?? throw new SecurityTokenException("Missing licensed claim");
            _ = principal.Claims.FirstOrDefault(c => c.Type == "ExpiresAt")?.Value ?? throw new SecurityTokenException("Missing expiration claim");
            var installKey = InstallKeyClaimTypes
                .Select(claimType => principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value)
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

            bool.TryParse(licensedClaim, out var licensed);
            return new LicenseValidationResult
            {
                IsLicensed = licensed,
                InstallKey = installKey
            };
        }
        catch (SecurityTokenException)
        {
            throw;
        }
    }
}