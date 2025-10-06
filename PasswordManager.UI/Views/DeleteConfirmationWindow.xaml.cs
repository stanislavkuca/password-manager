using PasswordManager.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for DeleteConfirmationWindow.xaml
    /// </summary>
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

        private void StornoConfirmationButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
