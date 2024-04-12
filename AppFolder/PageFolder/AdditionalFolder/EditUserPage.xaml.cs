using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.GlobalClassFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder
{
    /// <summary>
    /// Логика взаимодействия для EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        private string selectedPhoto = "";
        private string savePhone = "";

        public EditUserPage()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;



        }

        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Client client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

                if (client.Photo != null)
                {
                    PhotoUserIB.ImageSource = PhotoImageClass.GetImageFromBytes(client.Photo);
                    selectedPhoto = "";
                }

                EmailTB.Text = client.Email;
                savePhone = PhoneTB.Text = string.Format("{0:+# (###) ###-##-##}", client.PhoneNum);
                NameTB.Text = client.Name;
                SurnameTB.Text = client.Surname;
                DateOfBirthDP.SelectedDate = client.DateOfBirth;
                if (client.GenderID == 1)
                {
                    ManRB.IsChecked = true;
                }
                else
                {
                    WomanRB.IsChecked = true;
                }
                Visibility = Visibility.Visible;
            }
            catch 
            {

                new MessageWin("Произошлка ошибка при загрузке данных", MessageCode.Error).ShowDialog();
                GlobalVarriabels.FrontFrame.FrameErrorBack();
            }
        }

        private async void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();

            DBEntities.NullContext();

            try
            {
                if (CheckFieldsBeforeReg()) return;

                EditBTN.IsEnabled = false;
                long Phone = PhoneTB.FormatPhoneForDB();


                Client newClient = DBEntities.GetContext().Client.FirstOrDefault(u => u.PhoneNum == Phone);

                if (newClient != null && savePhone != PhoneTB.Text)
                {
                    throw new Exception("Данный телефон уже привзан к другой записи.");
                }

                Client client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

                if (selectedPhoto != "")
                {
                    client.Photo = PhotoImageClass.SetImageToBytes(ref selectedPhoto);
                    GlobalVarriabels.MainWindow.PhotoUserIB.ImageSource = PhotoImageClass.GetImageFromBytes(client.Photo);
                }

                client.PhoneNum = PhoneTB.FormatPhoneForDB();
                client.Name = NameTB.Text;
                client.Surname = SurnameTB.Text;
                client.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.SelectedDate);

                if (ManRB.IsChecked == true)
                    client.GenderID = 1;
                else
                    client.GenderID = 2;

                if (!string.IsNullOrWhiteSpace(PasswordPB.Password))
                {
                    client.Password = string.Format("{0:x}", PasswordPB.Password.Trim().GetHashCode());

                    var LogStaff = DBEntities.GetContext().ClientEnterPC.Where(u=>u.ClientID == client.ClientID && u.NamePC != GlobalVarriabels.currentClientPC);

                    foreach (var item in LogStaff)
                    {
                        item.NamePC = null;
                    }

                }
                DBEntities.GetContext().SaveChanges();

                savePhone = PhoneTB.Text;

                GlobalVarriabels.MainWindow.setClientContext(client);


                await Task.Delay(100);

                GlobalVarriabels.MainWindow.DataContext = client;

                await SuccedEdit();
            }
            catch (Exception ex)
            {

                ErrorLB.Text = ex.Message;
            }
            finally
            {
                EditBTN.IsEnabled = true;
            }

        }

        private bool CheckFieldsBeforeReg()
        {
            string messageError;
            bool gotError = false;

            PasswordPB.Tag = null;

            if (!string.IsNullOrWhiteSpace(PasswordPB.Password) && !PasswordPB.CheckPassword(out messageError))
            {
                PasswordPB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = messageError;
                    gotError = true;
                }
            }

            if (string.IsNullOrWhiteSpace(PhoneTB.Text))
            {
                PhoneTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = "Введите телефон";

                    gotError = true;
                }
            }
            else if (!PhoneTB.CheckLengthPhone())
            {
                PhoneTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = "Телефон не полностью введён";

                    gotError = true;
                }
            }

            if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                NameTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = "Введите имя";

                    gotError = true;
                }
            }
            if (string.IsNullOrWhiteSpace(SurnameTB.Text))
            {
                SurnameTB.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = "Введите фамилию";
                    gotError = true;
                }


            }
            if (DateOfBirthDP.SelectedDate == null)
            {
                DateOfBirthDP.Tag = GlobalVarriabels.ErrorTag;

                if (!gotError)
                {
                    ErrorLB.Text = "Выберите дату рождения";

                    gotError = true;
                }
            }


            return gotError;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            selectedTB.Tag = null;
            ErrorLB.Text = "";

            try
            {
                switch (selectedTB.Name)
                {

                    case "PhoneTB":
                        {
                            if (!selectedTB.Text.StartsWith("+7"))
                            {
                                //ErrorRegLB.Visibility = Visibility.Hidden;
                                ErrorLB.Text = "Неверный формат телефона";

                                selectedTB.Tag = GlobalVarriabels.ErrorTag;
                                return;
                            }


                            selectedTB.Phone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PhoneTB":
                    if (string.IsNullOrWhiteSpace(selectedTB.Text))
                        selectedTB.Text += "+7 ";
                    break;

            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var selectedTB = sender as TextBox;

            selectedTB.Text = selectedTB.Text.Trim();

            switch (selectedTB.Name)
            {
                case "PhoneTB":
                    {
                        if (!selectedTB.Text.StartsWith("+7"))
                        {
                            selectedTB.Text = "+7 ";
                        }
                    }
                    break;

            }
        }


        private async Task SuccedEdit()
        {
            ErrorLB.Text = "Данные изменены";
            ErrorLB.Foreground = new SolidColorBrush(Color.FromRgb(49, 163, 54));

            await Task.Delay(1500);

            ErrorLB.Text = "";
            ErrorLB.Foreground = new SolidColorBrush(Color.FromRgb(233, 0, 0));

        }

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PhoneTB":
                    e.OnlyNumsTB();
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

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PhotoImageClass.AddPhoto(PhotoUserIB, ref selectedPhoto);

        }

        private async void BorderPhoto_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;


            selectedPhoto = "";

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            foreach (var item in files)
            {
                selectedPhoto += item;
            }

            if (selectedPhoto.EndsWith(".png") || selectedPhoto.EndsWith(".jpeg") || selectedPhoto.EndsWith(".jpg"))
            {

                await Task.Delay(500);
                PhotoUserIB.ImageSource = PhotoImageClass.GetImageFromBytes(PhotoImageClass.SetImageToBytes(ref selectedPhoto));
                return;
            }

            ErrorLB.Text = "Файл должен быть: '.png','.jpeg','jpg'";
        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
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




            if (!PasswordPB.CheckPassword(out string messageError))
            {
                ErrorLB.Text = messageError;
                PasswordPB.Tag = GlobalVarriabels.ErrorTag;
            }
            else
            {
                ErrorLB.Text = "";
                PasswordPB.Tag = null;
            }
        }


    private void PasswordPB_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.OnlyEnglish();
    }
}
}
