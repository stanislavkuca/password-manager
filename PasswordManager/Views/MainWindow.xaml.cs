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

namespace PasswordManager.Views
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }
        private DispatcherTimer _lockTimer = new();

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            ViewModel.LoadData();
            StartTimer();
        }

        private void StartTimer()
        {
            _lockTimer.Interval = TimeSpan.FromMinutes(5);
            _lockTimer.Tick += (s, e) => Logout();
            _lockTimer.Start();
        }

        private void Logout()
        {
            _lockTimer.Stop();
            _lockTimer.Tick -= (s, e) => Logout();

            ViewModel.SaveData();

            MessageBox.Show("Please sign in again.", "You have been logged out after 5 minutes");
            ((App)Application.Current).RunAuthFlow();

            this.Close();
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchQuery = ((TextBox)sender).Text;
        }

        private void AllPasswordsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedFolder = null;
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var menu = btn.ContextMenu;

                menu.PlacementTarget = btn;

                menu.IsOpen = true;
            }
        }

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

        private void OnPreviewMouseRightBtnDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (sender is ListBoxItem item && item.ContextMenu != null)
            {
                item.ContextMenu.PlacementTarget = item;
                item.ContextMenu.IsOpen = true;
            }
        }

        private void CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Account acc })
            {
                if (!string.IsNullOrWhiteSpace(acc.Username))
                {
                    Clipboard.SetText(acc.Username);

                    MessageBox.Show("Username copied to clipoboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: Account acc })
            {
                string decryptedPassword = Account.DecryptPassword(acc.Password);

                if (!string.IsNullOrWhiteSpace(decryptedPassword))
                {
                    Clipboard.SetText(decryptedPassword);

                    MessageBox.Show("Password copied to clipoboard! After 30s clipboard will be cleared.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                    timer.Tick += (s, ev) =>
                    {
                        if (Clipboard.GetText() == decryptedPassword)
                        {
                            Clipboard.Clear();
                        }

                        timer.Stop();
                    };

                    timer.Start();
                }
            }
        }
    }
}
