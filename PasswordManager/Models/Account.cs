using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class Account : INotifyPropertyChanged
    {
        public Guid Id { get; } = Guid.NewGuid();

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
    }
}
