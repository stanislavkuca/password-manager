using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordManager.Views
{
    public partial class NewFolderDialog : Window
    {
        public Folder? FolderToEdit { get; set; }
        public Folder? CreatedFolder { get; private set; }

        public NewFolderDialog(Folder? folder = null)
        {
            InitializeComponent();
            FolderToEdit = folder;

            if (folder != null)
            {
                Title = "Rename Folder";
                ConfirmBtn.Content = "Rename folder";

                FolderNameTextBox!.Text = folder.Name ?? string.Empty;
            }

            FolderNameTextBox.Focus();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FolderNameTextBox.Text))
            {
                MessageBox.Show("Please enter a folder name.", "Warning");
                return;
            }

            if (FolderToEdit != null)
            {
                FolderToEdit.Name = FolderNameTextBox!.Text;
                CreatedFolder = FolderToEdit;
            }
            else
            {
                CreatedFolder = new Folder(FolderNameTextBox.Text);
            }
            
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
