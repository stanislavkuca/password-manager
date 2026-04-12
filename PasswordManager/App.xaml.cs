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

            bool isDark = PasswordManager.Properties.Settings.Default.IsDarkMode;
            ChangeTheme(isDark);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (EnsureMasterPasswordExists())
            {
                RunAuthFlow();
            }
        }

        public void ChangeTheme(bool isDark)
        {
            string themeName = isDark ? "DarkTheme.xaml" : "WhiteTheme.xaml";
            var uri = new Uri($"Styles/Themes/{themeName}", UriKind.Relative);

            ResourceDictionary newTheme = new ResourceDictionary { Source = uri };

            Current.Resources.MergedDictionaries[0] = newTheme;

            PasswordManager.Properties.Settings.Default.IsDarkMode = isDark;
            PasswordManager.Properties.Settings.Default.Save();
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
                var main = new MainWindow();

                this.MainWindow = main;

                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
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
