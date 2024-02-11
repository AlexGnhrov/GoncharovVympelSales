using GoncharovCarPartsAS.AppFolder.ClassFolder;
using GoncharovCarPartsAS.AppFolder.DataFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GoncharovCarPartsAS.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWin.xaml
    /// </summary>
    public partial class AuthorizationWin : Window
    {

        private double shrinkWinHeight = -325;
        private string EmailClient = "";

        public AuthorizationWin()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void EnableAuhtButton()
        {
            SignInBTN.IsEnabled = !(string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordPB.Password));
        }



        bool CodeAccepted = false;



        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            selectedTB.Tag = "";

            try
            {
                switch (selectedTB.Name)
                {
                    case "LoginTB":
                        EnableAuhtButton();
                        break;
                    case "EmailTB":
                        {
                            if (!string.IsNullOrWhiteSpace(EmailTB.Text))
                            {
                                bool isEmail = selectedTB.isEmail();

                                if (isEmail)
                                {
                                    if (!blockGetCodeBtn)
                                    {
                                        RegBTN.IsEnabled = isEmail;
                                    }
                                    else
                                    {
                                        CodeTB.IsEnabled = true;
                                    }

                                    ErrorRegLB.Visibility = Visibility.Hidden;
                                }
                                else
                                {
                                    showErrorMessage(ErrorRegLB, "Неверный формат почты");

                                    RegBTN.IsEnabled = CodeTB.IsEnabled = isEmail;

                                }


                            }

                        }
                        break;
                    case "CodeTB":
                        {
                            ErrorRegLB.Visibility = Visibility.Hidden;

                            if (selectedTB.Text.Length > 5)
                            {
                                if (selectedTB.Text == CodeForReg)
                                {
                                    timer.Stop();

                                    Thread.Sleep(300);

                                    await DisapearGetCode();



                                    blockGetCodeBtn = false;
                                    RegBTN.IsEnabled = true;

                                    if (RegLogoLB.Content.ToString() == "Восстановление")
                                    {
                                        RegBTN.Content = "Восстановить";

                                        ResetCodeSP.Visibility = Visibility.Visible;

                                        CodeAccepted = true;
                                    }
                                    else
                                    {
                                        RegBTN.Content = "Зарегистрироваться";
                                        CodeAccepted = true;

                                        await ExpandWinAnim();
                                        await ApperRegForm();

                                    }

                                    return;
                                }


                                Keyboard.ClearFocus();

                                selectedTB.Text = "";

                                showErrorMessage(ErrorRegLB, "Неверный код");

                                selectedTB.Tag = GlobalVarriabels.ErrorTag;

                            }

                        }

                        break;
                    case "PhoneRegTB":
                        if (!selectedTB.Text.StartsWith("+7"))
                        {
                            ErrorRegLB.Visibility = Visibility.Hidden;
                            showErrorMessage(ErrorRegLB, "Неверный формат телефона");

                            selectedTB.Tag = GlobalVarriabels.ErrorTag;
                        }
                        else
                        {
                            ErrorRegLB.Visibility = Visibility.Hidden;
                        }
                        selectedTB.Phone();
                        break;
                    case "PasswordRegTB":
                        {
                            string messageError;

                            if (!PasswordRegTB.CheckPassword(out messageError))
                            {
                                PasswordRegTB.Tag = GlobalVarriabels.ErrorTag;

                                showErrorMessage(ErrorRegLB, messageError);

                            }
                            else
                            {
                                ErrorRegLB.Visibility = Visibility.Hidden;
                            }

                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegLB, ex.Message);
            }

        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox password = sender as PasswordBox;

            switch (password.Name)
            {
                case "PasswordPB":
                    {
                        PasswordAuthHintLB.Visibility = password.Password.Length < 1 ? Visibility.Visible : Visibility.Collapsed;
                        EnableAuhtButton();
                    }
                    break;
                case "PasswordResetPB":
                    {
                        PasswordResetHintLB.Visibility = password.Password.Length < 1 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
                case "PasswordReset2PB":
                    {
                        PasswordResetHint2LB.Visibility = password.Password.Length < 1 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
                default:
                    break;
            }


        }

        private async void SignInBTN_Click(object sender, RoutedEventArgs e)
        {

            //DBEntities.NullContext();

            //SignInBTN.IsEnabled = false;

            //ErrorLB.Visibility = Visibility.Hidden;

            //Staff staff;
            //Client client;

            //string password = string.Format("{0:x}", PasswordPB.Password.Trim().GetHashCode());

            //bool skipStaff = false;
            //string login = LoginTB.Text.Trim();

            //try
            //{
            //    if (login.StartsWith("+7") | login.StartsWith("7"))
            //    {
            //        long phone = LoginTB.FormatPhoneForDB();

            //        client = DBEntities.GetContext().Client.FirstOrDefault(u => u.PhoneNum == phone);

            //        if (client == null || client.Password != password)
            //        {
            //            showErrorMessage(ErrorLB, "Неверный телефон или пароль");
            //            return;
            //        }

            //        GlobalVarriabels.currentUserID = client.ClientID;

            //        skipStaff = true;
            //    }


            //    if (LoginTB.isEmail())
            //    {
            //        password = string.Format("{0:x}", PasswordPB.Password.GetHashCode());

            //        client = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == LoginTB.Text.Trim());

            //        if (client == null || client.Password != password)
            //        {
            //            showErrorMessage(ErrorLB, "Неверная почта или пароль");
            //            return;
            //        }

            //        GlobalVarriabels.currentUserID = client.ClientID;

            //        skipStaff = true;

            //    }



            //    if (!skipStaff)
            //    {

            //        staff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.Login == LoginTB.Text.Trim());


            //        if (staff == null || staff.Password != PasswordPB.Password)
            //        {
            //            showErrorMessage(ErrorLB, "Неверный логин или пароль");
            //            return;
            //        }


            //        GlobalVarriabels.curDepCompanyID = staff.DepartamentID;
            //        GlobalVarriabels.CurrentRoleID = (GlobalVarriabels.RoleName)staff.RoleID;
            //    }



            await AnimWinClose();
            new MainWin().Show();
            Close();

            //}
            //catch (Exception ex)
            //{
            //    showErrorMessage(ErrorLB, ex.Message);

            //}
            //finally
            //{
            //    SignInBTN.IsEnabled = true;
            //}
        }

        private void RegBTN_Click(object sender, RoutedEventArgs e)
        {

            ErrorRegLB.Visibility = Visibility.Hidden;


            if (RegLogoLB.Content.ToString() == "Восстановление")
            {
                if (CodeAccepted)
                {
                    ResettingPassword();
                }
                else
                {
                    ReserPasswordGetCode();
                }
                return;
            }


            if (CodeAccepted)
            {
                Registraition();
            }
            else
            {
                RegistraitionGetCode();
            }




        }

        TimeSpan EndOfBlck;



        private void Timer_Tick(object sender, EventArgs e)
        {
            RepeatCodeTimerTBl.Text = EndOfBlck.Subtract(DateTime.Now.TimeOfDay).ToString("m':'ss");

            if (EndOfBlck <= DateTime.Now.TimeOfDay)
            {
                blockGetCodeBtn = false;
                RegBTN.IsEnabled = true;
                timer.Stop();
                RepeatCodePanelSP.Opacity = 0.56;
                return;
            }

        }

        DispatcherTimer timer;

        string CodeForReg = "";
        bool blockGetCodeBtn = false;


        private void TimerAndCode()
        {
            EndOfBlck = DateTime.Now.AddSeconds(61).TimeOfDay;

            RepeatCodePanelSP.Opacity = 1;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();


            blockGetCodeBtn = true;
            CodeTB.IsEnabled = true;
            RegBTN.IsEnabled = false;


            CodeGenerator();
        }





        private bool CheckFilesBeforeReg()
        {
            string messageError;
            bool gotError = false;

            if (string.IsNullOrWhiteSpace(PasswordRegTB.Text))
            {
                PasswordRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Введите пароль");
                    gotError = true;

                }
            }
            else if (!PasswordRegTB.CheckPassword(out messageError))
            {
                PasswordRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, messageError);
                    gotError = true;

                }
            }

            if (string.IsNullOrWhiteSpace(PhoneRegTB.Text))
            {
                PhoneRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Введите телефон");

                    gotError = true;
                }
            }
            else if (!PhoneRegTB.CheckLengthPhone())
            {
                PhoneRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Телефон не полностью введён");

                    gotError = true;
                }
            }

            if (string.IsNullOrWhiteSpace(NameRegTB.Text))
            {
                NameRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Введите имя");

                    gotError = true;
                }
            }
            if (string.IsNullOrWhiteSpace(SurnameRegTB.Text))
            {
                SurnameRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Введите фамилию");
                    gotError = true;
                }


            }
            if (DateOfBirthDP.SelectedDate == null)
            {
                DateOfBirthDP.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Выберите дату рождения");

                    gotError = true;
                }
            }
            if (ManChB.IsChecked == false && WomanChB.IsChecked == false)
            {
                if (!gotError)
                {
                    showErrorMessage(ErrorRegLB, "Выберите пол");
                    gotError = true;
                }

            }


            return gotError;
        }


        private async void Registraition()
        {
            DBEntities.NullContext();


            try
            {
                if (CheckFilesBeforeReg()) return;

                long Phone = PhoneRegTB.FormatPhoneForDB();


                Client newClient = DBEntities.GetContext().Client.FirstOrDefault(u => u.PhoneNum == Phone);

                if (newClient != null)
                {
                    showErrorMessage(ErrorRegLB, "Данный телефон уже зарегистрирован");
                    return;
                }

                newClient = new Client();

                newClient.Email = EmailTB.Text.Trim();
                newClient.PhoneNum = PhoneRegTB.FormatPhoneForDB();
                newClient.Password = string.Format("{0:x}", PasswordRegTB.Text.GetHashCode());
                newClient.Name = NameRegTB.Text.Trim();
                newClient.Surname = SurnameRegTB.Text.Trim();
                newClient.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.SelectedDate);
                newClient.GenderID = ManChB.IsChecked == true ? 1 : 2;

                DBEntities.GetContext().Client.Add(newClient);



                ClientEnterPC newPC = new ClientEnterPC();
                newPC.ClientID = newClient.ClientID;
                newPC.NamePC = string.Format("{0:x}", (Environment.MachineName + Environment.UserName).GetHashCode());

                DBEntities.GetContext().ClientEnterPC.Add(newPC);


                DBEntities.GetContext().SaveChanges();

                await AnimWinClose();

                new MainWin().Show();

                Close();

            }
            catch (Exception ex)
            {

                showErrorMessage(ErrorRegLB, ex.Message);
            }


        }


        private void RegistraitionGetCode()
        {

            try
            {
                Client newClient = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == EmailTB.Text.Trim());

                if (newClient != null)
                {
                    showErrorMessage(ErrorRegLB, "Данная почта уже зарегистрирована");
                    return;
                }

                EmailClient = EmailTB.Text.Trim();

                TimerAndCode();

                CodeGenerator();



            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegLB, ex.Message);
            }


        }


        private void ResettingPassword()
        {
            try
            {
                UnderTBlClick_MouseLeftButtonUp(BackAuthTBl, null);
            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegLB, ex.Message);
            }

        }

        private void ReserPasswordGetCode()
        {
            try
            {
                TimerAndCode();

                CodeGenerator();
            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegLB, ex.Message);
            }

        }




        TimeSpan blockTranz;

        private async void UnderTBlClick_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (blockTranz != null && blockTranz > DateTime.Now.TimeOfDay) return;

            TextBlock SelectedTBl = sender as TextBlock;

            blockTranz = DateTime.Now.AddSeconds(0.5).TimeOfDay;

            Keyboard.ClearFocus();


            ThicknessAnimation gridTransit = new ThicknessAnimation();
            PowerEase elasticEase = new PowerEase();


            elasticEase.Power = 2.6;
            var time = TimeSpan.FromSeconds(0.24);


            if (SelectedTBl.Name == "RegTBl")
            {
                gridTransit.To = new Thickness(shrinkWinHeight, 0, 0, 0);
                gridTransit.Duration = time;

                RegLogoLB.Content = Title = "Регистрация";


            }
            else
            {

                gridTransit.To = new Thickness(0);
                gridTransit.Duration = time;

                Title = "Авторизация";

            }


            gridTransit.EasingFunction = elasticEase;

            MainGrid.BeginAnimation(Grid.MarginProperty, gridTransit);


            if (CodeAccepted)
            {
                await Task.Delay(125);

                Keyboard.ClearFocus();

                RegBTN.Content = "Получить код";
                RegBTN.IsEnabled = false;


                //------ресет кода получения---------------

                EmailTB.Text = "";
                CodeTB.Text = "";
                CodeTB.IsEnabled = false;

                //------ресет регистраци текска---------------

                PasswordRegTB.Text = "";
                PhoneRegTB.Text = "";
                NameRegTB.Text = "";
                SurnameRegTB.Text = "";
                DateOfBirthDP.SelectedDate = null;
                WomanChB.IsChecked = false;
                ManChB.IsChecked = false;


                //------Ресет регистрации тегов---------------

                PasswordRegTB.Tag = "";
                EmailTB.Tag = "";
                CodeTB.Tag = "";
                PhoneRegTB.Tag = "";
                NameRegTB.Tag = "";
                SurnameRegTB.Tag = "";
                DateOfBirthDP.Tag = null;

                //------Ресет восстановаление пароля тег---------------

                PasswordResetPB.Password = "";
                PasswordReset2PB.Password = "";

                PasswordResetPB.Tag = "";
                PasswordReset2PB.Tag = "";

                CodeAccepted = false;

                RepeatCodePanelSP.Visibility = Visibility.Visible;
                RepeatCodePanelSP.IsEnabled = false;


                ErrorRegLB.Text = "";

                if (Height >= 650)
                {
                    DoubleAnimation ShkrinWindow = new DoubleAnimation();

                    ShkrinWindow.To = 475;
                    ShkrinWindow.Duration = time;

                    BeginAnimation(HeightProperty, ShkrinWindow);

                }


                ResetCodeSP.Visibility = Visibility.Collapsed;
                CodePanelSP.Visibility = Visibility.Visible;
                RegFormSP.Visibility = Visibility.Collapsed;
            }
        }

        private void ForgetPasswordTBl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UnderTBlClick_MouseLeftButtonUp(RegTBl, e);

            RegLogoLB.Content = Title = "Восстановление";


            RegBTN.IsEnabled = false;
        }



        private void PasswordPB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyEnglish();
        }


        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PasswordRegTB":
                    {
                        e.OnlyEnglish();
                    }
                    break;
                case "PhoneRegTB":
                    e.OnlyNumsTB();
                    break;
            }

        }



        private string CodeGenerator()
        {
            Random r = new Random();

            CodeForReg = "";
            for (int i = 0; i < 6; i++)
            {
                switch (r.Next(2))
                {

                    case 1:
                        CodeForReg += Convert.ToChar(r.Next(97, 123));
                        break;
                    default:
                        CodeForReg += r.Next(10).ToString();
                        break;
                }



            }
            Clipboard.SetText(CodeForReg);

            CodeTB.Focus();

            return CodeForReg;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PhoneRegTB":
                    if (string.IsNullOrWhiteSpace(selectedTB.Text))
                        selectedTB.Text += "+7 ";
                    break;

            }
        }

        private void DateOfBirthDP_LostFocus(object sender, RoutedEventArgs e)
        {

            if (DateOfBirthDP.SelectedDate == null)
            {
                DateOfBirthDP.Tag = "";
            }
        }



        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PhoneRegTB":
                    {
                        if (!selectedTB.Text.StartsWith("+7"))
                        {
                            selectedTB.Text = "+7 ";
                        }

                    }
                    break;

            }

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (MainGrid.Margin == new Thickness(0))
                {
                    if (SignInBTN.IsEnabled)
                    {
                        SignInBTN_Click(sender, e);
                        return;
                    }


                    if (LoginTB.IsFocused)
                    {
                        PasswordPB.Focus();
                    }
                    else
                    {
                        LoginTB.Focus();
                    }
                }

                else if (MainGrid.Margin == new Thickness(shrinkWinHeight, 0, 0, 0) && !CodeAccepted)
                {
                    if (RegBTN.IsEnabled)
                    {
                        RegBTN_Click(sender, e);
                        return;
                    }
                }

            }
        }










        private async Task ExpandWinAnim()
        {

            DoubleAnimation ShkrinWindow = new DoubleAnimation();

            var time = TimeSpan.FromSeconds(0.34);

            ShkrinWindow.To = 666;
            ShkrinWindow.Duration = time;


            BeginAnimation(HeightProperty, ShkrinWindow);


            await Task.Delay(time + TimeSpan.FromSeconds(0.10));
        }


        private async Task DisapearGetCode()
        {
            RepeatCodePanelSP.Visibility = Visibility.Hidden;


            DoubleAnimation OpaciyChange = new DoubleAnimation();

            var time = TimeSpan.FromSeconds(0.50);

            OpaciyChange.To = 0;
            OpaciyChange.Duration = time;


            CodePanelSP.BeginAnimation(OpacityProperty, OpaciyChange);

            await Task.Delay(time + TimeSpan.FromSeconds(0.10));


            time = TimeSpan.FromSeconds(0);
            OpaciyChange.To = 1;
            CodePanelSP.BeginAnimation(OpacityProperty, OpaciyChange);

            RepeatCodePanelSP.Opacity = 0.56;

            CodePanelSP.Visibility = Visibility.Collapsed;


        }


        private async Task ApperRegForm()
        {

            RegFormSP.Visibility = Visibility.Visible;

            DoubleAnimation OpaciyChange = new DoubleAnimation();

            var time = TimeSpan.FromSeconds(0.50);


            OpaciyChange.From = 0;
            OpaciyChange.To = 1;
            OpaciyChange.Duration = time;


            RegFormSP.BeginAnimation(OpacityProperty, OpaciyChange);

            await Task.Delay(time + TimeSpan.FromSeconds(0.10));

            RegBTN.IsEnabled = true;



        }





        private void showErrorMessage(TextBlock nameOfTB, string Message)
        {

            nameOfTB.Visibility = Visibility.Visible;
            nameOfTB.Text = Message;
        }

        private async void CloseWinBTN_Click(object sender, RoutedEventArgs e)
        {
            await AnimWinClose();
            Close();
        }

        private async Task AnimWinClose()
        {

            IsEnabled = false;

            await Task.Delay(500);

        }

    }
}

