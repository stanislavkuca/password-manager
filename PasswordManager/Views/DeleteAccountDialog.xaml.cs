using System.Collections.ObjectModel;
using System.Windows;
using PasswordManager.Models;

namespace PasswordManager.Views
{
    public partial class DeleteAccountDialog : Window
    {
        public Account AccountToDelete { get; }

        public DeleteAccountDialog(Account account)
        {
            InitializeComponent();
            AccountToDelete = account;
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
