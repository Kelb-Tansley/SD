using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SD.Services;

public sealed class IntegritySettings
{
    public bool Enforce { get; set; }
    public bool RequireSignedArtifacts { get; set; } = true;
    public bool RequireTrustedCertificate { get; set; } = true;
    public string? ExpectedPublisherThumbprint { get; set; }
    public List<IntegrityFileRule> Files { get; set; } = [];
}

public sealed class IntegrityFileRule
{
    public string Path { get; set; } = string.Empty;
    public string? Sha256 { get; set; }
}

public static class ArtifactIntegrityService
{
    public static void ValidateOrThrow(IntegritySettings? settings, string baseDirectory)
    {
        if (settings is null || !settings.Enforce)
            return;

        var normalizedThumbprint = Normalize(settings.ExpectedPublisherThumbprint);

        foreach (var rule in settings.Files)
        {
            if (string.IsNullOrWhiteSpace(rule.Path))
                continue;

            var fullPath = Path.IsPathRooted(rule.Path)
                ? rule.Path
                : Path.Combine(baseDirectory, rule.Path);

            if (!File.Exists(fullPath))
                throw new InvalidOperationException($"Integrity check failed. Missing file: {fullPath}");

            if (settings.RequireSignedArtifacts)
                VerifyAuthenticode(fullPath, normalizedThumbprint, settings.RequireTrustedCertificate);

            VerifyHash(fullPath, rule.Sha256);
        }
    }

    private static void VerifyAuthenticode(string filePath, string? expectedThumbprint, bool requireTrustedCertificate)
    {
        using var certificate = new X509Certificate2(X509Certificate.CreateFromSignedFile(filePath));

        if (!string.IsNullOrWhiteSpace(expectedThumbprint))
        {
            var actualThumbprint = Normalize(certificate.Thumbprint);
            if (!string.Equals(expectedThumbprint, actualThumbprint, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Integrity check failed. Unexpected certificate for file: {filePath}");
        }

        if (!requireTrustedCertificate)
            return;

        using var chain = new X509Chain
        {
            ChainPolicy =
            {
                RevocationMode = X509RevocationMode.Online,
                RevocationFlag = X509RevocationFlag.ExcludeRoot,
                VerificationFlags = X509VerificationFlags.NoFlag
            }
        };

        if (!chain.Build(certificate))
            throw new InvalidOperationException($"Integrity check failed. Certificate chain validation failed for file: {filePath}");
    }

    private static void VerifyHash(string filePath, string? expectedSha256)
    {
        var normalizedExpectedHash = Normalize(expectedSha256);
        if (string.IsNullOrWhiteSpace(normalizedExpectedHash))
            return;

        using var stream = File.OpenRead(filePath);
        using var sha256 = SHA256.Create();
        var actualHash = Convert.ToHexString(sha256.ComputeHash(stream));

        if (!string.Equals(normalizedExpectedHash, Normalize(actualHash), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Integrity check failed. Hash mismatch for file: {filePath}");
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Replace(" ", string.Empty, StringComparison.Ordinal).Trim().ToUpperInvariant();
}
