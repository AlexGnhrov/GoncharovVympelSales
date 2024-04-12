using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder
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


        //----Установка длины для Editable CombobBox-----------------------------------------



        public static readonly DependencyProperty LengthTextForCBProperty = DependencyProperty.RegisterAttached("LengthTextForCB",
                                                                                                        typeof(int),
                                                                                                        typeof(PropertyClass));

        public static void SetLengthTextForCB(DependencyObject element, int value)
        {
            element.SetValue(LengthTextForCBProperty, value);
        }

        public static int GetLengthTextForCB(DependencyObject element)
        {
            return (int)element.GetValue(LengthTextForCBProperty);
        }

        //----Установка WrapText для Editable CombobBox-----------------------------------------



        public static readonly DependencyProperty WrapTextForCBProperty = DependencyProperty.Register("WrapTextForCB", typeof(TextWrapping), typeof(PropertyClass));

        public static TextWrapping GetWrapTextForCB(DependencyObject obj)
        {
            return (TextWrapping)obj.GetValue(WrapTextForCBProperty);
        }

        public static void SetWrapTextForCB(DependencyObject obj, TextWrapping value)
        {
            obj.SetValue(WrapTextForCBProperty, value);
        }


        //----Установка Показать пароль для PasswordBox-----------------------------------------



        public static readonly DependencyProperty ShowPasswordProperty = DependencyProperty.RegisterAttached("ShowPassword",
                                                                                                        typeof(bool),
                                                                                                        typeof(PropertyClass));

        public static void SetShowPassword(DependencyObject element, bool value)
        {
            element.SetValue(ShowPasswordProperty, value);
        }

        public static bool GetShowPassword(DependencyObject element)
        {
            return (bool)element.GetValue(ShowPasswordProperty);
        }

    }
}
