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

    public static bool ValidateAndDecodeLicenseToken(this string token, string secret)
    {
        var result = token.ValidateAndDecodeLicenseTokenDetails(secret);
        return result.IsLicensed;
    }

    public static LicenseValidationResult ValidateAndDecodeLicenseTokenDetails(this string token, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "11c7f14f-3903-4948-8fe0-a106b8b001e2",       // must match issuer used when creating
            ValidateAudience = true,
            ValidAudience = "32631303-9274-4bf0-a8bd-4d1877a7f331",  // must match audience used when creating
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