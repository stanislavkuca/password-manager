using PasswordManager.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using PasswordManager.Services;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string masterPath = Path.Combine("Data", "master.hash");

            if (!File.Exists(masterPath))
            {
                var newPasswordDialog = new NewMasterPasswordDialog();

                if (newPasswordDialog.ShowDialog() != true)
                {
                    MessageBox.Show("Master password not created. Appliacation will exit.");
                    Shutdown();
                    return;
                }

                Directory.CreateDirectory("Data");
                File.WriteAllText(masterPath, AuthService.Hash(newPasswordDialog.NewPassword));
                MessageBox.Show("Master password created.");
            }

            var login = new LoginDialog();
            login.Show();
        }
    }

}
