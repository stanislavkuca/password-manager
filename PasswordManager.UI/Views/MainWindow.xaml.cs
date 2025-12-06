using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Dummy folder
            var work = new Folder("Work");
            work.Accounts.Add(new Account("Lorem", "Ipsum", "pass", "note"));
            Folders.Add(work);

            // Fill AllAccounts
            AllAccounts.Add(new Account("Email", "admin", "pass123", "My Gmail"));
            AllAccounts.Add(new Account("Steam", "gamer", "xxxx", "Games"));

            RefreshAccountList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private Folder? _selectedFolder;
        public Folder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                    RefreshAccountList();
                }
            }
        }

        private void RefreshAccountList()
        {
            Accounts.Clear();

            if (SelectedFolder == null)
            {
                foreach (var acc in AllAccounts)
                    Accounts.Add(acc);
            }
            else
            {
                foreach (var acc in SelectedFolder.Accounts)
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
                if (SelectedFolder != null)
                    SelectedFolder.Accounts.Add(window.CreatedAccount);
                else
                    AllAccounts.Add(window.CreatedAccount);

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

            if (window.ShowDialog() == true && window.CreatedFolderName != null)
            {
                if (!string.IsNullOrWhiteSpace(window.CreatedFolderName))
                {
                    Folders.Add(new Folder(window.CreatedFolderName));
                }
            }
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Account account)
            {
                var dialog = new DeleteConfirmationWindow(Accounts, account)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dialog.ShowDialog();
            }
        }

        private void AllPasswords_Click(object sender, RoutedEventArgs e)
        {
            SelectedFolder = null;
            FoldersListBox.SelectedIndex = -1;
        }
    }
}
