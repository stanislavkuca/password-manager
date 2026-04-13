using System.Windows;
using PasswordManager.Models;

namespace PasswordManager.Views
{
    public partial class NewAccountWindow : Window
    {
        public Account? AccountToEdit { get; set; }
        public Account? CreatedAccount { get; private set; }

        public NewAccountWindow(Account? account = null)
        {
            InitializeComponent();
            AccountToEdit = account;

            if (account != null)
            {
                Title = "Edit account";
                ConfirmBtn!.Content = "Save changes";

                AccountNameTextBox!.Text = account.Name ?? string.Empty;
                AccountUsernameTextBox!.Text = account.Username ?? string.Empty;
                AccountPasswordTextBox!.Text = Account.DecryptPassword(account.Password);
                AccountNoteTextBox!.Text = account.Note ?? string.Empty;
            }
        }

        private void OpenGenerator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow generatorWindow = new PasswordGeneratorWindow();
            generatorWindow.Owner = this;

            generatorWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            generatorWindow.Left = this.Left + this.Width + 10;
            generatorWindow.Top = this.Top;

            if (generatorWindow.Left + generatorWindow.Width > SystemParameters.WorkArea.Width)
            {
                generatorWindow.Left = this.Left - generatorWindow.Width - 10;
            }

            if (generatorWindow.ShowDialog() == true)
            {
                AccountPasswordTextBox.Text = generatorWindow.GeneratedPassword;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountToEdit != null)
            {
                AccountToEdit.Name = AccountNameTextBox!.Text;
                AccountToEdit.Username = AccountUsernameTextBox!.Text;
                // store encrypted password consistently
                AccountToEdit.Password = Account.EncryptPassword(AccountPasswordTextBox!.Text);
                AccountToEdit.Note = AccountNoteTextBox!.Text;
                CreatedAccount = AccountToEdit;
            }
            else
            {
                CreatedAccount = new Account(
                    AccountNameTextBox!.Text,
                    AccountUsernameTextBox!.Text,
                    Account.EncryptPassword(AccountPasswordTextBox!.Text),
                    AccountNoteTextBox!.Text
                );
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
