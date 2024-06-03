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

namespace GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder
{
    /// <summary>
    /// Логика взаимодействия для AEProductMaPage.xaml
    /// </summary>
    public partial class AEProductStoragePage : Page
    {


        Storage editStorage;


        int storageProductID;

        public AEProductStoragePage( int storageProductID)
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;
            this.storageProductID = storageProductID;

        }

        private void LoadCB()
        {
            if (storageProductID == 0)
            {
                List<int> allStorages = DBEntities.GetContext().Storage
                    .Where(s => s.DepartamentID == GlobalVarriabels.curDepCompanyID)
                    .Select(s => s.ProductID).ToList();

                List<Product> products = DBEntities.GetContext().Product.Where(p => !allStorages.Contains(p.ProductID)).ToList();

                if (products.Count == 0)
                {
                    AddEditBTN.IsEnabled = false;
                    ProductCB.IsEnabled = false;
                    AmountTB.IsEnabled = false;
                    NumStorageTB.IsEnabled = false;

                    ErrorLB.Text = "Товары для добавления отсутствуют!";
                    return;
                }

                ProductCB.ItemsSource = products;

            }
            else
            {
                ProductCB.ItemsSource = DBEntities.GetContext().Product.Where(u => u.ProductID == storageProductID).ToList();
            }

        }










        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DBEntities.NullContext();

                LoadCB();

                if (storageProductID > 0)
                {

                    NamePageLB.Content = "Редактирование товара";
                    AddEditBTN.Content = "Редактировать";


                    editStorage = DBEntities.GetContext().Storage.FirstOrDefault(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID
                                                                                   && u.ProductID == storageProductID);

                    ProductCB.IsReadOnly = true;

                    ProductCB.SelectedValue = editStorage.ProductID;
                    AmountTB.Text = editStorage.Amount.ToString();
                    NumStorageTB.Text = editStorage.StorageNum;
                }



            }
            catch (Exception ex)
            {
                if (storageProductID > 0)
                {
                    new MessageWin("Ошибка выгрузки данных",
                                   ex,
                                   MessageCode.Error).ShowDialog();
                }
                else
                {
                    new MessageWin("Ошибка выгрузки данных", ex,
                                   MessageCode.Error).ShowDialog();
                }



                GlobalVarriabels.FrontFrame.FrameErrorBack();
                GlobalVarriabels.MainWindow.catalogListPage.UpdateStaffList();

                return;
            }


            Visibility = Visibility.Visible;
        }


        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.Trim(' ');
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyNumsTB();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedTextBox = sender as TextBox;

            if (selectedTextBox.Name == "PriceTB")
            {
                selectedTextBox.FloatNums();
            }
            else if (selectedTextBox.Name == "NumStorageTB")
            {
                selectedTextBox.StorageNum();
            }

            selectedTextBox.Tag = null;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorLB.Text = "";

            (sender as ComboBox).Tag = null;


            int selectedID = Convert.ToInt32((sender as ComboBox).SelectedValue);

            Product selectedProduct = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == selectedID);

            DataContext = selectedProduct;


            DBEntities.NullContext();
        }


        private bool CheckFieldsBeforeReg()
        {

            bool gotError = false;


            ValidationDataClass.CheckFields(ref gotError, ProductCB, true);
            ValidationDataClass.CheckFields(ref gotError, AmountTB);
            ValidationDataClass.CheckFields(ref gotError, NumStorageTB);


            if (gotError) SystemSounds.Hand.Play();

            return gotError;
        }

        private async void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            DBEntities.NullContext();
            Keyboard.ClearFocus();


            if (CheckFieldsBeforeReg()) return;

            try
            {


                if (storageProductID == 0)
                {
                    editStorage = new Storage();
                }
                else
                {
                    editStorage = DBEntities.GetContext().Storage
                        .FirstOrDefault(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID &&
                        u.ProductID == storageProductID);
                }

                if (storageProductID == 0)
                {
                    editStorage.ProductID = Convert.ToInt32(ProductCB.SelectedValue);

                    editStorage.DepartamentID = GlobalVarriabels.curDepCompanyID;
                }

                editStorage.Amount = Convert.ToInt32(AmountTB.Text);
                editStorage.StorageNum = NumStorageTB.Text;


                if (storageProductID == 0) DBEntities.GetContext().Storage.Add(editStorage);


                DBEntities.GetContext().SaveChanges();

                DBEntities.NullContext();
                GlobalVarriabels.MainWindow.catalogListPage.UpdateStaffList();

                if (storageProductID == 0)
                    new MessageWin("Товар успешно добавлен", MessageCode.Info).ShowDialog();
                else
                    new MessageWin("Товар успешно отредактирован", MessageCode.Info).ShowDialog();

                await GlobalVarriabels.FrontFrame.AnimWinClose();




            }
            catch (Exception ex)
            {
                if (storageProductID > 0)
                {
                    new MessageWin("Ошибка редактирование товара",
                                   ex,
                                   MessageCode.Error).ShowDialog();
                }
                else
                {
                    new MessageWin("Ошибка добавление товара", ex,
                                   MessageCode.Error).ShowDialog();
                }


            }
        }

    }
}
