using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.GlobalClassFolder
{
    public static class PhotoImageClass
    {
        public static BitmapImage GetImageFromBytes(byte[] array)
        {
            if (array != null)
            {

                using (MemoryStream ms = new MemoryStream(array, 0, array.Length))
                {

                    var image = new BitmapImage();

                    image.BeginInit();


                    image.CacheOption = BitmapCacheOption.OnLoad;

                    image.StreamSource = ms;

                    image.EndInit();

                    return image;
                }
            }
            return null;
        }


        public static byte[] SetImageToBytes(ref string fileName)
        {
            try
            {
                Bitmap bitmap = new Bitmap(fileName);
                ImageFormat imageFormat = bitmap.RawFormat;

                Image imageToConvert = Image.FromFile(fileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, imageFormat);

                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка вставки фотографии", ex, MessageCode.Error).ShowDialog();
                fileName = "";
            }
            return null;
        }


        private static TimeSpan blockTranz;

        public static void AddPhoto(ImageBrush imageBrush, ref string selectedPhoto)
        {

            if (blockTranz != null && blockTranz > DateTime.Now.TimeOfDay) return;


            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.png *.jpeg *.jpg)|*.png;*.jpeg;*.jpg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            if (openFileDialog.ShowDialog() == true)
            {
                selectedPhoto = openFileDialog.FileName;

                if (SetImageToBytes(ref selectedPhoto) == null)
                {
                    selectedPhoto = "";
                    return;
                }

                imageBrush.ImageSource = GetImageFromBytes(SetImageToBytes(ref selectedPhoto));
            }

            blockTranz = DateTime.Now.AddSeconds(0.2).TimeOfDay;
        }

    }
}
