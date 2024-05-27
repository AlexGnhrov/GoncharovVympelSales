using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder
{
    /// <summary>
    /// Логика взаимодействия для CatalogListPage.xaml
    /// </summary>
    public partial class CatalogListPage : Page
    {
        public DispatcherTimer timerForUpdate;

        Product selectedProduct;
        Storage selectedStorage;

        public CatalogListPage()
        {
            InitializeComponent();


            FilterColumndCD.Width = new GridLength(0);

            timerForUpdate = new DispatcherTimer();
            timerForUpdate.Interval = TimeSpan.FromSeconds(0.3);
            timerForUpdate.Tick += Timer_Tick;


            if (GlobalVarriabels.RoleName.Client == GlobalVarriabels.currentRoleName)
            {
                CatalogMAdminLV.Visibility = Visibility.Collapsed;
                CatalogStaffLV.Visibility = Visibility.Visible;

                GridCategory.Visibility = Visibility.Collapsed;
                GridStatus.Visibility = Visibility.Collapsed;

                FilterColumndCD.Width = new GridLength(0, GridUnitType.Auto);

                //ContextMenuStaff.Visibility = Visibility.Collapsed;

                SeparatorCM.Visibility = Visibility.Collapsed;
                EditStorageMI.Visibility = Visibility.Collapsed;

                AddBTN.Visibility = Visibility.Collapsed;


                LoadFilterForCLient();

            }
            else if (GlobalVarriabels.isDepWorker)
            {

                CatalogMAdminLV.Visibility = Visibility.Collapsed;
                CatalogStaffLV.Visibility = Visibility.Visible;

                LoadCBforWorker();

            }
            else
            {
                CatalogMAdminLV.Visibility = Visibility.Visible;
                CatalogStaffLV.Visibility = Visibility.Collapsed;

                GridStatus.Visibility = Visibility.Collapsed;

                LoadCBforWorker();
                UpdateAdminList();
            }

            if (GlobalVarriabels.isReadOnly)
            {

                AddBTN.Visibility = Visibility.Collapsed;



                EditStorageMI.IsEnabled = false;
            }


        }




        public void Timer_Tick(object sender, EventArgs e)
        {
            DBEntities.NullContext();

            Cursor = Cursors.Wait;


            if (GlobalVarriabels.RoleName.Client == GlobalVarriabels.currentRoleName)
                UpdateClientList();
            else if (GlobalVarriabels.isDepWorker)
                UpdateStaffList();
            else
                UpdateAdminList();


            timerForUpdate.Stop();

            UpdateBTN.IsEnabled = true;

            Cursor = Cursors.Arrow;
        }


        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            timerForUpdate.Stop();
            timerForUpdate.Start();

            ClearLB.Visibility = !string.IsNullOrWhiteSpace(SearchTB.Text) ? Visibility.Visible : Visibility.Hidden; ;
        }


        private void LoadCBforWorker()
        {

            try
            {
                CategoryCB.ItemsSource = DBEntities.GetContext().Category.ToList();
                if (GlobalVarriabels.isDepWorker)
                {
                    StatusCB.ItemsSource = DBEntities.GetContext().StatusProduct.ToList();

                    StatusCB.SelectedValue = 1;
                }
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки комбокосов", ex, MessageCode.Error).ShowDialog();
            }
        }

        List<CheckBox> categoryCheckBoxes;

        public void LoadFilterForCLient()
        {
            try
            {
                DBEntities.NullContext();

                LoadCategoryFilter();
                LoadBrandFilter();
                LoadDepCombo();
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки фильтра", ex, MessageCode.Error).ShowDialog();
            }
        }

        private void LoadDepCombo()
        {
            Client client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

            var sourceDep = DBEntities.GetContext().DepartamentCompany.Where(u => u.StatusDepartamentID == 1).OrderBy(u => u.DepartamentID).ToList();

            DepoCompanyForClientCB.ItemsSource = sourceDep.ToList();


            if (client != null)
            {
                bool checkDep = DBEntities.GetContext().DepartamentCompany.Any(u => u.DepartamentID == client.SelectedDepID && u.StatusDepartamentID == 1);

                if (checkDep)
                {
                    DepoCompanyForClientCB.SelectedValue = int.Parse(client.SelectedDepID.ToString());
                }
                else
                {
                    var firstDep = DBEntities.GetContext().DepartamentCompany.Where(u=> u.StatusDepartamentID == 1).FirstOrDefault();

                    if (firstDep != null)
                    {
                        DepoCompanyForClientCB.SelectedValue = int.Parse(firstDep.DepartamentID.ToString());
                    }
                    else
                    {
                        client.SelectedDepID = null;

                        GlobalVarriabels.curDepCompanyID = 0;

                        DBEntities.GetContext().SaveChanges();
                    }


                }
            }
        }


        private void LoadCategoryFilter()
        {

            var elements = DBEntities.GetContext().Category.ToArray();

            categoryCheckBoxes = new List<CheckBox>();

            foreach (var item in elements)
            {
                CheckBox checkBox = new CheckBox();

                checkBox.IsChecked = false;
                checkBox.Content = item.NameCategory;
                checkBox.Margin = new Thickness(10, 5, 5, 5);
                checkBox.Padding = new Thickness(10, 0, 0, 0);
                checkBox.Click += CheckBox_Click;

                categoryCheckBoxes.Add(checkBox);

            }


        }

        List<CheckBox> brandCheckBoxes;

        private void LoadBrandFilter()
        {

            var elements = DBEntities.GetContext().Product.ToArray();

            brandCheckBoxes = new List<CheckBox>();

            foreach (var item in elements)
            {
                string nameBrand = GetNameBrandFromDescription(item.Description);

                if (!string.IsNullOrWhiteSpace(nameBrand) && !brandCheckBoxes.Any(check => check.Content.ToString() == nameBrand))
                {
                    CheckBox checkBox = new CheckBox
                    {
                        IsChecked = false,
                        Content = nameBrand,
                        Margin = new Thickness(10, 5, 5, 5),
                        Padding = new Thickness(10, 0, 0, 0)
                    };

                    checkBox.Click += CheckBox_Click;
                    brandCheckBoxes.Add(checkBox);
                }
            }

        }

        string GetNameBrandFromDescription(string description)
        {
            var splitDescription = description.Split(new char[] { ' ', '.', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitDescription.Length; i++)
            {
                if (splitDescription[i].Contains("Производитель"))
                {
                    return splitDescription.ElementAtOrDefault(i + 1);
                }
                else if (splitDescription[i].Contains("Разработчик") && splitDescription.ElementAtOrDefault(i + 1)?.Contains("видеокарты") == true)
                {
                    return splitDescription.ElementAtOrDefault(i + 2);
                }
            }

            return null;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            timerForUpdate.Stop();
            timerForUpdate.Start();
        }

        public void UpdateClientList()
        {

            try
            {

                int? selectedDep = Convert.ToInt32(DepoCompanyForClientCB.SelectedValue);

                if (selectedDep == 0) return;


                var mainSource = DBEntities.GetContext().Storage
                    .Where(u => (u.Product.NameProduct.Contains(SearchTB.Text) ||
                                 u.Product.Description.Contains(SearchTB.Text) ||
                                 u.StorageNum.Contains(SearchTB.Text)) &&
                                 u.DepartamentID == selectedDep).ToList();


                List<string> selectedCategory = CategoryFilterIC.Items.OfType<CheckBox>()
                    .Where(cb => cb.IsChecked == true)
                    .Select(cb => cb.Content.ToString()).ToList();

                List<string> selectedBrand = BrandFilterIC.Items.OfType<CheckBox>()
                                .Where(cb => cb.IsChecked == true)
                                .Select(cb => cb.Content.ToString()).ToList();



                mainSource = mainSource.Where(s => !selectedBrand.Any() ||
                              selectedBrand.Any(brand => s.Product.Description.Contains(brand))).ToList();



                mainSource = mainSource.Where(s => !selectedCategory.Any() ||
                                                           selectedCategory.Contains(s.Product.Category.NameCategory)).ToList();




                CatalogStaffLV.ItemsSource = mainSource.OrderBy(u => u.StatusProductID).ToList();



                CategoryFilterIC.ItemsSource = selectedCategory.Any() ?
                    CategoryFilterIC.ItemsSource.OfType<CheckBox>().OrderByDescending(u => u.IsChecked == true).ToList() : categoryCheckBoxes.ToList();

                BrandFilterIC.ItemsSource = selectedBrand.Any() ?
                     BrandFilterIC.ItemsSource.OfType<CheckBox>().OrderByDescending(u => u.IsChecked == true).ToList() : brandCheckBoxes.ToList();


                MessageListBorder.Visibility = CatalogStaffLV.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки списка", ex, MessageCode.Error).ShowDialog();
            }
        }


        public void UpdateStaffList()
        {


            IQueryable<Storage> source = null;

            int? selectedCategor = Convert.ToInt32(CategoryCB.SelectedValue);
            int? selectedStatus = Convert.ToInt32(StatusCB.SelectedValue);

            try
            {
                source = DBEntities.GetContext().Storage.Where(u => (u.Product.NameProduct.Contains(SearchTB.Text) ||
                                                                    u.Product.Description.Contains(SearchTB.Text) ||
                                                                    u.StorageNum.Contains(SearchTB.Text)) &&
                                                                    u.DepartamentID == GlobalVarriabels.curDepCompanyID &&
                                                                    u.StatusProductID == selectedStatus);

                if (selectedCategor != 0)
                    source = source.Where(u => u.Product.CategoryID == selectedCategor);

                CatalogStaffLV.ItemsSource = source.OrderBy(u => u.StatusProductID).ToList();


                MessageListBorder.Visibility = CatalogStaffLV.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки списка cотрудника", ex, MessageCode.Error).ShowDialog();
            }
        }


        public void UpdateAdminList()
        {



            IQueryable<Product> source = null;

            int? selectedCategor = Convert.ToInt32(CategoryCB.SelectedValue);

            try
            {
                source = DBEntities.GetContext().Product.Where(u => u.NameProduct.Contains(SearchTB.Text));

                if (selectedCategor != 0)
                    source = source.Where(u => u.CategoryID == selectedCategor);

                CatalogMAdminLV.ItemsSource = source.ToList();


                MessageListBorder.Visibility = CatalogMAdminLV.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки списка админа", ex, MessageCode.Error).ShowDialog();
            }
        }



        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVarriabels.RoleName.Client == GlobalVarriabels.currentRoleName) return;
            else if (GlobalVarriabels.isDepWorker)
                GlobalVarriabels.FrontFrame.Navigate(new AEProductStoragePage(0));
            else
                GlobalVarriabels.FrontFrame.Navigate(new AEProductMaPage(this, 0));
        }

        private void ClearLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchTB.Text = "";
        }


        private void ListViewButton_Click(object sender, RoutedEventArgs e)
        {
            DBEntities.NullContext();


            if ((GlobalVarriabels.isDepWorker && GlobalVarriabels.currentRoleName != GlobalVarriabels.RoleName.DepAdmin)
                || GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
            {

                Storage productOrder = CatalogStaffLV.SelectedItem as Storage;

                if (productOrder == null) return;

                GlobalVarriabels.FrontFrame.Navigate(new PreviewProductAddPage(this, productOrder.ProductID, true));

                return;
            }


            selectedProduct = CatalogMAdminLV.SelectedItem as Product;

            if ((sender as Button).Name == "AddItemAdminBTN")
                GlobalVarriabels.FrontFrame.Navigate(new AEProductMaPage(this, selectedProduct.ProductID));
            else if ((sender as Button).Name == "EditItemAdminBTN")
            {
                selectedStorage = CatalogStaffLV.SelectedItem as Storage;

                if (selectedStorage == null) return;

                GlobalVarriabels.FrontFrame.Navigate(new AEProductStoragePage(selectedStorage.ProductID));
            }
            else
                DeleteProduct();
        }



        public async void DeleteProduct()
        {
            try
            {
                var checkInStorage = DBEntities.GetContext().Storage.Where(u => u.ProductID == selectedProduct.ProductID);

                foreach (var item in checkInStorage)
                {
                    if (item.Amount > 0)
                    {
                        new MessageWin("Ошибка удаления", "Данный товар ещё не закончился в некоторых отделах", MessageCode.Error).ShowDialog();
                        return;
                    }
                }


                QuestionPage questionPage = new QuestionPage("Удалить", "Вы действительно хотите удалить данный товар?");

                GlobalVarriabels.FrontFrame.Navigate(questionPage);

                while (!questionPage.isYes)
                {
                    if (questionPage.isNo) return;

                    await Task.Delay(50);
                }


                Product deleteProduct = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == selectedProduct.ProductID);

                var deleteInStorage = DBEntities.GetContext().Storage.Where(u => u.ProductID == deleteProduct.ProductID);
                var deleteInBusket = DBEntities.GetContext().Busket.Where(u => u.ProductID == deleteProduct.ProductID);

                foreach (var item in deleteInStorage)
                {
                    DBEntities.GetContext().Storage.Remove(item);
                }
                foreach (var item in deleteInBusket)
                {
                    DBEntities.GetContext().Busket.Remove(item);
                }


                DBEntities.GetContext().Product.Remove(deleteProduct);

                DBEntities.GetContext().SaveChanges();

                UpdateAdminList();

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка удаления", ex, MessageCode.Error).ShowDialog();
            }
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if ((sender as ComboBox).Name == "DepoCompanyForClientCB")
                {


                    Client client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

                    int? selectedIndex = Convert.ToInt32(DepoCompanyForClientCB.SelectedValue);

                    if (selectedIndex == 0)
                        throw new Exception("Ошибка смены отдела");


                    client.SelectedDepID = selectedIndex;


                    DBEntities.GetContext().SaveChanges();

                    GlobalVarriabels.curDepCompanyID = Convert.ToInt32(selectedIndex);

                }

                UpdateBTN.IsEnabled = false;

                timerForUpdate.Stop();
                timerForUpdate.Start();

                ClearCheckLB.Visibility = CategoryCB.SelectedValue != null ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                new MessageWin(ex, MessageCode.Error).ShowDialog();
            }
        }

        private void ClearCheckLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CategoryCB.SelectedValue = null;
        }

        private void CatalogStafMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedItem = (sender as MenuItem);


            switch (selectedItem.Header)
            {
                case "Редактировать":
                    GlobalVarriabels.FrontFrame.Navigate(new AEProductStoragePage(selectedStorage.ProductID));
                    break;
                case "Обновить":
                    DBEntities.NullContext();
                    timerForUpdate.Start();
                    break;
            }
        }

        private void CatalogStaffLV_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedStorage = CatalogStaffLV.SelectedItem as Storage;


            EditStorageMI.IsEnabled = selectedStorage != null;

        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            UpdateBTN.IsEnabled = false;

            timerForUpdate.Stop();
            timerForUpdate.Start();
        }
    }
}
