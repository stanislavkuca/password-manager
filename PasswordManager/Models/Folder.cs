using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class Folder : INotifyPropertyChanged
    {
        // Stable identifier so folders can be referenced and persisted reliably.
        public Guid Id { get; } = Guid.NewGuid();

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged();  }
        }

        public Folder(string name) => _name = name;


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
