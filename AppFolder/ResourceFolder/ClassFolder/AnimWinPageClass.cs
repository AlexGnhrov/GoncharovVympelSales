using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoncharovCarPartsAS.AppFolder.ResourceFolder.ClassFolder
{
   public static class AnimWinPageClass
    {
        public static async Task AnimWinClose(this Window window)
        {

            window.IsEnabled = false;

            await Task.Delay(500);

        }

        public static async Task AnimWinClose(this Frame frame)
        {

            frame.IsEnabled = false;

            await Task.Delay(500);

        }

    }
}
