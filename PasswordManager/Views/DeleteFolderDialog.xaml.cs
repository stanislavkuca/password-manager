using System.Collections.ObjectModel;
using System.Windows;
using PasswordManager.Models;

namespace PasswordManager.Views
{
    // Options returned to the caller to indicate how the folder deletion should be handled.
    public enum DeleteFolderResult { OnlyFolder, Everything, Cancel }

    public partial class DeleteFolderDialog : Window
    {
        // Default to Cancel so callers can distinguish explicit choices from no action.
        public DeleteFolderResult Result { get; private set; } = DeleteFolderResult.Cancel;

        public DeleteFolderDialog() => InitializeComponent();

        private void DeleteFolderOnlyBtn_Click(object sender, RoutedEventArgs e) 
        {
            Result = DeleteFolderResult.OnlyFolder;
            DialogResult = true;
        }

        private void DeleteEverythingBtn_Click(object sender, RoutedEventArgs e)
        {
            Result = DeleteFolderResult.Everything;
            DialogResult = true;
        }
    }
}
