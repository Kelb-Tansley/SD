using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SD.Services;

public class InstallKeyFileService : IInstallKeyFileService
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Aurestruct.raw.v1");

    private readonly string[] _readCandidates;

    public string InstallKeyFilePath { get; }

    public InstallKeyFileService()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appDataAuthDir = Path.Combine(localAppData, "Aurestruct", "Auth");
        InstallKeyFilePath = Path.Combine(appDataAuthDir, ".aurestruct.raw");

        _readCandidates =
        [
            InstallKeyFilePath,
            Path.Combine(AppContext.BaseDirectory, ".aurestruct.raw"),
            Path.Combine(AppContext.BaseDirectory, ".aurestruct.installkey")
        ];
    }

    public bool TryReadInstallKey(out string? installKey)
    {
        installKey = null;

        try
        {
            foreach (var candidate in _readCandidates)
            {
                if (!File.Exists(candidate))
                    continue;

                var encrypted = File.ReadAllBytes(candidate);
                if (encrypted.Length == 0)
                    continue;

                var decrypted = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.LocalMachine);
                var value = Encoding.UTF8.GetString(decrypted).Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                installKey = value;
                return true;
            }

            return false;
        }
        catch (CryptographicException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public void SaveInstallKey(string installKey)
    {
        if (string.IsNullOrWhiteSpace(installKey))
            throw new ArgumentException("Install key must not be empty.", nameof(installKey));

        var directory = Path.GetDirectoryName(InstallKeyFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var plainBytes = Encoding.UTF8.GetBytes(installKey.Trim());
        var encrypted = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.LocalMachine);
        File.WriteAllBytes(InstallKeyFilePath, encrypted);
    }
}