using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.DeportamentFolder
{
    /// <summary>
    /// Логика взаимодействия для AEDeportamenPage.xaml
    /// </summary>
    public partial class AEDeportamenPage : Page
    {
        int deportamentID;
        DeportamentListPage deportamentListPage;
        DepartamentCompany editDeportamentCompany;


        string saveRegion;
        string saveCity;
        string saveStreet;
        string saveHome;
        string saveBuilding;


        public AEDeportamenPage(DeportamentListPage deportamentListPage, int deportamentID)
        {
            InitializeComponent();

            ButtonRegionSP.Width = 0;
            ButtonCitySP.Width = 0;
            ButtonStreetSP.Width = 0;

            ButtonRegionSP.IsEnabled = false;
            ButtonCitySP.IsEnabled = false;
            ButtonStreetSP.IsEnabled = false;


            GlobalVarriabels.IsFrontFrameActive = true;

            Visibility = Visibility.Hidden;

            this.deportamentID = deportamentID;
            this.deportamentListPage = deportamentListPage;

            if (deportamentID > 0)
            {
                NamePageLB.Content = "Редактирование отдела №" + deportamentID;
                AddEditBTN.Content = "Редактировать";
            }

        }


        private void LoadCB()
        {
            RegionCB.ItemsSource = DBEntities.GetContext().Region.OrderBy(u => u.RegionID).ToList();

            StatusCB.ItemsSource = DBEntities.GetContext().StatusDepartament.ToList();

            StatusCB.SelectedValue = 1;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DBEntities.NullContext();

                LoadCB();

                if (deportamentID > 0)
                {
                    editDeportamentCompany = DBEntities.GetContext().DepartamentCompany.FirstOrDefault(u => u.DepartamentID == deportamentID);


                    if (editDeportamentCompany == null)
                    {
                        throw new Exception("Отсутствует подключение или департамент был удалён.");
                    }

                    RegionCB.SelectedValue = Convert.ToInt32(editDeportamentCompany.Adress.RegionID);
                    CityCB.SelectedValue = Convert.ToInt32(editDeportamentCompany.Adress.CityID);
                    StreetCB.SelectedValue = Convert.ToInt32(editDeportamentCompany.Adress.StreetID);
                    HomeTB.Text = editDeportamentCompany.Adress.Home.ToString();
                    BuildingTB.Text = editDeportamentCompany.Adress.Building == null ? null : editDeportamentCompany.Adress.Building;
                    StatusCB.SelectedValue = Convert.ToInt32(editDeportamentCompany.StatusDepartamentID);


                    saveRegion = RegionCB.Text;
                    saveCity = CityCB.Text;
                    saveStreet = StreetCB.Text;
                    saveHome = HomeTB.Text;
                    saveBuilding = BuildingTB.Text;


                }



            }
            catch (Exception ex)
            {
                if (deportamentID > 0)
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


                DeleteRegionBTN.Visibility = Visibility.Collapsed;
                DeleteCityBTN.Visibility = Visibility.Collapsed;
                DeleteStreetBTN.Visibility = Visibility.Collapsed;

                GlobalVarriabels.FrontFrame.FrameErrorBack();
                deportamentListPage.UpdateList();

                return;
            }


            Visibility = Visibility.Visible;
        }


        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GlobalVarriabels.MainWindow.Focus();

            await GlobalVarriabels.FrontFrame.AnimWinClose();

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DBEntities.NullContext();
        }






        private void DeleteItemComboBoxBTN_Click(object sender, RoutedEventArgs e)
        {

            var selectedBTN = sender as Button;

            DBEntities.NullContext();

            DepartamentCompany checkAdress;
            Passport checkPassportStaff;

            MessageWin messageWin;

            string captionError = "";
            string selectedText;

            try
            {
                switch (selectedBTN.Name)
                {
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
                                throw new Exception("Данный регион уже используется департаментом.\nУдаление невозможно.");

                            if (checkPassportStaff != null)
                                throw new Exception("Данный регион уже используется в паспорте сотрудника.\nУдаление невозможно.");


                            messageWin = new MessageWin("Вы действительно хотите удалить данный регион?", MessageCode.Question);
                            messageWin.ShowDialog();

                            if (messageWin.DialogResult == false) return;


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
                            {
                                throw new Exception("Данный город уже используется департаментом.\nУдаление невозможно.");
                            }
                            if (checkPassportStaff != null)
                            {
                                throw new Exception("Данный город уже используется в паспорте сотрудника.\nУдаление невозможно.");
                            }

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
                                throw new Exception("Данная улица уже используется департаментом.\nУдаление невозможно.");

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



        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {


            var selectedCB = sender as ComboBox;

            selectedCB.Tag = null;


            try
            {
                switch (selectedCB.Name)
                {
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






        private void AddRegionCityStreet(out Region newRegion, out City newCity, out Street newStreet)
        {

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

            if (newCity != null && CityCB.SelectedValue == null)
                if (newCity.RegionID != null)
                    throw new Exception($"Данный город уже привязан к региону - \"{newCity.Region.NameRegion}\"");






            if (newCity == null)
            {
                newCity = new City();


                newCity.NameCity = CityCB.Text.Trim();
                newCity.RegionID = newRegion.RegionID;


                DBEntities.GetContext().City.Add(newCity);
            }
            else
            {
                if (newCity.RegionID == null)
                    newCity.RegionID = newRegion.RegionID;
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

        private bool CheckFieldsBeforeReg()
        {

            bool gotError = false;

            ValidationDataClass.CheckFields(ref gotError, RegionCB, false);
            ValidationDataClass.CheckFields(ref gotError, CityCB, false);
            ValidationDataClass.CheckFields(ref gotError, StreetCB, false);
            ValidationDataClass.CheckFields(ref gotError, HomeTB);
            ValidationDataClass.CheckFields(ref gotError, StatusCB, true);


            if (gotError) SystemSounds.Hand.Play();

            return gotError;
        }




        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).Text = (sender as ComboBox).Text.Trim();
        }

        private void HomeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyNumsTB();
        }

        private void HomeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            HomeTB.Tag = "";
        }

        bool isEdit = false;



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


        private void EnableAllForEditElements(bool state, int WhatEditBTN, TextBox textBox, Image editImage, Image closeImage)
        {
            isEdit = !isEdit;

            textBox.Visibility = state ? Visibility.Hidden : Visibility.Visible;
            editImage.Tag = state ? null : GlobalVarriabels.IsEditBtnTag;
            closeImage.Tag = state ? null : GlobalVarriabels.IsEditBtnTag;


            RegionCB.IsEnabled = state;
            CityCB.IsEnabled = state;
            AddEditBTN.IsEnabled = state;
            StreetCB.IsEnabled = state;
            HomeTB.IsEnabled = state;
            BuildingTB.IsEnabled = state;
            StatusCB.IsEnabled = state;

            switch (WhatEditBTN)
            {
                case 1:
                    EditCityBTN.IsEnabled = state;
                    DeleteCityBTN.IsEnabled = state;

                    EditStreetBTN.IsEnabled = state;
                    DeleteStreetBTN.IsEnabled = state;
                    break;
                case 2:
                    EditRegionBTN.IsEnabled = state;
                    DeleteRegionBTN.IsEnabled = state;

                    EditStreetBTN.IsEnabled = state;
                    DeleteStreetBTN.IsEnabled = state;
                    break;
                case 3:
                    EditRegionBTN.IsEnabled = state;
                    DeleteRegionBTN.IsEnabled = state;

                    EditCityBTN.IsEnabled = state;
                    DeleteCityBTN.IsEnabled = state;
                    break;

            }


        }





        private void EditTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox selectedTB = sender as TextBox;


            switch (selectedTB.Name)
            {
                case "EditRegionTB":
                    EditRegionBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);
                    break;
                case "EditCityTB":
                    EditCityBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);
                    break;
                case "EditStreetTB":

                    EditStreetBTN.IsEnabled = !string.IsNullOrWhiteSpace(selectedTB.Text);

                    break;
                default:
                    break;
            }
        }




        private async void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();

            if (CheckFieldsBeforeReg()) return;

            DBEntities.NullContext();


            Adress newAdress;

            Region newRegion;
            City newCity;
            Street newStreet;

            try
            {

                var Home = Convert.ToInt32(HomeTB.Text);


                if (saveRegion != RegionCB.Text &&
                    saveCity != CityCB.Text &&
                    saveStreet != StreetCB.Text &&
                    saveHome != HomeTB.Text &&
                    saveBuilding != BuildingTB.Text)
                {


                    newAdress = DBEntities.GetContext().Adress.FirstOrDefault(u => u.Region.NameRegion == RegionCB.Text &&
                                                                              u.City.NameCity == CityCB.Text &&
                                                                              u.Street.NameStreet == StreetCB.Text &&
                                                                              u.Home == Home &&
                                                                              u.Building == BuildingTB.Text &&
                                                                              u.Appartament == null);

                    if (newAdress != null)
                    {
                        new MessageWin("Данный адрес департамента уже существует", MessageCode.Error).ShowDialog();
                        return;
                    }

                }

                AddRegionCityStreet(out newRegion, out newCity, out newStreet);





                if (deportamentID > 0)
                {




                    editDeportamentCompany = DBEntities.GetContext().DepartamentCompany.FirstOrDefault(u => u.DepartamentID == deportamentID);


                    if (editDeportamentCompany == null)
                        throw new Exception("Отсутствует подключение или департамент был удалён.");

                    editDeportamentCompany.Adress.RegionID = newRegion.RegionID;
                    editDeportamentCompany.Adress.CityID = newCity.CityID;
                    editDeportamentCompany.Adress.StreetID = newStreet.StreetID;
                    editDeportamentCompany.Adress.Home = Home;
                    editDeportamentCompany.Adress.Building = string.IsNullOrWhiteSpace(BuildingTB.Text) ? null : BuildingTB.Text;
                    editDeportamentCompany.StatusDepartamentID = Convert.ToInt32(StatusCB.SelectedValue);

                    DBEntities.GetContext().SaveChanges();

                    deportamentListPage.UpdateList();

                    new MessageWin("Депортамент успешно отредактирован", MessageCode.Info).ShowDialog();

                    await GlobalVarriabels.FrontFrame.AnimWinClose();

                    return;
                }



                //---------------Добавление депортамента------------------



                newAdress = new Adress();

                newAdress.RegionID = newRegion.RegionID;
                newAdress.CityID = newCity.CityID;
                newAdress.StreetID = newStreet.StreetID;
                newAdress.Home = Home;
                newAdress.Building = string.IsNullOrWhiteSpace(BuildingTB.Text) ? null : BuildingTB.Text.Trim();

                DBEntities.GetContext().Adress.Add(newAdress);


                editDeportamentCompany = new DepartamentCompany();

                editDeportamentCompany.AdressID = newAdress.AdressID;
                editDeportamentCompany.StatusDepartamentID = Convert.ToInt32(StatusCB.SelectedValue);

                DBEntities.GetContext().DepartamentCompany.Add(editDeportamentCompany);


                DBEntities.GetContext().SaveChanges();
                deportamentListPage.UpdateList();

                new MessageWin("Департамент успешно добавлен", MessageCode.Info).ShowDialog();

                await GlobalVarriabels.FrontFrame.AnimWinClose();
            }
            catch (Exception ex)
            {

                if (deportamentID > 0)
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
    }
}
