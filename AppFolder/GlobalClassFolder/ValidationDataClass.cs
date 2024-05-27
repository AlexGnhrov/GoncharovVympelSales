using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoncharovVympelSale.AppFolder.ClassFolder
{
    public static class ValidationDataClass
    {
        public static void OnlyNumsTB(this TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public static void OnlyEnglish(this TextCompositionEventArgs e)
        {
            e.Handled = !new Regex($"^[a-zA-Z0-9_';:@!#$%^&*()+№.-]*$").IsMatch(e.Text);
        }

        public static void Login(this TextCompositionEventArgs e)
        {
            e.Handled = !new Regex(@"^[a-zA-Z0-9_!?]*$").IsMatch(e.Text);
        }


        public static void Phone(this TextBox PhoneNumText)
        {
            if (PhoneNumText.Text == "7")
            {
                PhoneNumText.Text = PhoneNumText.Text.Insert(0, "+");
                PhoneNumText.Text += " ";
                PhoneNumText.CaretIndex = 3;
            }
            try
            {
                if (!Keyboard.IsKeyDown(Key.Back))
                {
                    switch (PhoneNumText.Text.Length)
                    {
                        case 4:
                            PhoneNumText.Text = PhoneNumText.Text.Insert(3, "(");
                            PhoneNumText.CaretIndex = 6;
                            break;
                        case 8:
                            PhoneNumText.Text = PhoneNumText.Text.Insert(7, ") ");
                            PhoneNumText.CaretIndex = 10;
                            break;
                        case 12:
                            PhoneNumText.Text = PhoneNumText.Text.Insert(12, "-");
                            PhoneNumText.CaretIndex = 13;
                            break;
                        case 15:
                            PhoneNumText.Text = PhoneNumText.Text.Insert(15, "-");
                            PhoneNumText.CaretIndex = 16;
                            break;
                    }
                }

                else
                {
                    if (PhoneNumText.Text.Length < 4)
                    {
                        PhoneNumText.Text = "+7 ";
                        PhoneNumText.CaretIndex = 4;
                    }
                    switch (PhoneNumText.Text.Length)
                    {
                        case 7:
                            PhoneNumText.Text =
                                PhoneNumText.Text.Remove(PhoneNumText.Text.LastIndexOf("-"));
                            PhoneNumText.CaretIndex = 6;
                            break;
                        case 13:
                            PhoneNumText.Text =
                                PhoneNumText.Text.Remove(PhoneNumText.Text.LastIndexOf("-"));
                            PhoneNumText.CaretIndex = 12;
                            break;
                        case 16:
                            PhoneNumText.Text =
                                PhoneNumText.Text.Remove(PhoneNumText.Text.LastIndexOf("-"));
                            PhoneNumText.CaretIndex = 15;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public static bool isEmail(this TextBox textBox)
        {
            string trimmedTextBox = textBox.Text.Trim();


            if (trimmedTextBox.EndsWith("@ya.ru") || trimmedTextBox.EndsWith("@yandex.ru") ||
                trimmedTextBox.EndsWith("@inbox.ru") || trimmedTextBox.EndsWith("@list.ru") ||
                trimmedTextBox.EndsWith("@bk.ru") || 
                trimmedTextBox.EndsWith("@mail.ru") ||
                trimmedTextBox.EndsWith("@gmail.com"))
            {
                return true;
            }
            return false;
        }

        public static void FloatNums(this TextBox textBox)
        {
            string result = "";
            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };

            bool CommaIsUsing = false;
            int i = 1;

            byte afterCommaleng = 0;

            foreach (char c in textBox.Text)
            {

                if (Array.IndexOf(validChars, c) != -1)
                {

                    if (c == ',' || c == '.')
                    {
                        if (i != 1 && !CommaIsUsing)
                        {
                            result += ",";
                            CommaIsUsing = true;
                        }
                    }
                    else if (CommaIsUsing && afterCommaleng < 2)
                    {
                        result += c;
                        ++afterCommaleng;
                    }
                    else if (result.Length < textBox.MaxLength - 3 && afterCommaleng < 2)
                    {
                        result += c;
                    }
                }


                ++i;
            }

            textBox.Text = result;


            if (!Keyboard.IsKeyDown(Key.Back) &&
                (textBox.CaretIndex == textBox.Text.Length || textBox.CaretIndex == 0))
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }




        //public static void Phone(this TextBox PhoneNumText)
        //{
        //    string result = "";
        //    int saveCaret = PhoneNumText.CaretIndex;
        //    string textBoxTimmed = PhoneNumText.Text.Trim();


        //    char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        //    int i = 1;

        //    foreach (char c in textBoxTimmed)
        //    {

        //        if (Array.IndexOf(validChars, c) != -1)
        //        {

        //            if (i == 1)
        //            {
        //                result += "+";
        //                result += c;
        //                result += " ";

        //                saveCaret += 3;
        //            }
        //            else if(i == 6)
        //            {
        //                result += ")";
        //                saveCaret += 1;
        //            }
        //            //else if (i == 3)
        //            //{
        //            //    result += "(";

        //            //    //if (!Keyboard.IsKeyDown(Key.Back) && (!(textBoxTimmed.Length > 5) || !(textBoxTimmed.Length > 2)))
        //            //    //{
        //            //    ++saveCaret;
        //            //    //}
        //            //}
        //            result += c;
        //        }

        //        ++i;
        //    }

        //    PhoneNumText.Text = result;
        //    PhoneNumText.CaretIndex = saveCaret;

        //}



        public static void PassportNum(this TextBox textBox)
        {
            string result = "";
            int saveCaret = textBox.CaretIndex;
            string textBoxTimmed = textBox.Text.Trim();


            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            int i = 1;

            foreach (char c in textBoxTimmed)
            {

                if (Array.IndexOf(validChars, c) != -1)
                {

                    result += c;

                    if (i == 2 || i == 5)
                    {
                        result += " ";

                        if (!Keyboard.IsKeyDown(Key.Back) && (!(textBoxTimmed.Length > 5) || !(textBoxTimmed.Length > 2)))
                        {
                            ++saveCaret;
                        }

                    }
                }

                ++i;
            }

            textBox.Text = result;
            textBox.CaretIndex = saveCaret;

        }


        public static void StorageNum(this TextBox textBox)
        {
            string result = "";
            int saveCaret = textBox.CaretIndex;
            string textBoxTimmed = textBox.Text.Trim();


            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            int i = 1;

            foreach (char c in textBoxTimmed)
            {

                if (Array.IndexOf(validChars, c) != -1)
                {

                    result += c;

                    if (i == 2 || i == 5)
                    {
                        result += "-";

                        if (!Keyboard.IsKeyDown(Key.Back) && (!(textBoxTimmed.Length > 5) || !(textBoxTimmed.Length > 2)))
                        {
                            ++saveCaret;
                        }


                    }
                }

                ++i;
            }

            textBox.Text = result;
            textBox.CaretIndex = saveCaret;

        }



        public static void CodeDepNum(this TextBox textBox)
        {
            string result = "";
            int saveCaret = textBox.CaretIndex;
            string textBoxTimmed = textBox.Text.Trim();


            char[] validChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            int i = 1;

            foreach (char c in textBoxTimmed)
            {

                if (Array.IndexOf(validChars, c) != -1)
                {

                    result += c;

                    if (i == 3)
                    {
                        result += "-";

                        if (!Keyboard.IsKeyDown(Key.Back) && (!(textBoxTimmed.Length > 5) || !(textBoxTimmed.Length > 2)))
                        {
                            ++saveCaret;
                        }


                    }
                }

                ++i;
            }

            textBox.Text = result;
            textBox.CaretIndex = saveCaret;

        }


        public static bool CheckLengthPhone(this TextBox phone)
        {
            string CheckLegnt;

            CheckLegnt = phone.Text.Replace("(", " ");
            CheckLegnt = CheckLegnt.Replace(")", "");
            CheckLegnt = CheckLegnt.Replace(" ", "");
            CheckLegnt = CheckLegnt.Replace("-", "");

            if (CheckLegnt.Length < 12)

                return false;

            return true;
        }


        private static string BigSym = "QWERTYUIOPASDFGHJKLZXCVBNM";
        private static string LitSym = "qwertyuioasdfghjklzxcvbnm";
        private static string someSym = "!@#$%^&*():,./";
        private static string numSym = "1234567890";

        public static bool CheckPassword(this PasswordBox password, out string message)
        {

            message = "";

            if (password.Password.Length < 6)
            {

                message = "Пароль должен содержать не меньше 6 символов";
                return false;
            }
            if (password.Password.IndexOfAny(numSym.ToCharArray()) == -1)
            {

                message = "Пароль должен cодержать одну цифру";
                return false;
            }
            if (password.Password.IndexOfAny(BigSym.ToCharArray()) == -1)
            {

                message = "Пароль должен содержать одну прописную букву";
                return false;
            }
            if (password.Password.IndexOfAny(LitSym.ToCharArray()) == -1)
            {

                message = "Пароль должен содержать одну строчную букву";
                return false;
            }
            if (password.Password.IndexOfAny(someSym.ToCharArray()) == -1)
            {

                message = "Пароль должен содержать один из символов: " + someSym;
                return false;
            }
            return true;

        }




        public static void CheckFields(ref bool error, PasswordBox textBox)
        {

            if (string.IsNullOrWhiteSpace(textBox.Password))
            {
                textBox.Tag = GlobalVarriabels.ErrorTag;

                error = true;
            }

            if (error) return;
            error = false;
        }

        public static void CheckFields(ref bool error, TextBox textBox)
        {

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Tag = GlobalVarriabels.ErrorTag;

                error = true;
            }

            if (error) return;
            error = false;
        }

        public static void CheckFields(ref bool error, ComboBox textBox, bool isValue)
        {

            if (isValue)
            {
                if (textBox.SelectedValue == null)
                {
                    textBox.Tag = GlobalVarriabels.ErrorTag;

                    error = true;
                }
            }
            else if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Tag = GlobalVarriabels.ErrorTag;

                error = true;
            }


            if (error) return;
            error = false;
        }

        public static void CheckFields(ref bool error, DatePicker textBox)
        {

            if (textBox.SelectedDate == null)
            {
                textBox.Tag = GlobalVarriabels.ErrorTag;


                error = true;
            }

            if (error) return;
            error = false;
        }

    }
}
