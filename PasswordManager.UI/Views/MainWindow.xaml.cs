using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PasswordManager.UI.Helpers;
using PasswordManager.UI.Models;

namespace PasswordManager.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Account> Accounts { get; set; } = [];

        public MainWindow()
        {
            InitializeComponent();

            AccountList.ItemsSource = Accounts;

            Accounts.Add(new Account("Google", "user", "password", "stary ucet"));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowHelper.OpenDialog(this, out NewAccountWindow newAccountWindow) == true)
            {
                var account = newAccountWindow.CreatedAccount;
                if (account != null)
                {
                    Accounts.Add(account);

                }
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