using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SD.Services;

public class InstallKeyFileService
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Aurestruct.raw.v1");

    public string InstallKeyFilePath { get; }

    public InstallKeyFileService()
    {
        InstallKeyFilePath = Path.Combine(AppContext.BaseDirectory, ".aurestruct.raw");
    }

    public bool TryReadInstallKey(out string? installKey)
    {
        installKey = null;

        try
        {
            if (!File.Exists(InstallKeyFilePath))
                return false;

            var encrypted = File.ReadAllBytes(InstallKeyFilePath);
            if (encrypted.Length == 0)
                return false;

            var decrypted = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.LocalMachine);
            var value = Encoding.UTF8.GetString(decrypted).Trim();
            if (string.IsNullOrWhiteSpace(value))
                return false;

            installKey = value;
            return true;
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