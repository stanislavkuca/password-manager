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
using System.Windows.Shapes;
using System.IO;

namespace PasswordManager.Views
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
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
            string path = "Data/master.hash";

            if (!File.Exists(path))
            {
                string hash = AuthService.Hash(entered);
                File.WriteAllText(path, hash);
                DialogResult = true;
                return;
            }

            string storedHash = File.ReadAllText(path);

            if (AuthService.Verify(entered, storedHash))
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Wrong password");
            }
        }
    }
}
