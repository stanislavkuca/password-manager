using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.UI.Models
{
    public class Folder
    {
        public string Name { get; set; }
        public ObservableCollection<Guid> AccountIds { get; } = new();

        public Folder(string name)
        {
            Name = name;
        }
    }
}
