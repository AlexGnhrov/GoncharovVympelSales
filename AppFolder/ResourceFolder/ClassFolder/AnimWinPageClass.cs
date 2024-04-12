using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder
{
    public static class AnimWinPageClass
    {
        public static async Task AnimWinClose(this Window window)
        {

            window.IsEnabled = false;



            await Task.Delay(320);

        }

        public static async Task AnimWinClose(this Frame frame)
        {

            frame.IsEnabled = false;

            await Task.Delay(320);

            GlobalVarriabels.MainWindow.WinBorder.IsEnabled = true;
            GlobalVarriabels.MainWindow.LeftToolBarSP.IsEnabled = true;
            GlobalVarriabels.MainWindow.CloseWinBTN.IsEnabled = true;

            frame.Navigate(null);

        }

        public static void FrameErrorBack(this Frame frame)
        {

            GlobalVarriabels.MainWindow.WinBorder.IsEnabled = true;
            GlobalVarriabels.MainWindow.LeftToolBarSP.IsEnabled = true;
            GlobalVarriabels.MainWindow.CloseWinBTN.IsEnabled = true;

            frame.Navigate(null);

        }

    }
}
