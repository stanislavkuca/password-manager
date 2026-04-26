using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;

namespace PasswordManager.Models
{
    public class Account : INotifyPropertyChanged
    {
        // Stable identifier so accounts can be referenced and persisted reliably.
        public Guid AccountId { get; } = Guid.NewGuid();
        private Guid? _folderId;
        public Guid? FolderId
        {
            get => _folderId;
            set
            {
                if (_folderId != value)
                {
                    _folderId = value;
                    OnPropertyChanged(nameof(FolderId));
                }
            }
        }

        public string Name { get; set; }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                    OnPropertyChanged(nameof(MaskedUsername));
                }
            }
        }

        public string Password { get; set; }
        public string? Note { get; set; }

        private bool _isFavourite;
        public bool IsFavourite
        {
            get => _isFavourite;
            set
            {
                if (_isFavourite != value)
                {
                    _isFavourite = value;
                    OnPropertyChanged(nameof(IsFavourite));
                }
            }
        }

        public string MaskedUsername
        {
            get
            {
                if (string.IsNullOrEmpty(Username))
                    return string.Empty;

                if (Username.Length <= 4)
                    return new string('*', Username.Length);

                return "****" + Username.Substring(4);
            }
        }

        public Account(string name, string username, string password, string? note = null)
        {
            Name = name;
            Username = username;
            Password = password;
            Note = note;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string prop)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // Use DPAPI (CurrentUser) so encrypted passwords are tied to the current Windows user profile.
        public static string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            var bytes = Encoding.UTF8.GetBytes(data);
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        // Decrypt and return plain text
        public static string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData)) return string.Empty;
            try
            {
                var bytes = Convert.FromBase64String(encryptedData);
                var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (FormatException)
            {
                MessageBox.Show("Stored data is corrupted or not encrypted correctly.");
                return string.Empty;
            }
        }
    }
}
