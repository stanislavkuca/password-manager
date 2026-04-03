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

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (EnsureMasterPasswordExists())
            {
                RunAuthFlow();
            }
        }

        private bool EnsureMasterPasswordExists()
        {
            string masterPath = Path.Combine("Data", "master.hash");

            if (!File.Exists(masterPath))
            {
                var setup = new NewMasterPasswordDialog();

                if (setup.ShowDialog() == true)
                {
                    Directory.CreateDirectory("Data");
                    File.WriteAllText(masterPath, AuthService.Hash(setup.NewPassword));
                    return true;
                }
                Shutdown();
                return false;
            }

            return true;
        }

        public void RunAuthFlow()
        {
            var login = new LoginDialog();

            if (login.ShowDialog() == true)
            {
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                var main = new MainWindow();
                main.Show();
            }
            else
            {
                Shutdown();
            }
        }

        public bool ConfirmIdentity()
        {
            var login = new LoginDialog
            {
                Owner = Dispatcher.CheckAccess() ? MainWindow : null,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            return login.ShowDialog() == true;
        }
    }
}
