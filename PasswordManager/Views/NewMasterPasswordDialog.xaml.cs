using System;
using System.Collections.Generic;
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

namespace PasswordManager.Views
{
    /// <summary>
    /// Interaction logic for NewMasterPasswordDialog.xaml
    /// </summary>
    public partial class NewMasterPasswordDialog : Window
    {
        public NewMasterPasswordDialog()
        {
            InitializeComponent();
        }

        public string NewPassword => MasterPasswordBox.Password;

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MasterPasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmMasterPasswordBox.Password))
            {
                MessageBox.Show("Enter and confirm new password.");
                return;
            }

            if (MasterPasswordBox.Password != ConfirmMasterPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
