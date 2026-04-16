using PasswordManager.Models;
using PasswordManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace PasswordManager.ViewModels
{
    /// <summary>
    /// ViewModel holds UI-facing collections and simple filtering/search logic.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Accounts currently shown in the UI (filtered + sorted).
        public ObservableCollection<Account> DisplayedAccounts { get; } = new();

        // Source collections loaded from persistence.
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

        /// <summary>
        /// Load persisted data and replace internal collections with fresh instances.
        /// </summary>
        public void LoadData()
        {
            var data = DataService.Load();
            AllAccounts = new ObservableCollection<Account>(data.Accounts);
            Folders = new ObservableCollection<Folder>(data.Folders);
            RefreshList();
        }

        /// <summary>
        /// Persist current state; convert ObservableCollections to plain lists for storage.
        /// </summary>
        public void SaveData()
        {
            DataService.Save(new AppData
            {
                Accounts = AllAccounts.ToList(),
                Folders = Folders.ToList()
            });
        }

        /// <summary>
        /// Rebuild DisplayedAccounts from AllAccounts applying folder filter, search, and ordering.
        /// </summary>
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
