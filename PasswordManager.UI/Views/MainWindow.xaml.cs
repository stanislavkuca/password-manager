using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PasswordManager.UI.Models;

namespace PasswordManager.UI.Views
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Account> Accounts { get; } = new();
        public ObservableCollection<Account> AllAccounts { get; set; } = new();
        public ObservableCollection<Folder> Folders { get; set; } = new();


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

            // Dummy folder
            var work = new Folder("Work");
            var acc1 = new Account("Lorem", "Ipsum", "pass", "note");

            AllAccounts.Add(acc1);
            work.AccountIds.Add(acc1.Id);
            Folders.Add(work);

            // Fill AllAccounts
            AllAccounts.Add(new Account("Email", "admin", "pass123", "My Gmail"));
            AllAccounts.Add(new Account("Steam", "sSAdadsa", "xxxx", "Games"));

            RefreshAccountList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void RefreshAccountList()
        {
            Accounts.Clear();

            var filtered = SelectedFolder == null
                ? AllAccounts
                : AllAccounts.Where(a => SelectedFolder.AccountIds.Contains(a.Id));
            foreach (var acc in filtered)
            {
                Accounts.Add(acc);
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
    }
}
