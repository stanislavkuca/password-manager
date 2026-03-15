using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class AppData
    {
        public List<Account> Accounts { get; set; } = new();
        public List<Folder> Folders { get; set; } = new();
    }
}
