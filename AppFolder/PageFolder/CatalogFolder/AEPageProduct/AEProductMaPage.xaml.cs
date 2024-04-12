using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.GlobalClassFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder
{
    /// <summary>
    /// Логика взаимодействия для AEProductMaPage.xaml
    /// </summary>
    public partial class AEProductMaPage : Page
    {
        string selectedPhoto = "";

        CatalogListPage catalogListPage;
        Product editProduct;


        int productID;
        string saveName;

        public AEProductMaPage(CatalogListPage catalogListPage, int productID)
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;

            this.catalogListPage = catalogListPage;
            this.productID = productID;

        }

        private void LoadCB()
        {
            CategoryCB.ItemsSource = DBEntities.GetContext().Category.OrderBy(u=>u.CategoryID).ToList();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DBEntities.NullContext();

                LoadCB();

                if (productID > 0)
                {
                    NamePageLB.Content = "Редактирование товара";
                    AddEditBTN.Content = "Редактировать";

                    editProduct = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == productID);


                    if (editProduct.Photo != null)
                    {
                        ImagePhoto.ImageSource = PhotoImageClass.GetImageFromBytes(editProduct.Photo);
                        selectedPhoto = "Есть фото";
                        BorderPhoto.BorderThickness = new Thickness(0);
                    }

                    NameProductTB.Text = saveName = editProduct.NameProduct;
                    PriceTB.Text = editProduct.Price.ToString();
                    CategoryCB.SelectedValue = Convert.ToInt32(editProduct.CategoryID);
                    DescriptionTB.Text = editProduct.Description;

                }



            }
            catch (Exception ex)
            {
                if (productID > 0)
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
                catalogListPage.UpdateAdminList();

                return;
            }


            Visibility = Visibility.Visible;
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

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedTextBox = sender as TextBox;

            if (selectedTextBox.Name == "PriceTB")
            {
                selectedTextBox.FloatNums();
            }

            selectedTextBox.Tag = null;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorLB.Text = "";

            (sender as ComboBox).Tag = null;

            DBEntities.NullContext();
        }


        private bool CheckFieldsBeforeReg()
        {

            bool gotError = false;

            if (selectedPhoto == "")
            {

                ErrorLB.Text = "Выберите фото";
                BorderPhoto.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            ValidationDataClass.CheckFields(ref gotError, NameProductTB);
            ValidationDataClass.CheckFields(ref gotError, PriceTB);

            if (!gotError && Convert.ToDecimal(PriceTB.Text) == 0)
            {
                ErrorLB.Text = "Цена не может быть равна нулю.";
                PriceTB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            ValidationDataClass.CheckFields(ref gotError, CategoryCB, true);
            ValidationDataClass.CheckFields(ref gotError, DescriptionTB);


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

                if (saveName != NameProductTB.Text)
                {
                    var checkName = DBEntities.GetContext().Product.FirstOrDefault(u => u.NameProduct == NameProductTB.Text);

                    if (checkName != null)
                    {
                        NameProductTB.Focus();
                        throw new Exception("Такое название товара уже существует.");

                    }
                }

                if (productID == 0)
                    editProduct = new Product();
                else
                    editProduct = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == productID);


                if (selectedPhoto != "Есть фото")
                {
                    editProduct.Photo = PhotoImageClass.SetImageToBytes(ref selectedPhoto);
                }

                editProduct.NameProduct = NameProductTB.Text;
                editProduct.Price = Convert.ToDecimal(PriceTB.Text);
                editProduct.CategoryID = Convert.ToInt32(CategoryCB.SelectedValue);
                editProduct.Description = DescriptionTB.Text;

                if (productID == 0)
                    DBEntities.GetContext().Product.Add(editProduct);


                DBEntities.GetContext().SaveChanges();

                if (productID == 0)
                    new MessageWin("Товар успешно добавлен", MessageCode.Info).ShowDialog();
                else
                    new MessageWin("Товар успешно отредактирован", MessageCode.Info).ShowDialog();


                await GlobalVarriabels.FrontFrame.AnimWinClose();

                catalogListPage.UpdateAdminList();



            }
            catch (Exception ex)
            {
                if (productID > 0)
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
