using System.Collections.ObjectModel;
using System.Windows;
using PasswordManager.UI.Models;

namespace PasswordManager.UI.Views
{
    public partial class DeleteConfirmationWindow : Window
    {
        private readonly ObservableCollection<Account> _accounts;
        private readonly Account _accountToDelete;

        public DeleteConfirmationWindow(ObservableCollection<Account> accounts, Account account)
        {
            InitializeComponent();
            _accounts = accounts;
            _accountToDelete = account;
        }

        private void DeleteConfirmationButton_Click(object sender, RoutedEventArgs e)
        {
            _accounts.Remove(_accountToDelete);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
