using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PasswordManager.UI.Models;

namespace PasswordManager.UI.Views
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Account> Accounts { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AccountListData();
            AccountList.ItemsSource = Accounts;
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewAccountWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (window.ShowDialog() == true && window.CreatedAccount != null)
            {
                Accounts.Add(window.CreatedAccount);
            }
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Account account)
            {
                var dialog = new DeleteConfirmationWindow(Accounts, account)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dialog.ShowDialog();
            }
        }
    }
}
