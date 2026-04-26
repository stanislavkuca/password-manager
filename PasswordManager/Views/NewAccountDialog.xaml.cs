using PasswordManager.Models;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace PasswordManager.Views
{
    /// <summary>
    /// Dialog for creating or editing an account.
    /// </summary>
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

                AccountNameTextBox!.Text = account.Name;
                AccountUsernameTextBox!.Text = account.Username;
                AccountPasswordTextBox!.Text = account.Password;
                AccountNoteTextBox!.Text = account.Note ?? string.Empty;
            }
        }

        private void ToggleGenerator_Click(object sender, RoutedEventArgs e)
        {
            if (GeneratorPanel.Visibility == Visibility.Visible)
            {
                GeneratorPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                GeneratorPanel.Visibility = Visibility.Visible;
                GeneratePassword();
            }
        }

        // --- PW GENERATOR LOGIC ---
        private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LengthText != null)
            {
                LengthText.Text = e.NewValue.ToString("0");
                GeneratePassword();
            }
        }

        private void Option_Changed(object sender, RoutedEventArgs e)
        {
            if (IsLoaded) GeneratePassword();
        }

        private void GeneratePassword()
        {
            int length = (int)LengthSlider.Value;
            string validChars = "";

            if (CbUppercase?.IsChecked == true) validChars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (CbLowercase?.IsChecked == true) validChars += "abcdefghijklmnopqrstuvwxyz";
            if (CbNumbers?.IsChecked == true) validChars += "0123456789";
            if (CbSpecial?.IsChecked == true) validChars += "!@#$%^&*()_-+=[{]};:<>|./?";

            if (string.IsNullOrEmpty(validChars)) return;

            StringBuilder res = new StringBuilder();
            // Use cryptographic RNG to avoid predictable passwords.
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            AccountPasswordTextBox.Text = res.ToString();
        }


        // --- BUTTON LOGIC ---
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if ((AccountNameTextBox.Text.Length < 4 || AccountUsernameTextBox.Text.Length < 4 || AccountPasswordTextBox.Text.Length < 8))
            {
                MessageBox.Show("Service name and username must be at least 4 characters and password at least 8 characters.");
                return;
            }

            if (AccountToEdit != null)
            {
                AccountToEdit.Name = AccountNameTextBox!.Text;
                AccountToEdit.Username = AccountUsernameTextBox!.Text;
                // store encrypted password consistently
                AccountToEdit.Password = AccountPasswordTextBox!.Text;
                AccountToEdit.Note = AccountNoteTextBox!.Text;
                CreatedAccount = AccountToEdit;
            }
            else
            {
                CreatedAccount = new Account(
                    AccountNameTextBox!.Text,
                    AccountUsernameTextBox!.Text,
                    AccountPasswordTextBox!.Text,
                    AccountNoteTextBox!.Text
                );
            }

            DialogResult = true;
            Close();
        }
    }
}
