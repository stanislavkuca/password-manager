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
        public string Username { get; set; }
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

        public static string EncryptPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            try
            {
                var bytes = Convert.FromBase64String(encryptedPassword);
                var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (FormatException)
            {
                MessageBox.Show("Stored password is corrupted or not encrypted correctly.");
                return string.Empty;
            }
        }
    }
}
