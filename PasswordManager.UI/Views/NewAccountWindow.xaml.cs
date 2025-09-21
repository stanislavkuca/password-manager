using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordManager.UI.Views
{
    /// <summary>
    /// Interaction logic for NewAccountWindow.xaml
    /// </summary>
    public partial class NewAccountWindow : Window
    {
        public class Account(string name, string username, string password, string? note = null)
        {
            public string AccountName { get; } = name;
            public string AccountUsername { get; } = username;
            public string AccountPassword { get; } = password;
            public string? AccountNote { get; } = note;
        }
        
        public Account? CreatedAccount { get; private set; }

        public NewAccountWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreatedAccount = new Account(
                AccountNameTextBox.Text,
                AccountUsernameTextBox.Text,
                AccountPasswordTextBox.Text,
                AccountNoteTextBox.Text
            );

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
