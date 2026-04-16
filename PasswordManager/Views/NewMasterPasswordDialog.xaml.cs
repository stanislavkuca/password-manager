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
    /// Dialog used to create a new master password.
    /// Keep UI logic to minimal here; validation and persistence are intentionally simple.
    /// </summary>
    public partial class NewMasterPasswordDialog : Window
    {
        public NewMasterPasswordDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Entered master password. Avoid logging.
        /// </summary>
        public string NewPassword => MasterPasswordBox.Password;

        // Prevent accidental empty master password; more complex validation lives in a shared service.
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
    }
}
