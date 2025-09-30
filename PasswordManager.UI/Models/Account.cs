using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.UI.Models
{
    public class Account(string name, string username, string password, string? note = null)
    {
        public string AccountName { get; } = name;
        public string AccountUsername { get; } = username;
        public string AccountPassword { get; } = password;
        public string? AccountNote { get; } = note;
    }
}
