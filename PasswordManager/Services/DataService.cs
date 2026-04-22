using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Services
{
    /// <summary>
    /// Persisted data handling with DPAPI encryption.
    /// </summary>
    public static class DataService
    {
        private static string path = "Data/vault.dat";

        public static void Save(AppData data)
        {
            try
            {
                Directory.CreateDirectory("Data");

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(json);
                byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);

                File.WriteAllBytes(path, encryptedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Saving error: {ex.Message}");
            }
        }

        public static AppData Load()
        {
            if (!File.Exists(path))
                return new AppData();

            try
            {
                byte[] encryptedData = File.ReadAllBytes(path);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);

                string json = Encoding.UTF8.GetString(decryptedData);

                return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Decryption error.");
                return new AppData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Loading error: {ex.Message}");
                return new AppData();
            }
        }
    }
    
}
