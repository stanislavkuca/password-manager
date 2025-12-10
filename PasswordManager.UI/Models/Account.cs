using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.UI.Models
{
    public class Account
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Note { get; set; }

        public Account(string name, string username, string password, string? note = null)
        {
            Name = name;
            Username = username;
            Password = password;
            Note = note;
        }
    }
}
