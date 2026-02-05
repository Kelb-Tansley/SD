using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SD.Services;

public static class ValidateAndDecodeLicenseTokenService
{
    public static bool ValidateAndDecodeLicenseToken(this string token, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

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
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Extract claims
            var licensedClaim = principal.Claims.FirstOrDefault(c => c.Type == "Licensed")?.Value;
            var messageClaim = principal.Claims.FirstOrDefault(c => c.Type == "Message")?.Value;
            var expiresClaim = principal.Claims.FirstOrDefault(c => c.Type == "ExpiresAt")?.Value;

            bool.TryParse(licensedClaim, out var licensed);
            return licensed;
        }
        catch (SecurityTokenException ex)
        {
            return false;
        }
    }
}