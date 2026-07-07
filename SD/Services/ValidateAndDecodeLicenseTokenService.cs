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
            ValidIssuer = "de1ea3ae-85f5-4876-895d-58d126a372ab",       // must match issuer used when creating
            ValidateAudience = true,
            ValidAudience = "aurestruct-web-nonprod",  // must match audience used when creating
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