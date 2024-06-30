using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.GlobalClassFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using System;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GoncharovVympelSale.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWin.xaml
    /// </summary>
    public partial class AuthorizationWin : Window
    {

        private double shrinkWinHeight = -325;
        private string EmailClient = "";

        TimeSpan EndOfBlck;

        DispatcherTimer timer;

        string CodeForReg = "";
        bool blockGetCodeBtn = false;

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

            selectedTB.Tag = null;

            try
            {
                switch (selectedTB.Name)
                {
                    case "LoginTB":
                        EnableAuhtButton();
                        break;
                    case "EmailTB":
                        {

                            EmailTB.Text = EmailTB.Text.Trim();
                            EmailTB.Text = EmailTB.Text.Replace(" ", "");

                            if (string.IsNullOrWhiteSpace(EmailTB.Text))
                            {
                                RegBTN.IsEnabled = false;
                                return;
                            }


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

                                ErrorRegTBl.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                showErrorMessage(ErrorRegTBl, "Неверный формат почты");

                                RegBTN.IsEnabled = CodeTB.IsEnabled = isEmail;

                            }


                        }

                        break;
                    case "CodeTB":
                        {
                            ErrorRegTBl.Visibility = Visibility.Hidden;

                            if (!(selectedTB.Text.Length > 5)) return;

                            Keyboard.ClearFocus();

                            if (RegLogoLB.Content.ToString() == "Восстановление")
                            {
                                var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == EmailClient);

                                if (client == null)
                                    throw new Exception("Почта не существует или удалена.");

                                if (client.AuthCode == selectedTB.Text)
                                {

                                    timer.Stop();

                                    Thread.Sleep(300);

                                    await DisapearGetCode();

                                    blockGetCodeBtn = false;
                                    RegBTN.IsEnabled = true;

                                    RegBTN.Content = "Восстановить";

                                    ResetCodeSP.Visibility = Visibility.Visible;

                                    CodeAccepted = true;


                                    return;
                                }
                            }
                            else
                            {
                                if (selectedTB.Text == CodeForReg)
                                {

                                    timer.Stop();

                                    Thread.Sleep(300);

                                    await DisapearGetCode();

                                    blockGetCodeBtn = false;
                                    RegBTN.IsEnabled = true;

                                    RegBTN.Content = "Зарегистрироваться";
                                    CodeAccepted = true;

                                    await ExpandWinAnim();
                                    await AppearRegForm();
                                    return;
                                }

                            }


                            selectedTB.Text = "";

                            showErrorMessage(ErrorRegTBl, "Неверный код");

                            selectedTB.Tag = GlobalVarriabels.ErrorTag;

                        }


                        break;
                    case "PhoneRegTB":
                        {
                            if (selectedTB.Text == "7")
                            {
                                selectedTB.Text = selectedTB.Text.Insert(0, "+");
                                selectedTB.Text += " ";
                                selectedTB.CaretIndex = 3;
                            }

                            if (!selectedTB.Text.StartsWith("+7"))
                            {
                                //ErrorRegLB.Visibility = Visibility.Hidden;
                                showErrorMessage(ErrorRegTBl, "Неверный формат телефона");

                                selectedTB.Tag = GlobalVarriabels.ErrorTag;
                                return;
                            }

                            ErrorRegTBl.Visibility = Visibility.Hidden;

                            selectedTB.Phone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegTBl, ex.Message);
            }

        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordBox password = sender as PasswordBox;

                Label hintTextBlock = password.Template.FindName("HintLB", password) as Label;
                TextBlock textBlock = password.Template.FindName("TextPassword", password) as TextBlock;
                ToggleButton toggleButton = password.Template.FindName("ShowPasswordBTN", password) as ToggleButton;

                bool isShowPassword = PropertyClass.GetShowPassword(password);



                textBlock.Text = password.Password;

                hintTextBlock.Visibility = password.Password.Length < 1 ? Visibility.Visible : Visibility.Collapsed;

                if (isShowPassword)
                    toggleButton.Visibility = password.Password.Length < 1 ? Visibility.Collapsed : Visibility.Visible;

                switch (password.Name)
                {
                    case "PasswordPB":
                        {
                            EnableAuhtButton();
                        }
                        break;
                    case "PasswordRegPB":
                        {
                            string messageError;

                            if (!PasswordRegPB.CheckPassword(out messageError))
                            {
                                PasswordRegPB.Tag = GlobalVarriabels.ErrorTag;

                                showErrorMessage(ErrorRegTBl, messageError);
                                return;
                            }

                            ErrorRegTBl.Visibility = Visibility.Hidden;
                        }
                        break;
                    case "PasswordResetPB":
                    case "PasswordReset2PB":
                        {
                            ErrorRegTBl.Visibility = Visibility.Hidden;
                        }
                        break;
                }
            }
            catch { }


        }


        private void remeberMe(Client client)
        {
            if (RemeberChB.IsChecked == false) return;

            string userPcName = string.Format("{0:x}", (Environment.MachineName + Environment.UserName).GetHashCode());
            var getPC = DBEntities.GetContext().ClientEnterPC.FirstOrDefault(u => u.ClientID == client.ClientID && u.NamePC == null);

            if (getPC == null)
            {
                ClientEnterPC newPC = new ClientEnterPC();

                newPC.ClientID = client.ClientID;
                newPC.NamePC = userPcName;

                DBEntities.GetContext().ClientEnterPC.Add(newPC);
            }
            else
            {
                getPC.NamePC = userPcName;
            }


            DBEntities.GetContext().SaveChanges();

            GlobalVarriabels.currentClientPC = userPcName;
        }


        private async void SignInBTN_Click(object sender, RoutedEventArgs e)
        {
            GlobalVarriabels.isDepWorker = false;
            GlobalVarriabels.isReadOnly = false;

            Keyboard.ClearFocus();

            DBEntities.NullContext();

            SignInBTN.IsEnabled = false;
            ErrorTBl.Visibility = Visibility.Hidden;

            User user = null;
            Staff staff = null;
            Client client = null;

            string password = string.Format("{0:x}", PasswordPB.Password.Trim().GetHashCode());
            string login = LoginTB.Text.Trim();

            try
            {
                if (login.StartsWith("+7") | login.StartsWith("7"))
                {
                    long phone = LoginTB.FormatPhoneForDB();

                    client = DBEntities.GetContext().Client.FirstOrDefault(u => u.PhoneNum == phone);

                    if (client == null || client.Password != password)
                    {
                        showErrorMessage(ErrorTBl, "Неверный телефон или пароль");
                        SystemSounds.Exclamation.Play();
                        return;
                    }

                    remeberMe(client);

                    GlobalVarriabels.currentUserID = client.ClientID;
                    GlobalVarriabels.currentRoleName = GlobalVarriabels.RoleName.Client;
                    //GlobalVarriabels.curDepCompanyID = Convert.ToInt32(client.SelectedDepID);



                }
                else if (LoginTB.isEmail())
                {
                    password = string.Format("{0:x}", PasswordPB.Password.GetHashCode());

                    client = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == LoginTB.Text.Trim());

                    if (client == null || client.Password != password)
                    {
                        showErrorMessage(ErrorTBl, "Неверная почта или пароль");
                        SystemSounds.Exclamation.Play();
                        return;
                    }

                    remeberMe(client);

                    GlobalVarriabels.currentUserID = client.ClientID;
                    GlobalVarriabels.currentRoleName = GlobalVarriabels.RoleName.Client;
                    //GlobalVarriabels.curDepCompanyID = Convert.ToInt32(client.SelectedDepID);
                }
                else
                {

                    user = DBEntities.GetContext().User.FirstOrDefault(u => u.Login == LoginTB.Text.Trim());

                    if (user == null || user.Passsword != PasswordPB.Password)
                    {
                        showErrorMessage(ErrorTBl, "Неверный логин или пароль");
                        SystemSounds.Exclamation.Play();
                        return;
                    }

                    staff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.UserID == user.UserID);

                    if (staff?.StatusID == 3)
                    {
                        showErrorMessage(ErrorTBl, "Доступ запрещён");
                        return;
                    }



                    GlobalVarriabels.currentUserID = staff.StaffID;
                    GlobalVarriabels.curDepCompanyID = staff.DepartamentID;
                    GlobalVarriabels.currentRoleName = (GlobalVarriabels.RoleName)user.RoleID;

                    GlobalVarriabels.isReadOnly = GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.MainManager || GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepManager;

                    GlobalVarriabels.isDepWorker = !(GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.MainAdmin || GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.MainManager);



                }



                await AnimWinClose();
                new MainWin(staff, client).Show();
                Close();

            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorTBl, ex.Message);
            }
            finally
            {
                SignInBTN.IsEnabled = true;
            }
        }

        private void RegBTN_Click(object sender, RoutedEventArgs e)
        {

            DBEntities.NullContext();

            ErrorRegTBl.Visibility = Visibility.Hidden;

            Keyboard.ClearFocus();


            if (RegLogoLB.Content.ToString() == "Восстановление")
            {
                if (CodeAccepted)
                    ResettingPassword();
                else
                    ResetPasswordGetCode();

                return;
            }


            if (CodeAccepted)
                Registraition();
            else
                RegistraitionGetCode();


        }



        private void CreateTimeAndGenerateCode()
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


        }


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



        private bool CheckFieldsBeforeReg()
        {
            string messageError;
            bool gotError = false;

            if (string.IsNullOrWhiteSpace(PasswordRegPB.Password))
            {
                PasswordRegPB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Введите пароль");
                    gotError = true;
                }
            }
            else if (!PasswordRegPB.CheckPassword(out messageError))
            {
                PasswordRegPB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, messageError);
                    gotError = true;

                }
            }

            if (string.IsNullOrWhiteSpace(PhoneRegTB.Text))
            {
                PhoneRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Введите телефон");

                    gotError = true;
                }
            }
            else if (!PhoneRegTB.CheckLengthPhone())
            {
                PhoneRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Телефон не полностью введён");

                    gotError = true;
                }
            }

            if (string.IsNullOrWhiteSpace(NameRegTB.Text))
            {
                NameRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Введите имя");

                    gotError = true;
                }
            }
            if (string.IsNullOrWhiteSpace(SurnameRegTB.Text))
            {
                SurnameRegTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Введите фамилию");
                    gotError = true;
                }


            }
            if (DateOfBirthDP.SelectedDate == null)
            {
                DateOfBirthDP.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Выберите дату рождения");

                    gotError = true;
                }
            }
            if (ManChB.IsChecked == false && WomanChB.IsChecked == false)
            {
                if (!gotError)
                {
                    showErrorMessage(ErrorRegTBl, "Выберите пол");
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
                if (CheckFieldsBeforeReg()) return;

                long Phone = PhoneRegTB.FormatPhoneForDB();


                Client newClient = DBEntities.GetContext().Client.FirstOrDefault(u => u.PhoneNum == Phone);

                if (newClient != null)
                {
                    showErrorMessage(ErrorRegTBl, "Данный телефон уже зарегистрирован");
                    return;
                }

                newClient = new Client();

                newClient.Email = EmailTB.Text.Trim();
                newClient.PhoneNum = PhoneRegTB.FormatPhoneForDB();
                newClient.Password = string.Format("{0:x}", PasswordRegPB.Password.GetHashCode());
                newClient.Name = NameRegTB.Text.Trim();
                newClient.Surname = SurnameRegTB.Text.Trim();
                newClient.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.SelectedDate);
                newClient.GenderID = ManChB.IsChecked == true ? 1 : 2;

                DBEntities.GetContext().Client.Add(newClient);


                string userPcName = string.Format("{0:x}", (Environment.MachineName + Environment.UserName).GetHashCode());

                var checkPC = DBEntities.GetContext().ClientEnterPC.FirstOrDefault(u => u.NamePC == userPcName);

                if (checkPC != null)
                    checkPC.NamePC = null;

                ClientEnterPC newPC = new ClientEnterPC();


                newPC.ClientID = newClient.ClientID;
                newPC.NamePC = userPcName;

                DBEntities.GetContext().ClientEnterPC.Add(newPC);
                DBEntities.GetContext().SaveChanges();

                string message = $"<h2>Вы успешно зарегистрировали учётную запись в \"Вымпел продажи\"! </h2>" + $"{CodeGenerator()}";
                EmailClass.sendMessage(EmailClient, "Успешная регистрация", message);


                GlobalVarriabels.currentUserID = newClient.ClientID;
                GlobalVarriabels.currentClientPC = userPcName;
                GlobalVarriabels.currentRoleName = GlobalVarriabels.RoleName.Client;
                GlobalVarriabels.isDepWorker = false;

                await BorderSucced(true);

                await AnimWinClose();

                new MainWin(null, newClient).Show();

                Close();

            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegTBl, ex.Message);
            }


        }


        private void RegistraitionGetCode()
        {

            try
            {
                Client newClient = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == EmailTB.Text.Trim());

                if (newClient != null)
                {
                    showErrorMessage(ErrorRegTBl, "Данная почта уже зарегистрирована");

                    return;
                }

                EmailClient = EmailTB.Text.Trim();

                string message = $"<h2>Код для регистрации </h2>" +
                    $"{CodeGenerator()}";

                EmailClass.sendMessage(EmailClient, "Регистрация", message);


                CreateTimeAndGenerateCode();

                CodeTB.Focus();

            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegTBl, ex.Message);
            }


        }


        private async void ResettingPassword()
        {
            try
            {



                if (!PasswordResetPB.CheckPassword(out string messageError))
                {
                    PasswordRegPB.Tag = GlobalVarriabels.ErrorTag;
                    PasswordRegPB.Tag = GlobalVarriabels.ErrorTag;

                    showErrorMessage(ErrorRegTBl, messageError);
                    return;
                }
                else if (PasswordResetPB.Password != PasswordReset2PB.Password)
                {
                    PasswordResetPB.Tag = GlobalVarriabels.ErrorTag;
                    PasswordReset2PB.Tag = GlobalVarriabels.ErrorTag;
                    showErrorMessage(ErrorRegTBl, "Пароли не совпадают.");

                    return;
                }

                var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == EmailClient);
                var setPC = DBEntities.GetContext().ClientEnterPC.Where(u => u.ClientID == client.ClientID);

                client.Password = string.Format("{0:x}", PasswordResetPB.Password.GetHashCode());

                foreach (var item in setPC)
                {
                    item.NamePC = null;
                }


                DBEntities.GetContext().SaveChanges();

                Thread.Sleep(300);

                await BorderSucced(false);



            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegTBl, ex.Message);
            }

        }

        private void ResetPasswordGetCode()
        {
            try
            {

                EmailClient = EmailTB.Text.Trim();

                var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.Email == EmailClient);

                if (client == null)
                    throw new Exception("Такая почта не зарегистрирована.");

                client.AuthCode = CodeGenerator();

                DBEntities.GetContext().SaveChanges();


                string message = $"<h2>Код для восстановление пароля</h2>" +
                $"{client.AuthCode}";

                EmailClass.sendMessage(EmailClient, "Восстановление пароля", message);

                CreateTimeAndGenerateCode();


                CodeTB.Focus();

            }
            catch (Exception ex)
            {
                showErrorMessage(ErrorRegTBl, ex.Message);
            }

        }




        TimeSpan blockTranz;

        private async void UnderTBlClick_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (blockTranz != null && blockTranz > DateTime.Now.TimeOfDay) return;
            blockTranz = DateTime.Now.AddSeconds(0.5).TimeOfDay;

            Keyboard.ClearFocus();

            TextBlock SelectedTBl = sender as TextBlock;

            Keyboard.ClearFocus();


            ThicknessAnimation gridTransit = new ThicknessAnimation();
            PowerEase elasticEase = new PowerEase();

            gridTransit.EasingFunction = elasticEase;

            elasticEase.Power = 2.6;

            var time = TimeSpan.FromSeconds(0.4);


            if (SelectedTBl.Name == "RegTBl")
            {
                AuthorizationSP.IsEnabled = false;
                RegistrationSP.IsEnabled = true;


                gridTransit.To = new Thickness(shrinkWinHeight, 0, 0, 0);
                gridTransit.Duration = time;

                RegLogoLB.Content = Title = "Регистрация";


            }
            else
            {

                AuthorizationSP.IsEnabled = true;
                RegistrationSP.IsEnabled = false;

                gridTransit.To = new Thickness(0);
                gridTransit.Duration = time;


                timer?.Stop();



                await Task.Delay(125);

                Keyboard.ClearFocus();


                if (Height >= MaxHeight)
                {
                    DoubleAnimation ShkrinWindow = new DoubleAnimation();

                    ShkrinWindow.To = MinHeight;
                    ShkrinWindow.Duration = time;

                    BeginAnimation(HeightProperty, ShkrinWindow);

                }


                ResetCodeSP.Visibility = Visibility.Collapsed;
                CodePanelSP.Visibility = Visibility.Visible;
                RegFormSP.Visibility = Visibility.Collapsed;

                Title = "Авторизация";

                blockGetCodeBtn = false;
                RegBTN.IsEnabled = true;


                RegBTN.Content = "Получить код";
                RegBTN.IsEnabled = false;


                //------ресет кода получения---------------

                EmailTB.Text = "";
                CodeTB.Text = "";
                CodeTB.IsEnabled = false;

                //------ресет регистраци текска---------------

                PasswordRegPB.Password = "";
                PhoneRegTB.Text = "";
                NameRegTB.Text = "";
                SurnameRegTB.Text = "";
                DateOfBirthDP.SelectedDate = null;
                WomanChB.IsChecked = false;
                ManChB.IsChecked = false;


                //------Ресет регистрации тегов---------------

                PasswordRegPB.Tag = null;
                EmailTB.Tag = null; ;
                CodeTB.Tag = null;
                PhoneRegTB.Tag = null;
                NameRegTB.Tag = null;
                SurnameRegTB.Tag = null;
                DateOfBirthDP.Tag = null;

                //------Ресет восстановаление пароля тег---------------

                PasswordResetPB.Password = "";
                PasswordReset2PB.Password = "";

                PasswordResetPB.Tag = "";
                PasswordReset2PB.Tag = "";

                CodeAccepted = false;

                RepeatCodePanelSP.Visibility = Visibility.Visible;
                RepeatCodePanelSP.IsEnabled = false;
                RepeatCodeTimerTBl.Text = "1:00";

                ErrorRegTBl.Text = "";


            }


            gridTransit.EasingFunction = elasticEase;

            MainGrid.BeginAnimation(Grid.MarginProperty, gridTransit);


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
                case "PhoneRegTB":
                    e.OnlyNumsTB();
                    break;
                case "EmailTB":
                    e.OnlyEnglish();
                    break;
            }

        }



        private string CodeGenerator()
        {
            Random r = new Random();

            CodeForReg = "";
            for (int i = 0; i < 6; i++)
            {
                if (r.Next(2) == 1)
                {
                    CodeForReg += Convert.ToChar(r.Next(97, 123));
                }
                else
                {
                    CodeForReg += r.Next(10).ToString();
                }


            }

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
                DateOfBirthDP.Tag = null;
            }
        }



        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            selectedTB.Text = selectedTB.Text.Trim();

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

            var time = TimeSpan.FromSeconds(0.28);

            ShkrinWindow.To = MaxHeight;
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


        private async Task AppearRegForm()
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


        private async Task BorderSucced(bool IsRegistation)
        {

            if (IsRegistation)
                SuccsesTextTBL.Text = "Вы успешно зарегистрировались!";
            else
                SuccsesTextTBL.Text = "Пароль успешно восстановлен!";

            SuccseedBorder.Visibility = Visibility.Visible;

            DoubleAnimation OpaciyChange = new DoubleAnimation();

            var time = TimeSpan.FromSeconds(0.50);


            OpaciyChange.From = 0;
            OpaciyChange.To = 1;
            OpaciyChange.Duration = time;


            SuccseedBorder.BeginAnimation(OpacityProperty, OpaciyChange);

            if (!IsRegistation)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                UnderTBlClick_MouseLeftButtonUp(BackAuthTBl, null);
            }

            await Task.Delay(TimeSpan.FromSeconds(2.5));

            if (IsRegistation) return;


            OpaciyChange.From = 1;
            OpaciyChange.To = 0;
            OpaciyChange.Duration = time;

            SuccseedBorder.BeginAnimation(OpacityProperty, OpaciyChange);

            await Task.Delay(time + TimeSpan.FromSeconds(0.10));

            SuccseedBorder.Visibility = Visibility.Hidden;

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

        private void DateOfBirthDP_GotFocus(object sender, RoutedEventArgs e)
        {
            DateOfBirthDP.Tag = null;
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {


                string userPcName = string.Format("{0:x}", (Environment.MachineName + Environment.UserName).GetHashCode());


                var setPC = DBEntities.GetContext().ClientEnterPC.FirstOrDefault(u => u.NamePC == userPcName);

                if (setPC == null) return;


                var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == setPC.ClientID);

                GlobalVarriabels.currentUserID = client.ClientID;
                GlobalVarriabels.currentRoleName = GlobalVarriabels.RoleName.Client;
                GlobalVarriabels.curDepCompanyID = Convert.ToInt32(client.SelectedDepID);
                GlobalVarriabels.currentClientPC = userPcName;

                await AnimWinClose();
                new MainWin(null, client).Show();
                Close();

            }
            catch
            {
                showErrorMessage(ErrorTBl, "Не удалось загрузить данные для входа.");
            }
        }
    }


}

