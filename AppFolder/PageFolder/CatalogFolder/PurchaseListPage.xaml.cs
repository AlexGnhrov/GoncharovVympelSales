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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder
{
    /// <summary>
    /// Логика взаимодействия для PurchaseListPage.xaml
    /// </summary>
    public partial class PurchaseListPage : Page
    {
        DispatcherTimer timer;

        public bool skipExit = false;
        public bool orderDone = false;
        bool isCbLoaded = false;
        int saveCurDeportament = 0;

        public PurchaseListPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;

            Visibility = Visibility.Hidden;

            ButtonAdressSP.Width = 0;
            ButtonAdressSP.IsEnabled = false;

            if (GlobalVarriabels.isDepWorker)
                GridClient.Visibility = Visibility.Collapsed;


            DepoCompanyForClientCB.Visibility = Visibility.Collapsed;
            GridAdress.Visibility = Visibility.Collapsed;


        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {

                DBEntities.GetContext().SaveChanges();

                foreach (Busket item in BuscketProductDG.ItemsSource)
                {
                    if (item.MessageVisibility == Visibility.Visible)
                    {
                        AddOrderBTN.IsEnabled = false;
                        break;
                    }
                    else
                    {
                        AddOrderBTN.IsEnabled = true;
                    }
                }

                timer.Stop();

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }



        bool gotExit = false;
        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (orderDone)
                gotExit = true;

            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {



                saveCurDeportament = GlobalVarriabels.curDepCompanyID;

                if (!isCbLoaded)
                {
                    LoadCB();
                    isCbLoaded = true;
                }





                LoadBusket();
                DepoCompanyForClientCB.SelectedValue = GlobalVarriabels.curDepCompanyID;


            }
            catch (Exception ex)
            {

                new MessageWin("Ошибка выгрузки данных", ex, MessageCode.Error).ShowDialog();


                GlobalVarriabels.FrontFrame.FrameErrorBack();
                return;
            }


            Visibility = Visibility.Visible;
        }

        private bool CheckFieldsBeforeReg()
        {

            bool gotError = false;

            if (Convert.ToInt32(PaymentTypeCB.SelectedValue) == 0)
            {
                ErrorLB.Text = "Выберите тип оплаты!";
                PaymentTypeCB.Tag = GlobalVarriabels.ErrorTag;

                gotError = true;
            }

            if (GridClient.IsVisible)
            {
                if (DeliveryRB.IsChecked == false && PickupRB.IsChecked == false)
                {
                    if (!gotError)
                        ErrorLB.Text = "Выберите тип получения!";

                    gotError = true;
                }
                else if (DeliveryRB.IsChecked == true)
                {
                    if (!gotError && string.IsNullOrWhiteSpace(AdressCB.Text))
                    {
                        AdressCB.Tag = GlobalVarriabels.ErrorTag;
                        ErrorLB.Text = "Укажите адресс доставки!";

                        gotError = true;
                    }
                }
            }


            //if (gotError) SystemSounds.Hand.Play();

            return gotError;
        }
        private async void AddOrderBTN_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();

            await Task.Delay(200);

            DBEntities.NullContext();

            if (CheckFieldsBeforeReg()) return;

            try
            {
                LoadBusket();

                if (!AddOrderBTN.IsEnabled)
                {
                    return;
                }


                Order newOrder = new Order();

                newOrder.UQnum = CodeGenerator();

                if (GlobalVarriabels.isDepWorker)
                {
                    newOrder.StaffID = GlobalVarriabels.currentUserID;
                    newOrder.TypeOfIssueD = 2;
                    newOrder.PaymentTypetID = int.Parse(PaymentTypeCB.SelectedValue.ToString());
                    newOrder.StatusOrderID = 1;

                }
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                {
                    newOrder.ClientID = GlobalVarriabels.currentUserID;

                    if (PickupRB.IsChecked == true)
                    {
                        newOrder.TypeOfIssueD = 1;
                    }
                    else
                    {
                        newOrder.TypeOfIssueD = 3;

                        newOrder.DeliveryAdress = AdressCB.Text;

                        var adressClient = DBEntities.GetContext().AdressClient.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID && u.FullAdress == AdressCB.Text);

                        if (adressClient == null)
                        {
                            adressClient = new AdressClient();

                            adressClient.ClientID = GlobalVarriabels.currentUserID;
                            adressClient.FullAdress = AdressCB.Text;

                            DBEntities.GetContext().AdressClient.Add(adressClient);


                        }

                        Client client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

                        client.SelectedAdressID = adressClient.AdressClientID;


                    }

                    newOrder.PaymentTypetID = int.Parse(PaymentTypeCB.SelectedValue.ToString());
                    newOrder.StatusOrderID = 5;

                }

                newOrder.DepartamentCompanyID = GlobalVarriabels.curDepCompanyID;

                DBEntities.GetContext().Order.Add(newOrder);


                foreach (Busket item in BuscketProductDG.ItemsSource)
                {

                    ProductOrder newProductOrder = DBEntities.GetContext().ProductOrder
                        .Where(u => u.Photo == item.Product.Photo && u.NameProduct == item.Product.NameProduct)
                        .FirstOrDefault();




                    if (newProductOrder == null)
                    {
                        newProductOrder = new ProductOrder();


                        newProductOrder.Photo = item.Product.Photo;
                        newProductOrder.NameProduct = item.Product.NameProduct;
                        newProductOrder.ProductID = item.ProductID;


                        DBEntities.GetContext().ProductOrder.Add(newProductOrder);

                        DBEntities.GetContext().SaveChanges();
                    }


                    ProductGroup product = new ProductGroup();

                    product.OrderID = newOrder.OrderID;
                    product.ProductOrderID = newProductOrder.ProductOrderID;
                    product.Price = newProductOrder.Product.Price;
                    product.Amount = item.Amount;

                    DBEntities.GetContext().ProductGroup.Add(product);


                    Storage storageChangeAmount = DBEntities.GetContext().Storage
                        .Where(u => u.ProductID == item.ProductID && u.DepartamentID == GlobalVarriabels.curDepCompanyID)
                        .FirstOrDefault();

                    if (storageChangeAmount == null)
                        throw new Exception("Произошла ошибка при при оформление заказа.");

                    storageChangeAmount.Amount -= item.Amount;

                    DBEntities.GetContext().Busket.Remove(item);
                }

                DBEntities.GetContext().SaveChanges();

                orderDone = true;



                GlobalVarriabels.MainWindow.SetAmountBuscket();
                GlobalVarriabels.MainWindow.SetAmountOrder();

                GlobalVarriabels.MainWindow.orderListPage?.timerForUpdate.Start();
                GlobalVarriabels.MainWindow.catalogListPage.timerForUpdate.Start();



               await BorderSucced();

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
                LoadBusket();
            }
        }





        private void IncrementAmountBTN_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                ErrorLB.Text = "";



                var selectedItem = BuscketProductDG.SelectedItem as Busket;

                if (selectedItem == null) return;

                var BuscketItem = DBEntities.GetContext().Busket.FirstOrDefault(u => u.ClientBuscketID == selectedItem.ClientBuscketID);

                if (BuscketItem.Amount == 999 || BuscketItem.MaxAmount <= BuscketItem.Amount) return;

                BuscketItem.Amount++;

                DBEntities.GetContext().SaveChanges();

                LoadBusket();

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private void DecrenetAmountBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorLB.Text = "";

                var selectedItem = BuscketProductDG.SelectedItem as Busket;

                if (selectedItem == null) return;

                var BuscketItem = DBEntities.GetContext().Busket.FirstOrDefault(u => u.ClientBuscketID == selectedItem.ClientBuscketID);


                if (BuscketItem.Amount <= 1) return;


                --BuscketItem.Amount;

                DBEntities.GetContext().SaveChanges();


                LoadBusket();



            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }


        }

        private void AmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyNumsTB();
        }

        private void SumOverAllPrice()
        {
            decimal overallPrice = 0;

            foreach (Busket item in BuscketProductDG.Items)
            {
                overallPrice += item.Price;
            }

            PriceTBL.Text = overallPrice.ToString() + " ₽";
        }


        private void Page_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = BuscketProductDG.SelectedItem as Busket;


                if (selectedItem == null) return;

                DBEntities.GetContext().Busket.Remove(selectedItem);
                DBEntities.GetContext().SaveChanges();


                LoadBusket();

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }


        private void LoadCB()
        {
            PaymentTypeCB.ItemsSource = DBEntities.GetContext().PaymentType.ToList();
        }

        private void LoadBusket()
        {
            try
            {
                List<Busket> busketItems = null;

                if (GlobalVarriabels.isDepWorker)
                {
                    busketItems = DBEntities.GetContext().Busket.Where(u => u.StaffID == GlobalVarriabels.currentUserID).ToList();
                }
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                {
                    busketItems = DBEntities.GetContext().Busket.Where(u => u.ClientID == GlobalVarriabels.currentUserID).ToList();
                }



                bool isAnyMessageVisible = busketItems.Any(item => item.MessageVisibility == Visibility.Visible);

                foreach (var item in busketItems)
                {
                    if (item.Amount > 999)
                    {
                        item.Amount = 999;
                    }
                }

                MessageListBorder.Visibility = busketItems.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                AddOrderBTN.IsEnabled = !(isAnyMessageVisible || busketItems.Count == 0);

                BuscketProductDG.ItemsSource = busketItems.ToList();

                SumOverAllPrice();

                DBEntities.GetContext().SaveChanges();


                ErrorLB.Text = !AddOrderBTN.IsEnabled && busketItems.Count() > 0 ? "Некоторые товары недоступны к покупке или были изменены." : "";
            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }


        }



        private string CodeGenerator()
        {
            Random r = new Random();


            return $"{r.Next(0, 10000)}-{r.Next(0, 10000)}";
        }

        private async Task BorderSucced()
        {

            SuccseedPasswordBorder.Visibility = Visibility.Visible;

            DoubleAnimation OpaciyChange = new DoubleAnimation();

            var time = TimeSpan.FromSeconds(0.50);


            OpaciyChange.From = 0;
            OpaciyChange.To = 1;
            OpaciyChange.Duration = time;


            SuccseedPasswordBorder.BeginAnimation(OpacityProperty, OpaciyChange);


            await Task.Delay(TimeSpan.FromSeconds(2.5));

            if (!gotExit)
                await GlobalVarriabels.FrontFrame.AnimWinClose();



        }

        private void PaymentTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ComboBox).Tag = null;
            ErrorLB.Text = "";
        }

        private void DeleteItemAdressBTN_Click(object sender, RoutedEventArgs e)
        {
            if (IsEdit)
            {
                editChange();
                return;
            }
            try
            {
                MessageWin messageWin = null;

                string selectedText = AdressCB.Text.Trim();


                var checkAdress = DBEntities.GetContext().AdressClient.FirstOrDefault(u => u.FullAdress == selectedText &&
                u.ClientID == GlobalVarriabels.currentUserID);





                messageWin = new MessageWin("Вы действительно хотите удалить данный адрес?", MessageCode.Question);
                messageWin.ShowDialog();

                if (messageWin.DialogResult == false) return;

                var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.SelectedAdressID == checkAdress.AdressClientID);

                if (client != null)
                    client.SelectedAdressID = null;


                DBEntities.GetContext().AdressClient.Remove(checkAdress);

                DBEntities.GetContext().SaveChanges();


                AdressCB.ItemsSource = DBEntities.GetContext().AdressClient.Where(u => u.ClientID == client.ClientID).ToList();

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private bool IsEdit = false;
        private void editChange()
        {
            if (!IsEdit)
            {
                ImageEAdress.Tag = GlobalVarriabels.IsEditBtnTag;
                ImageDAdress.Tag = GlobalVarriabels.IsEditBtnTag;

                EditAdressTB.Text = AdressCB.Text;
                EditAdressTB.Visibility = Visibility.Visible;

                IsEdit = true;
            }
            else
            {
                ImageEAdress.Tag = null;
                ImageDAdress.Tag = null;

                EditAdressTB.Visibility = Visibility.Collapsed;

                IsEdit = false;
            }

            if (!EditAdressBTN.IsEnabled)
                EditAdressBTN.IsEnabled = true;

            BuscketProductDG.IsEnabled = !IsEdit;
            PaymentTypeCB.IsEnabled = !IsEdit;
            DeliveryRB.IsEnabled = !IsEdit;
            AddOrderBTN.IsEnabled = !IsEdit;
        }


        private void EditItemAdressBTN_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();


            try
            {

                if (AdressCB.Text == EditAdressTB.Text.Trim() && IsEdit)
                {
                    editChange();
                    return;
                }

                if (IsEdit)
                {
                    var Adress = DBEntities.GetContext().AdressClient.FirstOrDefault(u => u.FullAdress == AdressCB.Text.Trim());

                    Adress.FullAdress = EditAdressTB.Text.Trim();

                    DBEntities.GetContext().SaveChanges();



                    AdressCB.ItemsSource = null;
                    AdressCB.ItemsSource = DBEntities.GetContext().AdressClient.Where(u => u.ClientID == GlobalVarriabels.currentUserID).ToList();
                    AdressCB.Text = Adress.FullAdress;
                }
                editChange();
            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        bool isDepLoaded = false;

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radioButton = sender as RadioButton;

                if (radioButton.Content.ToString() == "Самовывоз")
                {
                    GridAdress.Visibility = Visibility.Collapsed;


                    if (!isDepLoaded)
                    {
                        DepoCompanyForClientCB.ItemsSource = DBEntities.GetContext().DepartamentCompany.Where(u => u.StatusDepartamentID == 1).ToList();
                        isDepLoaded = true;
                    }

                    if (DepoCompanyForClientCB.SelectedValue == null)
                        DepoCompanyForClientCB.SelectedValue = GlobalVarriabels.curDepCompanyID;

                    DepoCompanyForClientCB.Visibility = Visibility.Visible;
                }
                else
                {

                    DepoCompanyForClientCB.Visibility = Visibility.Collapsed;

                    var client = DBEntities.GetContext().Client.FirstOrDefault(u => u.ClientID == GlobalVarriabels.currentUserID);

                    if (AdressCB.Items.Count == 0)
                    {
                        AdressCB.ItemsSource = DBEntities.GetContext().AdressClient.Where(u => u.ClientID == client.ClientID).ToList();
                        AdressCB.Text = client?.AdressClient1?.FullAdress;
                    }
                    GridAdress.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private void EditTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            EditAdressBTN.IsEnabled = !string.IsNullOrWhiteSpace(EditAdressTB.Text);
        }

        private void DepoCompanyForClientCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (DepoCompanyForClientCB.SelectedValue != null)

            GlobalVarriabels.curDepCompanyID = Convert.ToInt32(DepoCompanyForClientCB.SelectedValue);


            LoadBusket();


        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (IsEdit)
                editChange();
            GlobalVarriabels.curDepCompanyID = saveCurDeportament;
        }

        private void AdressCB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AdressCB.Text))
                AdressCB.Text.Trim();
        }

        private void AdressCB_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                var adressClient = DBEntities.GetContext().AdressClient.Where(u => u.ClientID == GlobalVarriabels.currentUserID && u.FullAdress == AdressCB.Text.Trim()).ToList();

                ButtonAdressSP.IsEnabled = adressClient.Count > 0;

                AdressCB.Tag = null;
                ErrorLB.Text = "";
            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private void AdressCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DBEntities.NullContext();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            Border selectedBorder = sender as Border;
            ToolTip toolTip = selectedBorder.ToolTip as ToolTip;
            Border firstBorder = toolTip.Content as Border;
            Border imageBorder = firstBorder.Child as Border;

            ImageBrush imageBrush = imageBorder.Background as ImageBrush;


            Point mouse = e.GetPosition(selectedBorder);


            imageBrush.Viewbox = new Rect(mouse.X / (selectedBorder.ActualWidth * 1.75), mouse.Y / (selectedBorder.ActualHeight * 1.5), 0.4, 0.4);
        }
    }
}
