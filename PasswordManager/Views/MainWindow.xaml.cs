using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xaml.Permissions;
using PasswordManager.Models;
using PasswordManager.Services;
using System.Threading;
using System.Windows.Threading;
using PasswordManager.ViewModels;
using System.Windows.Input;
using System.Windows.Media;

namespace PasswordManager.Views
{
    /// <summary>
    /// Main application window. Contains UI logic only; logic is in MainViewModel.
    /// Keep UI code focused on view concerns (dialogs, timers, simple event handling).
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// ViewModel for the main window.
        /// </summary>
        public MainViewModel ViewModel { get; }
        private DispatcherTimer _lockTimer = new();

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            // Detect whether a "Dark" resource dictionary is loaded and set the toggle accordingly.
            var currentTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Dark"));

            // Set theme toggle after window is loaded to ensure controls are available.
            this.Loaded += (s, e) =>
            {
                ThemeToggleButton.IsChecked = (currentTheme != null);
            };

            // Load persisted data and start the inactivity timer.
            ViewModel.LoadData();
            StartTimer();
        }

        private int _secondsLeft;
        private const int DefaultTimeoutSeconds = 300;

        /// <summary>
        /// Initialize and start the UI timer used for automatic logout on inactivity.
        /// </summary>
        private void StartTimer()
        {
            _secondsLeft = DefaultTimeoutSeconds;

            _lockTimer.Interval = TimeSpan.FromSeconds(1);
            _lockTimer.Tick += Timer_Tick!;
            _lockTimer.Start();
        }

        /// <summary>
        /// Tick handler updates UI countdown and triggers logout when time expires.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            _secondsLeft--;

            TimeSpan time = TimeSpan.FromSeconds(_secondsLeft);
            TimerDisplay.Text = $"{time.ToString(@"mm\:ss")}";

            if (_secondsLeft < 60)
            {
                TimerDisplay.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TimerDisplay.ClearValue(TextBlock.ForegroundProperty);
            }

            if (_secondsLeft <= 0)
            {
                _lockTimer.Stop();
                Logout();
            }
        }

        /// <summary>
        /// Reset inactivity timer on user activity (mouse/keyboard)
        /// </summary>
        private void Window_UserActivity(object sender, RoutedEventArgs e)
        {
            _secondsLeft = DefaultTimeoutSeconds;
        }

        /// <summary>
        /// Perform a safe logout: stop timers, save data, and start auth flow.
        /// </summary>
        private void Logout()
        {
            _lockTimer.Stop();
            _lockTimer.Tick -= Timer_Tick!;

            ViewModel.SaveData();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            MessageBox.Show("Session expired after 5 minutes for security reasons.", "Please sign in again");

            this.Close();

            ((App)Application.Current).RunAuthFlow();
        }

        // --- Dialog Events ---

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewAccountWindow 
            { 
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (dialog.ShowDialog() == true && dialog.CreatedAccount != null)
            {
                ViewModel.AllAccounts.Add(dialog.CreatedAccount);
                ViewModel.SaveData();
                ViewModel.RefreshList();
            }
        }

        private void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewFolderDialog 
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.CreatedFolder?.Name))
            {
                var newFolder = new Folder(dialog.CreatedFolder.Name);

                ViewModel.Folders.Add(newFolder);
                ViewModel.SaveData();

                MessageBox.Show($"Folder '{newFolder.Name}' has been created.");
            }
        }

        private void FavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Account acc })
            {
                acc.IsFavourite = !acc.IsFavourite;

                ViewModel.RefreshList();
                ViewModel.SaveData();
            }
        }

        private void EditAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Account acc })
            {
                if (((App)Application.Current).ConfirmIdentity())
                {
                    var dialog = new NewAccountWindow(acc)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        ViewModel.SaveData();
                        ViewModel.RefreshList();
                    }
                }
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Account acc })
            {
                if (new DeleteAccountDialog(acc) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog() == true)
                {
                    ViewModel.AllAccounts.Remove(acc);

                    ViewModel.SaveData();
                    ViewModel.RefreshList();
                }
            }
        }

        /// <summary>
        /// Show all accounts by clearing selected folder filter.
        /// </summary>
        private void AllPasswordsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedFolder = null;
        }

        /// <summary>
        /// Open the folder context menu attached to the plus button.
        /// </summary>
        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var menu = btn.ContextMenu;

                menu.PlacementTarget = btn;

                menu.IsOpen = true;
            }
        }

        /// <summary>
        /// Assign or unassign an account to a folder based on menu selection.
        /// </summary>
        private void FolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { Tag: (Folder folder, Guid accId) })
            {
                var account = ViewModel.AllAccounts.FirstOrDefault(a => a.AccountId == accId);

                if (account != null)
                {
                    account.FolderId = (account.FolderId == folder.Id) ? null : folder.Id;

                    ViewModel.RefreshList();
                    ViewModel.SaveData();
                }
            }
        }

        /// <summary>
        /// Build folder menu dynamically when opened so it reflects current folders and selection.
        /// </summary>
        private void FolderMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu && menu.PlacementTarget is Button { Tag: Guid accountId })
            {
                menu.Items.Clear();
                var account = ViewModel.AllAccounts.FirstOrDefault(a => a.AccountId == accountId);
                if (account == null) return;

                foreach (var folder in ViewModel.Folders)
                {
                    var item = new MenuItem
                    {
                        Header = (account.FolderId == folder.Id) ? $"✓ {folder.Name}" : folder.Name,
                        Tag = (folder, accountId)
                    };

                    item.Click += FolderMenuItem_Click;
                    menu.Items.Add(item);
                }
            }
        }

        public void RenameFolder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Folder folder })
            {
                var dialog = new NewFolderDialog(folder)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                if (dialog.ShowDialog() == true)
                {
                    ViewModel.SaveData();
                    ViewModel.RefreshList();
                }
            }
        }

        /// <summary>
        /// Delete folder with two behaviours: remove only folder (keep accounts) or remove everything (even accounts in the folder).
        /// </summary>
        public void DeleteFolder_Click(Object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Folder folder })
            {
                var dialog = new DeleteFolderDialog { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };

                if (dialog.ShowDialog() == true)
                {
                    if (dialog.Result == DeleteFolderResult.OnlyFolder)
                    {
                        var accountsInFolder = ViewModel.AllAccounts.Where(a => a.FolderId == folder.Id).ToList();
                        foreach (var acc in accountsInFolder)
                            acc.FolderId = null;
                    }
                    else if (dialog.Result == DeleteFolderResult.Everything)
                    {
                        var accountsToRemove = ViewModel.AllAccounts.Where(a => a.FolderId == folder.Id).ToList();
                        foreach (var acc in accountsToRemove)
                            ViewModel.AllAccounts.Remove(acc);
                    }
                

                    ViewModel.Folders.Remove(folder);

                    if (ViewModel.SelectedFolder == folder)
                    {
                        ViewModel.SelectedFolder = null;
                    }

                    ViewModel.SaveData();
                    ViewModel.RefreshList();
                }
            }
        }

        /// <summary>
        /// Show context menu on right-click for list items. Mark event handled to prevent default selection behaviour.
        /// </summary>
        private void OnPreviewMouseRightBtnDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (sender is ListBoxItem item && item.ContextMenu != null)
            {
                item.ContextMenu.PlacementTarget = item;
                item.ContextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Copy username to clipboard and notify user. Avoid storing sensitive data in logs.
        /// </summary>
        private void CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Account acc })
            {
                string textToCopy = acc.Username;

                if (!string.IsNullOrWhiteSpace(textToCopy))
                {
                    Clipboard.SetText(textToCopy);

                    MessageBox.Show("Username copied to clipoboard! After 30s clipboard will be cleared.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                    timer.Tick += (s, ev) =>
                    {
                        if (Clipboard.GetText() == textToCopy)
                        {
                            Clipboard.Clear();
                        }

                        timer.Stop();
                    };

                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Decrypt password, copy to clipboard, and clear clipboard after 30s if unchanged.
        /// </summary>
        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Account acc })
            {
                string textToCopy = acc.Password;

                if (!string.IsNullOrWhiteSpace(textToCopy))
                {
                    Clipboard.SetText(textToCopy);

                    MessageBox.Show("Password copied to clipoboard! After 30s clipboard will be cleared.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                    timer.Tick += (s, ev) =>
                    {
                        if (Clipboard.GetText() == textToCopy)
                        {
                            Clipboard.Clear();
                        }

                        timer.Stop();
                    };

                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Ensure the grid receives focus on mouse down to allow keyboard shortcuts and focus behaviours.
        /// </summary>
        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is IInputElement element)
            {
                element.Focus();
                Keyboard.ClearFocus();
            }
        }

        /// <summary>
        /// Escape clears search and removes focus from the search box.
        /// </summary>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();

                SearchBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Theme toggle handlers call centralized theme change in app for consistency.
        /// </summary>
        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ChangeTheme(true);
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ChangeTheme(false);
        }
    }
}
