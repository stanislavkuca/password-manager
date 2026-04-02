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
            DisplayedAccounts.Clear();

            var filtered = AllAccounts.AsEnumerable();

            if (SelectedFolder != null)
                filtered = filtered.Where(a => SelectedFolder.AccountIds.Contains(a.Id));

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                filtered = filtered.Where(a => a.Name.ToLower().Contains(q) || (a.Note?.ToLower().Contains(q) ?? false));
            }

            var sorted = filtered.OrderByDescending(a => a.IsFavourite).ThenBy(a => a.Name);

            foreach (var acc in sorted) DisplayedAccounts.Add(acc);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
