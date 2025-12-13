using System.Windows;
using PasswordManager.UI.Models;

namespace PasswordManager.UI.Views
{
    public partial class NewAccountWindow : Window
    {
        public Account? CreatedAccount { get; private set; }

        public NewAccountWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreatedAccount = new Account(
                AccountNameTextBox.Text,
                AccountUsernameTextBox.Text,
                AccountPasswordTextBox.Text,
                AccountNoteTextBox.Text
            );

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
