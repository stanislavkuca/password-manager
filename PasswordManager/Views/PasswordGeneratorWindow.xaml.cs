using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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
    /// <summary>
    /// Interaction logic for PasswordGeneratorWindow.xaml
    /// </summary>
    public partial class PasswordGeneratorWindow : Window
    {
        public string? GeneratedPassword { get; private set; }

        public PasswordGeneratorWindow()
        {
            InitializeComponent();
        }

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

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            GeneratePassword();
        }

        private void GeneratePassword()
        {
            if (ResultTextBox == null) return;

            int length = (int)(LengthSlider?.Value ?? 16);
            string validChars = "";

            if (CbUppercase?.IsChecked == true) validChars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (CbLowercase?.IsChecked == true) validChars += "abcdefghijklmnopqrstuvwxyz";
            if (CbNumbers?.IsChecked == true) validChars += "0123456789";
            if (CbSpecial?.IsChecked == true) validChars += "!@#$%^&*()_-+=[{]};:<>|./?";

            if (validChars.Length == 0)
            {
                validChars = "abcdefghijklmnopqrstuvwxyz";

                if (CbLowercase != null)
                {
                    CbLowercase.Checked -= Option_Changed;
                    CbLowercase.IsChecked = true;
                    CbLowercase.Checked += Option_Changed;
                }
            }

            StringBuilder res = new StringBuilder();

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            ResultTextBox.Text = res.ToString();
        }

        private void UsePassword_Click(object sender, RoutedEventArgs e)
        {
            GeneratedPassword = ResultTextBox.Text;
            DialogResult = true;
        }
    }
}
