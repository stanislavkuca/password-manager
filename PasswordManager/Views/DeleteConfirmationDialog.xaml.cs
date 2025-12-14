using System.Collections.ObjectModel;
using System.Windows;
using PasswordManager.Models;

namespace PasswordManager.Views
{
    public partial class DeleteConfirmationWindow : Window
    {
        public Account AccountToDelete { get; }

        public DeleteConfirmationWindow(Account account)
        {
            InitializeComponent();
            AccountToDelete = account;
        }

        private void DeleteConfirmationButton_Click(object sender, RoutedEventArgs e)
        {
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
