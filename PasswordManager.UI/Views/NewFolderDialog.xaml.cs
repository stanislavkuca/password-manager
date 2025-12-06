using PasswordManager.UI.Models;
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

namespace PasswordManager.UI.Views
{
    /// <summary>
    /// Interaction logic for NewFolderDialog.xaml
    /// </summary>
    public partial class NewFolderDialog : Window
    {
        public string? CreatedFolderName { get; private set; }

        public NewFolderDialog()
        {
            InitializeComponent();
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = FolderNameTextBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(this, "Please enter a folder name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CreatedFolderName = name;
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
