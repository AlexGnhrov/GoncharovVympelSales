using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.GlobalClassFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.StaffFolder
{
    /// <summary>
    /// Логика взаимодействия для AEStaffPage.xaml
    /// </summary>
    public partial class AEStaffPage : Page
    {

        StaffListPage staffListPage;
        Staff editStaff;

        double ValueGridTransit = -590;

        int StaffID;


        string saveLogin = "";
        string savePhone = "";
        string selectedPhoto = "";

        string saveSeriesPas;
        string saveNumPass;
        int saveStatusID;

        bool gotPhoto = false;



        public AEStaffPage(StaffListPage staffListPage, int StaffID)
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;


            this.staffListPage = staffListPage;
            this.StaffID = StaffID;

            ButtonDepoIssueSP.Width = 0;
            ButtonRegionSP.Width = 0;
            ButtonCitySP.Width = 0;
            ButtonStreetSP.Width = 0;

            ButtonDepoIssueSP.IsEnabled = false;
            ButtonRegionSP.IsEnabled = false;
            ButtonCitySP.IsEnabled = false;
            ButtonStreetSP.IsEnabled = false;


            if (StaffID > 0)
            {
                NamePageLB.Content = "Редактирование сотрудника";
                AddEditBTN.Content = "Редактировать";

                if(StaffID == 1)
                {
                    RoleCB.IsEnabled = false;
                    StatusCB.IsEnabled = false;
                }
            }

            if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepAdmin)
            {
                DeportamentCB.Visibility = Visibility.Collapsed;
            }

        }

        private void LoadCB()
        {
            DepoIssueCB.ItemsSource = DBEntities.GetContext().DepartamentIssue.OrderBy(u => u.DepartamentID).ToList();
            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();


            if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepAdmin)
                RoleCB.ItemsSource = DBEntities.GetContext().Role.Where(u => u.RoleID != 1 && u.RoleID != 2).OrderBy(u => u.RoleID).ToList();
            else
                RoleCB.ItemsSource = DBEntities.GetContext().Role.OrderBy(u => u.RoleID).ToList();

            DeportamentCB.ItemsSource = DBEntities.GetContext().DepartamentCompany.ToList();


            StatusCB.ItemsSource = StaffID > 0 ? DBEntities.GetContext().StatusUser.ToList() : DBEntities.GetContext().StatusUser.Where(u => u.StatusUserID != 3).ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DBEntities.NullContext();

                LoadCB();

                StatusCB.SelectedValue = 1;

                if (StaffID > 0)
                {
                    editStaff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.StaffID == StaffID);


                    if (editStaff == null)
                    {
                        throw new Exception("Отсутствует подключение или отдел был удалён.");
                    }

                    selectedPhoto = "Есть фото";

                    ImagePhoto.ImageSource = PhotoImageClass.GetImageFromBytes(editStaff.Photo);

                    SNPstaffTB.Text = $"{editStaff.Surname} {editStaff.Name}";
                    SNPstaffTB.Text += editStaff.Patronymic != null ? $" {editStaff.Patronymic}" : null;

                    LoginTB.Text = saveLogin = editStaff.User.Login;
                    PasswordPB.Password = editStaff.User.Passsword;
                    PhoneTB.Text = savePhone = editStaff.PhoneNum;
                    EmailTB.Text = editStaff.Email;

                    RoleCB.SelectedValue = Convert.ToInt32(editStaff.User.RoleID);
                    DeportamentCB.SelectedValue = Convert.ToInt32(editStaff.DepartamentID);
                    StatusCB.SelectedValue = saveStatusID = Convert.ToInt32(editStaff.StatusID);


                    DepoIssueCB.Text = editStaff.Passport.DepartamentIssue.NameDep;

                    PassportTB.Text = editStaff.Passport.Series + " " + editStaff.Passport.Num;

                    saveSeriesPas = editStaff.Passport.Series;
                    saveNumPass = editStaff.Passport.Num;

                    DateOfIssueDP.SelectedDate = Convert.ToDateTime(editStaff.Passport.DateOfIssue);
                    DateOfBirthDP.SelectedDate = Convert.ToDateTime(editStaff.Passport.DateOfBirth);

                    if (editStaff.Passport.GenderID == 1)
                    {
                        ManRB.IsChecked = true;
                    }
                    else
                    {
                        WomanRB.IsChecked = true;
                    }


                    RegionCB.Text = editStaff.Passport.Adress.Region.NameRegion;
                    CityCB.Text = editStaff.Passport.Adress.City.NameCity;
                    StreetCB.Text = editStaff.Passport.Adress.Street.NameStreet;
                    HomeTB.Text = editStaff.Passport.Adress.Home.ToString();
                    BuildingTB.Text = editStaff.Passport.Adress.Building == null ? null : editStaff.Passport.Adress.Building;
                    AppartamentTB.Text = editStaff.Passport.Adress.Appartament.ToString();

                }



            }
            catch (Exception ex)
            {
                if (StaffID > 0)
                {
                    new MessageWin("Ошибка выгрузки данных",
                                   "Отсутствует подключение к интернету",
                                   MessageCode.Error).ShowDialog();
                }
                else
                {
                    new MessageWin("Ошибка выгрузки данных", ex,
                                   MessageCode.Error).ShowDialog();
                }



                GlobalVarriabels.FrontFrame.FrameErrorBack();
                staffListPage.UpdateList();

                return;
            }


            Visibility = Visibility.Visible;
        }

        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }

        TimeSpan blockTranz;

        bool isRightActive = false;

        private void ArrowBTN_Click(object sender, RoutedEventArgs e)
        {

            if (blockTranz != null && blockTranz > DateTime.Now.TimeOfDay) return;
            blockTranz = DateTime.Now.AddSeconds(0.5).TimeOfDay;


            Keyboard.ClearFocus();

            ThicknessAnimation gridTransit = new ThicknessAnimation();
            PowerEase elasticEase = new PowerEase();

            elasticEase.Power = 2;
            elasticEase.EasingMode = EasingMode.EaseOut;

            var time = TimeSpan.FromSeconds(0.4);
            gridTransit.Duration = time;
            gridTransit.EasingFunction = elasticEase;


            if (!isRightActive)
            {
                gridTransit.To = new Thickness(ValueGridTransit, 0, 0, 0);
                isRightActive = true;


                RightGrid.IsEnabled = true;
                LeftGrid.IsEnabled = false;
            }
            else
            {
                LeftGrid.IsEnabled = true;
                RightGrid.IsEnabled = false;

                gridTransit.To = new Thickness(0);

                isRightActive = false;
            }


            MainGrid.BeginAnimation(Grid.MarginProperty, gridTransit);


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLB.Text = "";

            TextBox textBox = sender as TextBox;



            switch (textBox.Name)
            {
                case "PhoneTB":
                    textBox.Phone();
                    break;

                case "SNPstaffTB":

                    string[] SplitSNP = textBox.Text.Split(' ');

                    int strlen = textBox.Text.Length;

                    if (SplitSNP.Length > 3 && SplitSNP[2] == "")
                    {
                        textBox.Text = textBox.Text.Remove(strlen - 1);
                        textBox.CaretIndex = strlen;
                    }
                    else if (SplitSNP.Length > 2 && SplitSNP[1] == "")
                    {
                        textBox.Text = textBox.Text.Remove(strlen - 1);
                        textBox.CaretIndex = strlen;
                    }
                    break;
                case "PassportTB":
                    textBox.PassportNum();
                    break;
                case "CodeDepTB":
                    textBox.CodeDepNum();
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

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            (sender as TextBox).Tag = null;

            var selectedTB = sender as TextBox;

            switch (selectedTB.Name)
            {
                case "PhoneTB":
                case "HomeTB":
                case "PassportTB":
                    e.OnlyNumsTB();
                    break;

                case "LoginTB":
                    e.Login();
                    break;

                case "EmailTB":
                    e.OnlyEnglish();
                    break;

            }

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            var selectedTB = sender as TextBox;

            selectedTB.Tag = null;

            switch (selectedTB.Name)
            {
                case "PhoneTB":
                    if (string.IsNullOrWhiteSpace(selectedTB.Text))
                        selectedTB.Text += "+7 ";
                    break;

            }

        }

        private void BorderPhoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PhotoImageClass.AddPhoto(ImagePhoto, ref selectedPhoto);

            if (selectedPhoto != "" || selectedPhoto != null)
            {
                ErrorLB.Text = "";
                BorderPhoto.Tag = null;
            }

        }


        string saveEditRegion;
        string saveEditCity;
        string saveEditStreet;

        private void EditItemComboBoxBTN_Click(object sender, RoutedEventArgs e)
        {
            var selectedBTN = sender as Button;

            Keyboard.ClearFocus();
            DBEntities.NullContext();



            MessageWin messageWin;

            string captionError = "";

            string nameElement;

            try
            {
                switch (selectedBTN.Name)
                {

                    case "EditDepBTN":
                        {

                            captionError = "Ошибка редактирования департамента";


                            if (!isEdit)
                            {
                                EnableAllForEditElements(isEdit, 4, EditDepoIssueTB, ImageEDep, ImageDDep);
                                EditDepoIssueTB.Text = DepoIssueCB.Text;
                                EditCodeDepTB.Text = CodeDepTB.Text;

                                return;
                            }

                            nameElement = EditDepoIssueTB.Text.Trim();
                            string nameCodeDep = EditCodeDepTB.Text.Trim();

                            if (DepoIssueCB.Text == nameElement && nameCodeDep == CodeDepTB.Text)
                            {
                                EnableAllForEditElements(isEdit, 4, EditDepoIssueTB, ImageEDep, ImageDDep);
                                return;
                            }

                            if (EditCodeDepTB.Text.Length < 7)
                            {
                                EditCodeDepTB.Tag = GlobalVarriabels.ErrorTag;
                                SystemSounds.Exclamation.Play();
                                return;
                            }

                            var checkDepo = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.NameDep == nameElement);
                            var checkCode = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.CodeDep == nameCodeDep);

                            if (CodeDepTB.Text != nameCodeDep && checkCode != null)
                                throw new Exception($"Данный код уже принадлежить отделу:\n\"{checkCode.NameDep}\"");

                            if (DepoIssueCB.Text != nameElement && checkDepo != null)
                                throw new Exception("Данный депортамент уже существует.");


                            messageWin = new MessageWin("Вы действительно хотите изменить текущий департамент?",
                                                        MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;

                            var editDepoIssue = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.NameDep == DepoIssueCB.Text);

                            editDepoIssue.NameDep = nameElement;
                            editDepoIssue.CodeDep = nameCodeDep;



                            DBEntities.GetContext().SaveChanges();



                            EnableAllForEditElements(isEdit, 4, EditDepoIssueTB, ImageEDep, ImageDDep);



                            DepoIssueCB.ItemsSource = null;
                            DepoIssueCB.ItemsSource = DBEntities.GetContext().DepartamentIssue.OrderBy(u => u.DepartamentID).ToList();

                            DepoIssueCB.Text = nameElement;





                        }
                        break;
                    case "EditRegionBTN":
                        {

                            captionError = "Ошибка редактирования региона";


                            if (!isEdit)
                            {

                                saveEditCity = CityCB.Text;
                                saveEditStreet = StreetCB.Text;

                                EnableAllForEditElements(isEdit, 1, EditRegionTB, ImageERegion, ImageDRegion);
                                EditRegionTB.Text = RegionCB.Text;

                                return;
                            }

                            nameElement = EditRegionTB.Text.Trim();

                            if (RegionCB.Text == nameElement)
                            {
                                EnableAllForEditElements(isEdit, 1, EditRegionTB, ImageERegion, ImageDRegion);
                                return;
                            }



                            var checkRegion = DBEntities.GetContext().Region.FirstOrDefault(u => u.NameRegion == nameElement);
                            var editableRegion = DBEntities.GetContext().Region.FirstOrDefault(u => u.NameRegion == RegionCB.Text);

                            if (checkRegion != null)
                                throw new Exception("Данный регион уже существует.");


                            messageWin = new MessageWin("Вы действительно хотите изменить текущее название региона?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;

                            editableRegion.NameRegion = nameElement;


                            DBEntities.GetContext().SaveChanges();



                            EnableAllForEditElements(isEdit, 1, EditRegionTB, ImageERegion, ImageDRegion);


                            RegionCB.ItemsSource = null;
                            CityCB.ItemsSource = null;
                            StreetCB.ItemsSource = null;

                            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

                            RegionCB.Text = nameElement;
                            CityCB.Text = saveEditCity;
                            StreetCB.Text = saveEditStreet;




                        }
                        break;
                    case "EditCityBTN":
                        {
                            captionError = "Ошибка редактирование города";

                            if (!isEdit)
                            {
                                saveEditRegion = RegionCB.Text;
                                saveEditStreet = StreetCB.Text;


                                EnableAllForEditElements(isEdit, 2, EditCityTB, ImageECity, ImageDCity);
                                EditCityTB.Text = CityCB.Text;

                                return;
                            }


                            nameElement = EditCityTB.Text.Trim();

                            if (CityCB.Text == nameElement)
                            {
                                EnableAllForEditElements(isEdit, 2, EditCityTB, ImageECity, ImageDCity);
                                return;
                            }




                            var checkCity = DBEntities.GetContext().City.FirstOrDefault(u => u.NameCity == nameElement);
                            var editableCity = DBEntities.GetContext().City.FirstOrDefault(u => u.NameCity == CityCB.Text);

                            if (checkCity != null)
                                throw new Exception("Данный город уже существует.");


                            messageWin = new MessageWin("Вы действительно хотите изменить текущее название города?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;


                            editableCity.NameCity = nameElement;


                            DBEntities.GetContext().SaveChanges();


                            EnableAllForEditElements(isEdit, 2, EditCityTB, ImageECity, ImageDCity);



                            RegionCB.ItemsSource = null;
                            CityCB.ItemsSource = null;
                            StreetCB.ItemsSource = null;

                            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

                            RegionCB.Text = saveEditRegion;
                            CityCB.Text = nameElement;
                            StreetCB.Text = saveEditStreet;

                        }
                        break;
                    case "EditStreetBTN":
                        {
                            captionError = "Ошибка редактирование города";




                            if (!isEdit)
                            {
                                saveEditRegion = RegionCB.Text;
                                saveEditCity = CityCB.Text;

                                EnableAllForEditElements(isEdit, 3, EditStreetTB, ImageEStreet, ImageDStreet);
                                EditStreetTB.Text = StreetCB.Text;

                                return;
                            }



                            nameElement = EditStreetTB.Text.Trim();

                            if (StreetCB.Text == nameElement)
                            {
                                EnableAllForEditElements(isEdit, 3, EditStreetTB, ImageEStreet, ImageDStreet);
                                return;
                            }



                            var checkStreet = DBEntities.GetContext().Street.FirstOrDefault(u => u.NameStreet == nameElement);
                            var editableStreet = DBEntities.GetContext().Street.FirstOrDefault(u => u.NameStreet == StreetCB.Text);

                            if (checkStreet != null)
                                throw new Exception("Данная улица уже существует.");

                            messageWin = new MessageWin("Вы действительно хотите изменить текущее название улицы?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;



                            editableStreet.NameStreet = nameElement;


                            DBEntities.GetContext().SaveChanges();



                            EnableAllForEditElements(isEdit, 3, EditStreetTB, ImageEStreet, ImageDStreet);

                            RegionCB.ItemsSource = null;
                            CityCB.ItemsSource = null;
                            StreetCB.ItemsSource = null;

                            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

                            RegionCB.Text = saveEditRegion;
                            CityCB.Text = saveEditCity;
                            StreetCB.Text = nameElement;



                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                new MessageWin(captionError, ex, MessageCode.Error).ShowDialog();
            }
        }


        private void EditTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox selectedTB = sender as TextBox;


            switch (selectedTB.Name)
            {
                case "EditDepoIssueTB":
                    EditDepBTN.IsEnabled = !(string.IsNullOrWhiteSpace(EditDepoIssueTB.Text) || string.IsNullOrWhiteSpace(EditCodeDepTB.Text));
                    break;
                case "EditCodeDepTB":
                    selectedTB.CodeDepNum();
                    EditDepBTN.IsEnabled = !(string.IsNullOrWhiteSpace(EditDepoIssueTB.Text) || string.IsNullOrWhiteSpace(EditCodeDepTB.Text));
                    break;
                case "EditRegionTB":
                    EditRegionBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);
                    break;
                case "EditCityTB":
                    EditCityBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);
                    break;
                case "EditStreetTB":
                    EditStreetBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorLB.Text = "";

            (sender as ComboBox).Tag = null;

            DBEntities.NullContext();
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLB.Text = "";

            var selectedCB = sender as ComboBox;

            selectedCB.Tag = null;

            if (string.IsNullOrWhiteSpace(selectedCB.Text))
            {
                selectedCB.IsDropDownOpen = false;
            }


            try
            {
                switch (selectedCB.Name)
                {
                    case "DepoIssueCB":
                        {
                            CodeDepTB.IsEnabled = !string.IsNullOrWhiteSpace(selectedCB.Text);

                            DepartamentIssue DepartamentIssue = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.NameDep == selectedCB.Text.Trim());

                            if (DepartamentIssue != null)
                            {
                                CodeDepTB.Text = DepartamentIssue.CodeDep;
                                CodeDepTB.IsReadOnly = true;
                                CodeDepTB.Cursor = Cursors.No;
                                CodeDepTB.Tag = null;

                                ButtonDepoIssueSP.IsEnabled = true;
                            }
                            else
                            {
                                CodeDepTB.IsReadOnly = false;
                                CodeDepTB.Text = "";
                                CodeDepTB.Cursor = Cursors.Arrow;

                                ButtonDepoIssueSP.IsEnabled = false;
                            }


                        }
                        break;
                    case "RegionCB":
                        {
                            CityCB.IsEnabled = !string.IsNullOrWhiteSpace(selectedCB.Text);


                            List<City> city = DBEntities.GetContext().City.Where(u => u.Region.NameRegion == selectedCB.Text.Trim()).OrderBy(u => u.CityID).ToList();



                            if (city.Count > 0)
                            {
                                CityCB.ItemsSource = city;
                                ButtonRegionSP.IsEnabled = true;
                                return;
                            }

                            CityCB.ItemsSource = DBEntities.GetContext().City.Where(u => u.Region.NameRegion == null).OrderBy(u => u.CityID).ToList();

                            ButtonRegionSP.IsEnabled = RegionCB.Items.Count > 0;
                            ButtonRegionSP.IsEnabled = false;

                            CityCB.Text = "";
                            StreetCB.Text = "";


                        }
                        break;
                    case "CityCB":
                        {
                            StreetCB.IsEnabled = !string.IsNullOrWhiteSpace(selectedCB.Text);


                            StreetCB.ItemsSource = DBEntities.GetContext().StreetsOfCity
                                .Where(u => u.City.NameCity == selectedCB.Text.Trim() && u.City.Region.NameRegion == RegionCB.Text.Trim())
                                .OrderBy(u => u.StreetID).ToList();


                            ButtonCitySP.IsEnabled = StreetCB.Items.Count > 0;


                            StreetCB.Text = "";

                        }
                        break;
                    case "StreetCB":
                        {
                            var check = DBEntities.GetContext().StreetsOfCity.FirstOrDefault(u => u.Street.NameStreet == selectedCB.Text.Trim() &&
                                                                                                  u.City.NameCity == CityCB.Text.Trim());
                            ButtonStreetSP.IsEnabled = check != null;
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                new MessageWin(ex, MessageCode.Error).ShowDialog();
            }

        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).Text = (sender as ComboBox).Text.Trim();
        }

        private void EnableAllForEditElements(bool state, int WhatEditBTN, TextBox textBox, Image editImage, Image closeImage)
        {
            isEdit = !isEdit;

            textBox.Visibility = state ? Visibility.Hidden : Visibility.Visible;


            editImage.Tag = state ? null : GlobalVarriabels.IsEditBtnTag;
            closeImage.Tag = state ? null : GlobalVarriabels.IsEditBtnTag;




            LeftArrowBTN.IsEnabled = state;
            AddEditBTN.IsEnabled = state;

            RegionCB.IsEnabled = state;
            CityCB.IsEnabled = state;
            StreetCB.IsEnabled = state;

            HomeTB.IsEnabled = state;
            BuildingTB.IsEnabled = state;
            AppartamentTB.IsEnabled = state;
            PassportTB.IsEnabled = state;

            DateOfBirthDP.IsEnabled = state;
            DateOfIssueDP.IsEnabled = state;

            ManRB.IsEnabled = state;
            WomanRB.IsEnabled = state;

            switch (WhatEditBTN)
            {
                case 1:
                    {
                        EditDepBTN.IsEnabled = state;
                        DeleteDepBTN.IsEnabled = state;

                        DepoIssueCB.IsEnabled = state;
                        CodeDepTB.IsEnabled = state;

                        EditCityBTN.IsEnabled = state;
                        DeleteCityBTN.IsEnabled = state;

                        EditStreetBTN.IsEnabled = state;
                        DeleteStreetBTN.IsEnabled = state;
                    }
                    break;
                case 2:
                    {
                        EditDepBTN.IsEnabled = state;
                        DeleteDepBTN.IsEnabled = state;

                        DepoIssueCB.IsEnabled = state;
                        CodeDepTB.IsEnabled = state;

                        EditRegionBTN.IsEnabled = state;
                        DeleteRegionBTN.IsEnabled = state;

                        EditStreetBTN.IsEnabled = state;
                        DeleteStreetBTN.IsEnabled = state;
                    }
                    break;
                case 3:
                    {
                        EditDepBTN.IsEnabled = state;
                        DeleteDepBTN.IsEnabled = state;

                        DepoIssueCB.IsEnabled = state;
                        CodeDepTB.IsEnabled = state;

                        EditRegionBTN.IsEnabled = state;
                        DeleteRegionBTN.IsEnabled = state;

                        EditCityBTN.IsEnabled = state;
                        DeleteCityBTN.IsEnabled = state;
                    }
                    break;
                case 4:
                    {
                        EditCodeDepTB.Visibility = state ? Visibility.Hidden : Visibility.Visible;

                        EditRegionBTN.IsEnabled = state;
                        DeleteRegionBTN.IsEnabled = state;

                        EditCityBTN.IsEnabled = state;
                        DeleteCityBTN.IsEnabled = state;

                        EditStreetBTN.IsEnabled = state;
                        DeleteStreetBTN.IsEnabled = state;
                    }
                    break;

            }


        }


        private bool isEdit = false;
        private void DeleteItemComboBoxBTN_Click(object sender, RoutedEventArgs e)
        {

            var selectedBTN = sender as Button;

            DBEntities.NullContext();

            DepartamentCompany checkAdress;
            Staff checkStaff;
            Passport checkPassportStaff;

            MessageWin messageWin;

            string captionError = "";
            string selectedText;


            try
            {
                switch (selectedBTN.Name)
                {
                    case "DeleteDepBTN":
                        {

                            if (isEdit)
                            {
                                EditDepBTN.IsEnabled = true;

                                EnableAllForEditElements(isEdit, 4, EditDepoIssueTB, ImageEDep, ImageDDep);
                                return;
                            }


                            captionError = "Ошибка удаления департамента";
                            selectedText = DepoIssueCB.Text.Trim();


                            checkStaff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.Passport.DepartamentIssue.NameDep == selectedText);

                            if (checkStaff != null)
                                throw new Exception("Данный департамент уже находиться в паспортах сотрудников.\nУдаление невозможно.");



                            messageWin = new MessageWin("Вы действительно хотите удалить данный департамент?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;


                            DepartamentIssue deleteDepartamentIssue = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.NameDep == selectedText);


                            DBEntities.GetContext().DepartamentIssue.Remove(deleteDepartamentIssue);

                            DBEntities.GetContext().SaveChanges();

                            DepoIssueCB.Text = "";
                            CodeDepTB.Text = "";

                            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

                        }
                        break;
                    case "DeleteRegionBTN":
                        {

                            if (isEdit)
                            {
                                EditRegionBTN.IsEnabled = true;

                                EnableAllForEditElements(isEdit, 1, EditRegionTB, ImageERegion, ImageDRegion);
                                return;
                            }


                            captionError = "Ошибка удаления региона";
                            selectedText = RegionCB.Text.Trim();


                            checkAdress = DBEntities.GetContext().DepartamentCompany.FirstOrDefault(u => u.Adress.Region.NameRegion == selectedText);
                            checkPassportStaff = DBEntities.GetContext().Passport.FirstOrDefault(u => u.Adress.Region.NameRegion == selectedText);

                            if (checkAdress != null)
                                throw new Exception("Данный регион уже используется отделом.\nУдаление невозможно.");

                            if (checkPassportStaff != null)
                                throw new Exception("Данный регион уже используется в паспорте сотрудника.\nУдаление невозможно.");


                            messageWin = new MessageWin("Вы действительно хотите удалить данный регион?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;


                            Region deleteRegion = DBEntities.GetContext().Region.FirstOrDefault(u => u.NameRegion == selectedText);


                            City[] selectCities = DBEntities.GetContext().City.Where(u => u.RegionID == deleteRegion.RegionID).ToArray();

                            foreach (var item in selectCities)
                            {
                                item.RegionID = null;
                            }

                            DBEntities.GetContext().Region.Remove(deleteRegion);

                            DBEntities.GetContext().SaveChanges();

                            RegionCB.Text = "";
                            CityCB.Text = "";
                            StreetCB.Text = "";

                            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

                        }
                        break;
                    case "DeleteCityBTN":
                        {

                            if (isEdit)
                            {
                                EditCityBTN.IsEnabled = true;

                                EnableAllForEditElements(isEdit, 2, EditCityTB, ImageECity, ImageDCity);
                                return;
                            }

                            captionError = "Ошибка удаления города";
                            selectedText = CityCB.Text.Trim();


                            checkAdress = DBEntities.GetContext().DepartamentCompany.FirstOrDefault(u => u.Adress.City.NameCity == selectedText);
                            checkPassportStaff = DBEntities.GetContext().Passport.FirstOrDefault(u => u.Adress.City.NameCity == selectedText);

                            if (checkAdress != null)
                                throw new Exception("Данный город уже используется отделом.\nУдаление невозможно.");

                            if (checkPassportStaff != null)
                                throw new Exception("Данный город уже используется в паспорте сотрудника.\nУдаление невозможно.");


                            messageWin = new MessageWin("Вы действительно хотите удалить данный город?", MessageCode.Question);
                            messageWin.ShowDialog();


                            if (!(bool)messageWin.DialogResult) return;




                            City deleteCity = DBEntities.GetContext().City.FirstOrDefault(u => u.NameCity == selectedText);

                            StreetsOfCity[] selectStreetsOfCity = DBEntities.GetContext().StreetsOfCity.Where(u => u.CityID == deleteCity.CityID).ToArray();

                            foreach (var item in selectStreetsOfCity)
                            {
                                DBEntities.GetContext().StreetsOfCity.Remove(item);
                            }

                            DBEntities.GetContext().City.Remove(deleteCity);

                            DBEntities.GetContext().SaveChanges();


                            CityCB.ItemsSource = DBEntities.GetContext().City.Where(u => u.Region.NameRegion == CityCB.Text.Trim()).OrderBy(u => u.CityID).ToList();

                            CityCB.Text = "";
                            StreetCB.Text = "";
                        }
                        break;
                    case "DeleteStreetBTN":
                        {

                            if (isEdit)
                            {
                                EditStreetBTN.IsEnabled = true;
                                EnableAllForEditElements(isEdit, 3, EditStreetTB, ImageEStreet, ImageDStreet);
                                return;
                            }

                            captionError = "Ошибка удаления города";
                            selectedText = StreetCB.Text.Trim();


                            checkAdress = DBEntities.GetContext().DepartamentCompany.FirstOrDefault(u => u.Adress.Street.NameStreet == selectedText);
                            checkPassportStaff = DBEntities.GetContext().Passport.FirstOrDefault(u => u.Adress.Street.NameStreet == selectedText);

                            if (checkAdress != null)
                                throw new Exception("Данная улица уже используется отделом.\nУдаление невозможно.");

                            if (checkPassportStaff != null)
                                throw new Exception("Данная улица уже используется в паспорте сотрудника.\nУдаление невозможно.");



                            messageWin = new MessageWin("Вы действительно хотите удалить данную улицу?" +
                                "\nДанная улица будет удалена у привязанных городов.", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (!(bool)messageWin.DialogResult) return;


                            Street streetDelete = DBEntities.GetContext().Street.FirstOrDefault(u => u.NameStreet == selectedText);

                            StreetsOfCity[] selectStreetsOfCity = DBEntities.GetContext().StreetsOfCity.Where(u => u.StreetID == streetDelete.StreetID).ToArray();

                            foreach (var item in selectStreetsOfCity)
                            {
                                DBEntities.GetContext().StreetsOfCity.Remove(item);
                            }

                            DBEntities.GetContext().Street.Remove(streetDelete);

                            DBEntities.GetContext().SaveChanges();

                            StreetCB.Text = "";

                            StreetCB.ItemsSource = DBEntities.GetContext().StreetsOfCity
                                .Where(u => u.City.NameCity == CityCB.Text.Trim() &&
                                            u.City.Region.NameRegion == RegionCB.Text.Trim())
                                            .OrderBy(u => u.StreetID).ToList();

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                new MessageWin(captionError, ex, MessageCode.Error).ShowDialog();
            }
        }



        private void DateOfPicker_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as DatePicker).SelectedDate != null)
            {
                (sender as DatePicker).Tag = null;
            }
        }


        private void AddDepRegionCityStreet(out DepartamentIssue departamentIssue, out Region newRegion, out City newCity, out Street newStreet)
        {


            //-----------Проверка и добавление депортамента---------------------


            departamentIssue = DBEntities.GetContext().DepartamentIssue.FirstOrDefault(u => u.NameDep == DepoIssueCB.Text);


            if (departamentIssue == null)
            {
                departamentIssue = new DepartamentIssue();

                departamentIssue.NameDep = DepoIssueCB.Text;
                departamentIssue.CodeDep = CodeDepTB.Text;

                DBEntities.GetContext().DepartamentIssue.Add(departamentIssue);
            }


            //-----------Проверка и добавление Региона---------------------


            newRegion = DBEntities.GetContext().Region.FirstOrDefault(u => u.NameRegion == RegionCB.Text);


            if (newRegion == null)
            {
                newRegion = new Region();

                newRegion.NameRegion = RegionCB.Text;

                DBEntities.GetContext().Region.Add(newRegion);
            }


            //-----------Проверка и добавление города-------------------- -


            newCity = DBEntities.GetContext().City.FirstOrDefault(u => u.NameCity == CityCB.Text);

            if (newCity != null && CityCB.SelectedValue == null && newCity.RegionID != null)
            {
                if (!isRightActive)
                {
                    ArrowBTN_Click(null, null);
                    CityCB.Focus();
                }
                throw new Exception($"Данный город уже привязан к региону - \"{newCity.Region.NameRegion}\"");
            }


            if (newCity == null)
            {
                newCity = new City();


                newCity.NameCity = CityCB.Text.Trim();
                newCity.RegionID = newRegion.RegionID;


                DBEntities.GetContext().City.Add(newCity);
            }

            //-----------Проверка и добавление улицы---------------------


            newStreet = DBEntities.GetContext().Street.FirstOrDefault(u => u.NameStreet == StreetCB.Text);


            if (newStreet == null)
            {
                newStreet = new Street();
                newStreet.NameStreet = StreetCB.Text.Trim();

                DBEntities.GetContext().Street.Add(newStreet);
            }


            StreetsOfCity streetsOfCity = DBEntities.GetContext().StreetsOfCity
                                        .FirstOrDefault(u => u.City.NameCity == CityCB.Text &&
                                                             u.Street.NameStreet == StreetCB.Text);

            if (streetsOfCity == null)
            {
                streetsOfCity = new StreetsOfCity();

                streetsOfCity.CityID = newCity.CityID;
                streetsOfCity.StreetID = newStreet.StreetID;

                DBEntities.GetContext().StreetsOfCity.Add(streetsOfCity);

            }


        }

        private bool CheckLeftBorder()
        {
            bool gotError = false;

            if (selectedPhoto == "")
            {

                ErrorLB.Text = "Выберите фото";
                BorderPhoto.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            ValidationDataClass.CheckFields(ref gotError, SNPstaffTB);

            string[] SplitSNP = SNPstaffTB.Text.Split(' ');


            if (SplitSNP.Length > 1 && string.IsNullOrWhiteSpace(SplitSNP[1]) ||
                SplitSNP.Length == 1 && !string.IsNullOrWhiteSpace(SplitSNP[0]))
            {
                ErrorLB.Text = "Введите имя";
                SNPstaffTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }
            else if (SplitSNP.Length == 1 && string.IsNullOrWhiteSpace(SplitSNP[0]))
            {
                ErrorLB.Text = "Введите фамилию";
                SNPstaffTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }


            ValidationDataClass.CheckFields(ref gotError, LoginTB);
            ValidationDataClass.CheckFields(ref gotError, PasswordPB);
            ValidationDataClass.CheckFields(ref gotError, PhoneTB);

            if (PasswordPB.Password.Length < 7 && !gotError)
            {
                ErrorLB.Text = "Пароль должен содержать минимум 7 символов";

                PasswordPB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            if (PhoneTB.Text.Length < 18 && !gotError)
            {
                ErrorLB.Text = "Телефон введён не до конца";

                PhoneTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            ValidationDataClass.CheckFields(ref gotError, RoleCB, true);
            if (DeportamentCB.IsVisible)
                ValidationDataClass.CheckFields(ref gotError, DeportamentCB, true);
            ValidationDataClass.CheckFields(ref gotError, StatusCB, true);



            return gotError;
        }

        private bool CheckRightBorder()
        {
            bool gotError = false;

            ValidationDataClass.CheckFields(ref gotError, DepoIssueCB, true);
            ValidationDataClass.CheckFields(ref gotError, CodeDepTB);
            ValidationDataClass.CheckFields(ref gotError, PassportTB);


            if (PassportTB.Text.Length < 12 && !gotError)
            {
                ErrorLB.Text = "Паспорт введён не до конца";

                PassportTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            if (CodeDepTB.Text.Length < 7 && !gotError)
            {
                ErrorLB.Text = "Код депортамента введён не до конца";

                CodeDepTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }



            ValidationDataClass.CheckFields(ref gotError, CodeDepTB);
            ValidationDataClass.CheckFields(ref gotError, RegionCB, false);
            ValidationDataClass.CheckFields(ref gotError, CityCB, false);
            ValidationDataClass.CheckFields(ref gotError, StreetCB, false);
            ValidationDataClass.CheckFields(ref gotError, HomeTB);
            ValidationDataClass.CheckFields(ref gotError, AppartamentTB);
            ValidationDataClass.CheckFields(ref gotError, DateOfBirthDP);
            ValidationDataClass.CheckFields(ref gotError, DateOfIssueDP);


            if (ManRB.IsChecked == false && WomanRB.IsChecked == false && !gotError)
            {
                ErrorLB.Text = "Выберите пол";
                gotError = true;
            }

            return gotError;

        }


        private bool CheckFieldsBeforeReg()
        {
            bool GotErrorLeft = CheckLeftBorder();
            bool GotErrorRight = CheckRightBorder();


            bool gotError = GotErrorLeft || GotErrorRight;


            if ((!isRightActive && GotErrorRight && !GotErrorLeft) ||
                (isRightActive && GotErrorLeft && !GotErrorRight))
                ArrowBTN_Click(null, null);



            if (gotError) SystemSounds.Hand.Play();

            return gotError;
        }


        private async void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {

            Keyboard.ClearFocus();

            if (CheckFieldsBeforeReg()) return;


            DBEntities.NullContext();


            DepartamentIssue newDepartamentIssue;

            Passport newPassport;
            Adress newAdress;

            Region newRegion;
            City newCity;
            Street newStreet;

            try
            {
                int Home = Convert.ToInt32(HomeTB.Text);
                int Appartament = Convert.ToInt32(AppartamentTB.Text);

                var SNParray = SNPstaffTB.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var passportArray = PassportTB.Text.Split(' ');

                string passportSeries = passportArray[0] + " " + passportArray[1];
                string passportNum = passportArray[2];




                if (savePhone != PhoneTB.Text)
                {
                    var checkPhone = DBEntities.GetContext().Staff.FirstOrDefault(u => u.PhoneNum == PhoneTB.Text);

                    if (checkPhone != null)
                    {
                        PhoneTB.Tag = GlobalVarriabels.ErrorTag;
                        PhoneTB.Focus();
                        throw new Exception($"Данный телефон уже привязан к сотруднику: {checkPhone.User.Login}");
                    }
                }

                if (saveLogin != LoginTB.Text)
                {
                    var checkLogin = DBEntities.GetContext().Staff.FirstOrDefault(u => u.User.Login == LoginTB.Text);

                    if (checkLogin != null)
                    {
                        LoginTB.Tag = GlobalVarriabels.ErrorTag;
                        LoginTB.Focus();

                        throw new Exception($"Данный логин уже привязан к сотруднику:\n{checkLogin.Surname} {checkLogin.Name} {checkLogin.Patronymic}");
                    }
                }

                if (saveSeriesPas != passportSeries || saveNumPass != passportNum)
                {
                    var checkPassport = DBEntities.GetContext().Staff.FirstOrDefault(u => u.Passport.Series == passportSeries && u.Passport.Num == passportNum);

                    if (checkPassport != null)
                    {
                        PassportTB.Tag = GlobalVarriabels.ErrorTag;
                        PassportTB.Focus();

                        throw new Exception("Данный паспорт уже привязан к логину: " + checkPassport.User.Login);
                    }
                }


                AddDepRegionCityStreet(out newDepartamentIssue, out newRegion, out newCity, out newStreet);



                if (StaffID > 0)
                {

                    string message = "";

                    if (saveStatusID != Convert.ToInt32(StatusCB.SelectedValue))
                    {

                        if (Convert.ToInt32(StatusCB.SelectedValue) == 3 && saveStatusID != 3)
                            message += "При статусе \"Заблокирован\", сотрудник не сможет войти в свою запись." +
                                "\nИ через полгода можно будет удалить запись.\n\n";


                        if (saveStatusID == 3 && Convert.ToInt32(StatusCB.SelectedValue) != 3)
                            message += "Сменив статуc с \"Заблокирован\", сброситься дата возможности удаления.\n\n";


                        message += "Вы действительно хотите сменить статус сотрудника?";

                        MessageWin messageWin = new MessageWin(message, MessageCode.Question);

                        messageWin.ShowDialog();

                        if (!(bool)messageWin.DialogResult) return;
                    }

                    editStaff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.StaffID == StaffID);


                    if (editStaff == null)
                        throw new Exception("Отсутствует подключение или отдел был удалён.");

                    if (selectedPhoto != "Есть фото")
                        editStaff.Photo = PhotoImageClass.SetImageToBytes(ref selectedPhoto);


                    editStaff.Surname = SNParray[0];
                    editStaff.Name = SNParray[1];
                    editStaff.Patronymic = SNParray.Length > 2 ? SNParray[2] : null;

                    editStaff.User.Login = LoginTB.Text;
                    editStaff.User.Passsword = PasswordPB.Password;

                    editStaff.PhoneNum = PhoneTB.Text;
                    editStaff.Email = EmailTB.Text != "" ? EmailTB.Text : null;

                    editStaff.Passport.Series = passportSeries;
                    editStaff.Passport.Num = passportNum;

                    editStaff.Passport.DepOfIssueID = newDepartamentIssue.DepartamentID;
                    editStaff.Passport.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.SelectedDate);
                    editStaff.Passport.DateOfIssue = Convert.ToDateTime(DateOfIssueDP.SelectedDate);

                    editStaff.Passport.Adress.RegionID = newRegion.RegionID;
                    editStaff.Passport.Adress.CityID = newCity.CityID;
                    editStaff.Passport.Adress.StreetID = newStreet.StreetID;
                    editStaff.Passport.Adress.Home = Home;
                    editStaff.Passport.Adress.Building = string.IsNullOrWhiteSpace(BuildingTB.Text) ? null : BuildingTB.Text;
                    editStaff.Passport.Adress.Appartament = Appartament;

                    if (saveStatusID != 3 && Convert.ToInt32(StatusCB.SelectedValue) == 3)
                    {
                        editStaff.BlockDate = DateTime.Now.AddDays(180);
                    }
                    else
                    {
                        editStaff.BlockDate = null;
                    }


                    editStaff.User.RoleID = Convert.ToInt32(RoleCB.SelectedValue);

                    if (DeportamentCB.IsVisible)
                        editStaff.DepartamentID = Convert.ToInt32(DeportamentCB.SelectedValue);
                    else
                        editStaff.DepartamentID = GlobalVarriabels.curDepCompanyID;

                    editStaff.Passport.GenderID = ManRB.IsChecked == true ? 1 : 2;
                    editStaff.StatusID = Convert.ToInt32(StatusCB.SelectedValue);


                    EditStaff LogStaff = DBEntities.GetContext().EditStaff.FirstOrDefault(u => u.StaffID == editStaff.StaffID);

                    if (LogStaff == null)
                    {
                        LogStaff = new EditStaff();

                        LogStaff.StaffID = editStaff.StaffID;

                        DBEntities.GetContext().EditStaff.Add(LogStaff);
                    }

                    LogStaff.WhoEditStaffID = GlobalVarriabels.currentUserID;
                    LogStaff.DateEdit = Convert.ToDateTime(DateTime.Now);


                    DBEntities.GetContext().SaveChanges();

                    staffListPage.UpdateList();


                    new MessageWin("Сотрудник успешно отредактирован", MessageCode.Info).ShowDialog();

                    await GlobalVarriabels.FrontFrame.AnimWinClose();

                    return;
                }



                //---------------Добавление сотрудника-------------------------------------------------------------------------------------------------------------------------


                //----------Адресс---------------


                newAdress = new Adress();

                newAdress.RegionID = newRegion.RegionID;
                newAdress.CityID = newCity.CityID;
                newAdress.StreetID = newStreet.StreetID;
                newAdress.Home = Home;
                newAdress.Building = string.IsNullOrWhiteSpace(BuildingTB.Text) ? null : BuildingTB.Text;
                newAdress.Appartament = Appartament;

                DBEntities.GetContext().Adress.Add(newAdress);

                //----------Паспорт---------------


                newPassport = new Passport();

                newPassport.Series = passportSeries;
                newPassport.Num = passportNum;

                newPassport.DepOfIssueID = newDepartamentIssue.DepartamentID;
                newPassport.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.SelectedDate);
                newPassport.DateOfIssue = Convert.ToDateTime(DateOfIssueDP.SelectedDate);
                newPassport.AdressRegID = newAdress.AdressID;
                newPassport.GenderID = ManRB.IsChecked == true ? 1 : 2;

                DBEntities.GetContext().Passport.Add(newPassport);

                //-------------------------------------

                editStaff = new Staff();

                editStaff.Photo = PhotoImageClass.SetImageToBytes(ref selectedPhoto);

                editStaff.Surname = SNParray[0];
                editStaff.Name = SNParray[1];
                editStaff.Patronymic = SNParray.Length > 2 ? SNParray[2] : null;


                User user = new User();

                user.Login = LoginTB.Text;
                user.Passsword = PasswordPB.Password;
                user.RoleID = Convert.ToInt32(RoleCB.SelectedValue);

                DBEntities.GetContext().User.Add(user);

                editStaff.PassportID = newPassport.PassportID;

                editStaff.PhoneNum = PhoneTB.Text;
                editStaff.Email = EmailTB.Text != "" ? EmailTB.Text : null;


                if (DeportamentCB.IsVisible) editStaff.DepartamentID = Convert.ToInt32(DeportamentCB.SelectedValue);
                else editStaff.DepartamentID = GlobalVarriabels.curDepCompanyID;

                editStaff.StatusID = Convert.ToInt32(StatusCB.SelectedValue);

                editStaff.UserID = user.UserID;


                DBEntities.GetContext().Staff.Add(editStaff);


                DBEntities.GetContext().SaveChanges();

                staffListPage.UpdateList();

                new MessageWin("Сотрудник успешно добавлен", MessageCode.Info).ShowDialog();

                await GlobalVarriabels.FrontFrame.AnimWinClose();
            }
            catch (Exception ex)
            {


                if (StaffID > 0)
                {

                    new MessageWin("Ошибка редактирования",
                                   ex,
                                   MessageCode.Error).ShowDialog();
                }
                else
                {

                    new MessageWin("Ошибка добавления", ex,
                                   MessageCode.Error).ShowDialog();
                }

            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLB.Text = "";
        }

        private void PasswordPB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyEnglish();
        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {

            Label hintTextBlock = PasswordPB.Template.FindName("HintLB", PasswordPB) as Label;
            TextBlock textBlock = PasswordPB.Template.FindName("TextPassword", PasswordPB) as TextBlock;
            ToggleButton toggleButton = PasswordPB.Template.FindName("ShowPasswordBTN", PasswordPB) as ToggleButton;



            textBlock.Text = PasswordPB.Password;
            hintTextBlock.Visibility = PasswordPB.Password.Length < 1 ? Visibility.Visible : Visibility.Collapsed;
            toggleButton.Visibility = PasswordPB.Password.Length < 1 ? Visibility.Collapsed : Visibility.Visible;

            PasswordPB.Tag = null;
            ErrorLB.Text = "";
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
                ImagePhoto.ImageSource = PhotoImageClass.GetImageFromBytes(PhotoImageClass.SetImageToBytes(ref selectedPhoto));
                return;
            }

            new MessageWin("Файл должен быть: '.png','.jpeg','jpg'", MessageCode.Error).ShowDialog();


        }
    }
}
