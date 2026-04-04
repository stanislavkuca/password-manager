using PasswordManager.Models;
using PasswordManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace PasswordManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Account> DisplayedAccounts { get; } = new();
        public ObservableCollection<Account> AllAccounts { get; private set; } = new();
        public ObservableCollection<Folder> Folders { get; private set; } = new();

        private Folder? _selectedFolder;
        public Folder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                RefreshList();
            }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                RefreshList();
            }
        }

        public void LoadData()
        {
            var data = DataService.Load();
            AllAccounts = new ObservableCollection<Account>(data.Accounts);
            Folders = new ObservableCollection<Folder>(data.Folders);
            RefreshList();
        }

        public void SaveData()
        {
            DataService.Save(new AppData
            {
                Accounts = AllAccounts.ToList(),
                Folders = Folders.ToList()
            });
        }

        public void RefreshList()
        {
            IEnumerable<Account> filtered = AllAccounts;

            if (SelectedFolder != null)
            {
                filtered = filtered
                    .Where(a => a.FolderId == SelectedFolder.Id);
            }
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filtered = filtered
                    .Where(a => a.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                a.Note!.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var finalData = filtered
                .OrderByDescending(a => a.IsFavourite)
                .ThenBy(a => a.Name)
                .ToList();

            DisplayedAccounts.Clear();
            foreach (var acc in finalData)
                DisplayedAccounts.Add(acc);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
