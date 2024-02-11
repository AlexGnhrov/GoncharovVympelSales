using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GoncharovCarPartsAS.AppFolder.ResourceFolder.ClassFolder
{



    public static class PropertyClass
    {



        //----Иконка для текстового поля слева------------------------------------


        public static readonly DependencyProperty ImageForTBProperty =
            DependencyProperty.RegisterAttached(
                "ImageForTB", typeof(string), typeof(PropertyClass),
                new PropertyMetadata(string.Empty, OnImageForTBChanged));

        public static void SetImageForTB(DependencyObject element, object value)
        {
            element.SetValue(ImageForTBProperty, value);
        }

        public static object GetImageForTB(DependencyObject element)
        {
            return element.GetValue(ImageForTBProperty);
        }

        private static void OnImageForTBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image image && e.NewValue is string imagePath)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                }
                //else
                //{
                //    image.Source = null; // Optionally clear the image source if the path is empty or null
                //}
            }
        }

        //----Текст по верх тексбока-----------------------------------------


        public static readonly DependencyProperty HintTBProperty =
            DependencyProperty.RegisterAttached("HintTB", typeof(string), typeof(PropertyClass),
                new PropertyMetadata(string.Empty));

        public static void SetHintTB(DependencyObject element, string value)
        {
            element.SetValue(HintTBProperty, value);
        }

        public static string GetHintTB(DependencyObject element)
        {
            return (string)element.GetValue(HintTBProperty);
        }



    }



}
