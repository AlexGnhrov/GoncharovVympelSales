using GoncharovVympelSale.AppFolder.WinFolder;
using System.Windows;

namespace GoncharovVympelSale.AppFolder.ClassFolder
{
    public static class ConfigSettingsClass
    {
        public static void SaveConfig(this MainWin window)
        {
            if (window.Width >= window.MinWidth && window.Height >= window.MinHeight)
            {
                Properties.Settings.Default.WinHeight = window.Height;
                Properties.Settings.Default.WinWidth = window.Width;

                Properties.Settings.Default.WinisResize = window.WindowState == WindowState.Maximized;

                Properties.Settings.Default.Save();
            }

        }


        public static void LoadSettings(this MainWin window)
        {

            window.Height = Properties.Settings.Default.WinHeight;
            window.Width = Properties.Settings.Default.WinWidth;

            if (Properties.Settings.Default.WinisResize)
                window.WindowState = WindowState.Maximized;

        }



    }
}
