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

namespace PasswordManager.Views
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Account> Accounts { get; } = new();
        public ObservableCollection<Account> AllAccounts { get; set; } = new();
        public ObservableCollection<Folder> Folders { get; set; } = new();

        DispatcherTimer lockTimer = new DispatcherTimer();

        private Folder? _selectedFolder;
        public Folder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                RefreshAccountList();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            lockTimer.Interval = TimeSpan.FromMinutes(5);
            lockTimer.Tick += LockTimer_LogOff;
            lockTimer.Start();

            var data = DataService.Load();

            foreach (var acc in data.Accounts)
            {
                acc.Password = acc.DecryptPassword(acc.Password);
                AllAccounts.Add(acc);
            }

            foreach (var folder in data.Folders)
            {
                Folders.Add(folder);
            }

            RefreshAccountList();
        }

        private void LockTimer_LogOff(object? sender, EventArgs e)
        {
            lockTimer.Stop();

            var login = new LoginDialog();

            if (login.ShowDialog() == true)
            {
                lockTimer.Start();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void SaveData()
        {
            var accountsToSave = AllAccounts.Select(a => new Account(
                a.Name,
                a.Username,
                a.EncryptPassword(a.Password),
                a.Note
            )
            {
                IsFavourite = a.IsFavourite
            }).ToList();

            var data = new AppData
            {
                Accounts = accountsToSave,
                Folders = Folders.ToList(),
            };

            DataService.Save(data);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var data = new AppData
            {
                Accounts = AllAccounts.ToList(),
                Folders = Folders.ToList()
            };

            DataService.Save(data);

            base.OnClosing(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void RefreshAccountList()
        {
            Accounts.Clear();

            IEnumerable<Account> source;

            if (SelectedFolder == null)
            {
                source = AllAccounts;
            }
            else
            {
                source = SelectedFolder.AccountIds
                    .Select(id => AllAccounts.FirstOrDefault(a => a.Id == id))
                    .Where(a => a != null)
                    .Cast<Account>();
            }

            string query = SearchBox.Text?.Trim().ToLower() ?? "";

            if (!string.IsNullOrEmpty(query))
            {
                source = source.Where(a =>
                (a.Name?.ToLower().Contains(query) ?? false) ||
                (a.Note?.ToLower().Contains(query) ?? false));
            }

            source = source
                .OrderByDescending(a => a.IsFavourite)
                .ThenBy(a => a.Name);

            foreach (var account in source)
            {
                Accounts.Add(account);
            }
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e) 
        { 
            var window = new NewAccountWindow 
            { 
                Owner = this, 
                WindowStartupLocation = WindowStartupLocation.CenterOwner 
            }; 
            
            if (window.ShowDialog() == true && window.CreatedAccount != null) 
            { 
                var acc = window.CreatedAccount; 
                
                AllAccounts.Add(acc); 
                SaveData();
                
                if (SelectedFolder != null) 
                { 
                    SelectedFolder.AccountIds.Remove(acc.Id); 
                } 
                
                RefreshAccountList(); 
            } 
        }

        private void NewFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewFolderDialog
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (window.ShowDialog() == true && !string.IsNullOrWhiteSpace(window.CreatedFolderName))
            {
                var folder = new Folder(window.CreatedFolderName);

                Folders.Add(folder);
                SaveData();
            }
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Account account)
            {
                var dialog = new DeleteConfirmationWindow(account)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                if (dialog.ShowDialog() == true)
                {
                    DeleteAccount(account.Id);
                    SaveData();
                }
            }
        }

        private void DeleteAccount(Guid accountId)
        {
            var acc = AllAccounts.FirstOrDefault(a => a.Id == accountId);
            if (acc != null)
                AllAccounts.Remove(acc);

            foreach (var folder in Folders)
            {
                if (folder.AccountIds.Contains(accountId))
                    folder.AccountIds.Remove(accountId);
            }
            SaveData();
            RefreshAccountList();
        }

        private void AllPasswords_Click(object sender, RoutedEventArgs e)
        {
            SelectedFolder = null;
            FoldersListBox.SelectedItem = null;
            RefreshAccountList();
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var menu = btn.ContextMenu;
                menu.PlacementTarget = btn;

                FolderMenu_Opened(menu, new RoutedEventArgs());

                menu.IsOpen = true;
            }
        }

        private void FolderMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
            {
                if (menu.PlacementTarget is Button btn && btn.Tag is Guid accountId)
                {
                    menu.Items.Clear();

                    foreach (var folder in Folders)
                    {
                        bool alreadyInFolder = folder.AccountIds.Contains(accountId);

                        var item = new MenuItem
                        {
                            Header = alreadyInFolder ? $"✓ {folder.Name}" : folder.Name,
                            Tag = (folder, accountId)
                        };

                        item.Click += FolderMenuItem_Click;
                        menu.Items.Add(item);
                    }
                }
            }
        }

        private void FolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item && item.Tag is (Folder folder, Guid accountId))
            {
                if (folder.AccountIds.Contains(accountId))
                {
                    folder.AccountIds.Remove(accountId);
                }
                else
                {
                    folder.AccountIds.Add(accountId);
                }

                if (SelectedFolder == folder)
                {
                    RefreshAccountList();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshAccountList();
        }

        private void FavouriteButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Account account)
            {
                account.IsFavourite = !account.IsFavourite;
                RefreshAccountList();
            }
        }
    }
}
