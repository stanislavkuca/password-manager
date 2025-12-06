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
        public ObservableCollection<Account> Accounts { get; set; } = new();

        public Folder() { }

        public Folder(string name)
        {
            Name = name;
        }
    }
}
