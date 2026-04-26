using PasswordManager.Services;
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
using System.IO;

namespace PasswordManager.Views
{
    /// <summary>
    /// Login dialog. Keeps auth logic minimal and delegates hashing/verification to AuthService.
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string entered = MasterPasswordBox.Password;
            string path = Path.Combine("Data", "master.hash");

            if (File.Exists(path) && AuthService.Verify(entered, File.ReadAllText(path)))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Wrong password.");
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Resetting the master password will delete all stored data. Continue?",
                "Warning",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                if (File.Exists("Data/vault.dat"))
                    File.Delete("Data/vault.dat");

                if (File.Exists("Data/master.hash"))
                    File.Delete("Data/master.hash");
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Error deleting data: " + ex.Message);
                return;
            }

            // After successful deletion prompt for a new master passsword.
            var newPasswordDialog = new NewMasterPasswordDialog();
            bool? dialogResult = newPasswordDialog.ShowDialog();

            if (dialogResult == true)
            {
                string newPassword = newPasswordDialog.NewPassword;
                string hash = AuthService.Hash(newPassword);

                Directory.CreateDirectory("Data");
                File.WriteAllText(Path.Combine("Data", "master.hash"), hash);

                MessageBox.Show("New master password created.");
            }
            else
            {
                MessageBox.Show("Master password not created. Application cannot continue.");
                Application.Current.Shutdown();
            }
        }
    }
}
