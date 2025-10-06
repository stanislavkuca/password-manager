using Microsoft.Win32;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.UI.Helpers
{
    public static class WindowHelper
    {
        /// <summary>
        /// Used for showing a simple window
        /// </summary>
        public static void OpenWindow<TWindow>(Window owner, bool showDialog = true)
            where TWindow : Window, new()
        {
            var window = new TWindow
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (showDialog)
            {
                window.ShowDialog();
            }
            else
                window.Show();
        }

        /// <summary>
        /// Used for showing forms etc.
        /// </summary>
        public static bool? OpenDialog<TWindow>(Window owner, out TWindow window) 
            where TWindow : Window, new()
        {
            window = new TWindow
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            return window.ShowDialog();
        }
    }
}
