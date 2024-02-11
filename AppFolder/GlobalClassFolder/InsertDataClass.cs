using GoncharovCarPartsAS.AppFolder.WinFolder;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoncharovCarPartsAS.AppFolder.ClassFolder
{
    public static class InsertDataClass
    {
        public static int InsertDataInt(TextBox InsertingData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(InsertingData.Text))
                {
                    return Convert.ToInt32(InsertingData.Text);
                }

            }
            catch (Exception ex)
            {
                if(ex is OverflowException)
                {
                    new MessageWin(null, $"Превышен лимит в {int.MaxValue}.\n" +
                                         $"Поэтому значение поставлено на 0",
                                         (int)GlobalVarriabels.MessageCode.Error).Show();
                }
            }

            return 0;

        }


        public static long InsertDataLong(TextBox InsertingData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(InsertingData.Text))
                {
                    return Convert.ToInt64(InsertingData.Text);
                }

            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {

                    new MessageWin(null, $"Превышен лимит в {long.MaxValue}.\n" +
                                         $"Поэтому значение поставлено на 0",
                                         (int)GlobalVarriabels.MessageCode.Error).Show();
                }
            }

            return 0;

        }

        public static double InsertDataFloat(TextBox InsertingData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(InsertingData.Text))
                {
                    return Convert.ToDouble(InsertingData.Text);
                }

            }
            catch {}

            return 0;

        }




        public static void PasteOnlyNums(this ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Command == ApplicationCommands.Paste)
                {
                    string text = Clipboard.GetText();

                    if (Convert.ToInt32(text) == 0)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch 
            {
                e.Handled = true;
            }
        }

        public static void PasteOnlyNumsFloat(this ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Command == ApplicationCommands.Paste)
                {
                    string text = Clipboard.GetText();

                    if (Convert.ToDouble(text) <= 0)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch 
            {
                e.Handled = true;
            }
        }







        public static long FormatPhoneForDB(this TextBox textBox)
        {
            string CheckLegnt;

            CheckLegnt = textBox.Text.Replace("(", " ");
            CheckLegnt = CheckLegnt.Replace(")", "");
            CheckLegnt = CheckLegnt.Replace(" ", "");
            CheckLegnt = CheckLegnt.Replace("-", "");
            CheckLegnt = CheckLegnt.Replace("+", "");

            return Convert.ToInt64(CheckLegnt);
        }

    }
}

