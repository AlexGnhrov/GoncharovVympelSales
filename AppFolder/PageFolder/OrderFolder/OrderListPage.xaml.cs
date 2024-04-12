using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder;
using GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder;
using GoncharovVympelSale.AppFolder.PageFolder.OrderFolder.AdditionalPage;
using GoncharovVympelSale.AppFolder.PageFolder.StaffFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.OrderFolder
{
    /// <summary>
    /// Логика взаимодействия для OrderListPage.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {

        public DispatcherTimer timerForUpdate;


        private Order selectedOrderItem;

        public OrderListPage()
        {
            InitializeComponent();

            timerForUpdate = new DispatcherTimer();
            timerForUpdate.Interval = TimeSpan.FromSeconds(0.3);
            timerForUpdate.Tick += Timer_Tick;

            if (GlobalVarriabels.isReadOnly ||
                GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
            {
                SeparatorMI.Visibility = Visibility.Collapsed;
                StatusMI.Visibility = Visibility.Collapsed;
            }

            if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client ||
               GlobalVarriabels.isDepWorker)
            {
                GridDepBox.Visibility = Visibility.Collapsed;
            }



            LoadCB();
            UpdateList();
        }


        private void LoadCB()
        {
            int? selecteStatus = Convert.ToInt32(StatusCB.SelectedValue);
            int? selectedDepCompany = Convert.ToInt32(DepoCompanyCB.SelectedValue);

            try
            {
                StatusOrder[] StatusList = DBEntities.GetContext().StatusOrder.OrderBy(u => u.StatusOrderID).ToArray();

                StatusOrder tempStatus = StatusList[1];

                StatusList[1] = StatusList[5];
                StatusList[5] = tempStatus;



                StatusCB.ItemsSource = StatusList.ToList();
                DepoCompanyCB.ItemsSource = DBEntities.GetContext().DepartamentCompany.ToList();

                var sas = DBEntities.GetContext().StatusOrder.OrderBy(u => u.StatusOrderID).ToList();




                StatusCB.SelectedValue = selecteStatus;
                DepoCompanyCB.SelectedValue = selectedDepCompany;
            }
            catch (Exception ex)
            {

                new MessageWin("Ошибка выгрузки комбокса", ex, MessageCode.Error).ShowDialog();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateList();
            timerForUpdate.Stop();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            timerForUpdate.Stop();
            timerForUpdate.Start();

            ClearLB.Visibility = string.IsNullOrWhiteSpace(SearchTB.Text) ? Visibility.Hidden : Visibility.Visible;
        }

        bool isUpdating = false;

        public async void UpdateList()
        {

            if (isUpdating) return;


            DBEntities.NullContext();

            UpdateListBTN.IsEnabled = false;
            UpdateListBTN.IsEnabled = false;
            isUpdating = true;

            Cursor = Cursors.Wait;

            int? selecteStatus = Convert.ToInt32(StatusCB.SelectedValue);
            int? selectedDepCompany = Convert.ToInt32(DepoCompanyCB.SelectedValue);

            try
            {

                IQueryable<Order> elements = DBEntities.GetContext().Order
                    .Where(u => u.UQnum.Contains(SearchTB.Text));

                if (GlobalVarriabels.isDepWorker)
                    elements = elements.Where(u => u.DepartamentCompanyID == GlobalVarriabels.curDepCompanyID);
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                    elements = elements.Where(u => u.ClientID == GlobalVarriabels.currentUserID);

                if (DepoCompanyCB.SelectedValue != null)
                    elements = elements.Where(u => u.DepartamentCompanyID == selectedDepCompany);

                if (StatusCB.SelectedValue != null)
                    elements = elements.Where(u => u.StatusOrderID == selecteStatus);



                GetExpiredOrder(elements);


                foreach (Order item in elements.Where(u => u.StatusOrderID == 1 || u.StatusOrderID == 2))
                {
                    item.SetGridVisibility();
                }

                //int pageSize = 100;
                //int startIndex = 0;
                //while (startIndex < elements.Count())
                //{
                //    OrderLV.ItemsSource = elements.OrderByDescending(u => u.StatusOrderID).ThenByDescending(u => u.OrderID).Skip(startIndex).Take(pageSize).ToList();
                //    startIndex += pageSize;
                //    await Task.Delay(100);
                //}
                OrderLV.ItemsSource = elements.OrderByDescending(u => u.StatusOrderID).ThenByDescending(u => u.OrderID).ToList();



                MessageListBorder.Visibility = OrderLV.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки заказов", ex, MessageCode.Error).ShowDialog();
            }
            finally
            {
                Cursor = Cursors.Arrow;
                isUpdating = false;

                UpdateListBTN.IsEnabled = true;
            }

        }


        private async Task GetExpiredOrder(IQueryable<Order> elements)
        {
            try
            {
                if (elements.Any(u => u.ShellLife == null)) return;

                foreach (Order item in elements)
                {
                    if (item.ShellLife != null && item.ShellLife < DateTime.Now)
                    {
                        item.StatusOrderID = 2;
                        item.ShellLife = null;
                        item.CancelDescription = "Дата хранения истекла";



                        List<Product> products = new List<Product>();

                        var ProductOfOrder = DBEntities.GetContext().ProductGroup.Where(u => u.OrderID == item.OrderID);


                        foreach (var product in ProductOfOrder)
                        {
                            if (product.ProductOrder?.ProductID == null) continue;

                            var Storage = DBEntities.GetContext().Storage.FirstOrDefault(u => u.DepartamentID == item.DepartamentCompanyID && u.ProductID == product.ProductOrder.ProductID);

                            if (Storage == null) continue;
                            Storage.Amount += product.Amount;

                        }


                        DBEntities.GetContext().SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка просроченных заказов", ex, MessageCode.Error).ShowDialog();
            }
        }



        private void ClearLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchTB.Text = "";
        }



        private void StackPanel_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timerForUpdate.Start();

            ClearStatusLB.Visibility = StatusCB.SelectedValue == null ? Visibility.Collapsed : Visibility.Visible;
            ClearDepLB.Visibility = DepoCompanyCB.SelectedValue == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ClearCheckLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Label).Name == "ClearStatusLB")
                StatusCB.SelectedValue = null;
            else
                DepoCompanyCB.SelectedValue = null;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menuItem = sender as MenuItem;

            switch (menuItem.Header)
            {
                case "Обновить":
                    LoadCB();
                    UpdateList();
                    break;
                default:
                    ChangeStatus(menuItem.Header.ToString());
                    break;
            }
        }

        private async void ChangeStatus(string NameHeader)
        {
            QuestionPage questionPage = null;

            int statusID = 0;
            string message = "Вы действительно хотите сменить статус заказа?";


            switch (NameHeader)
            {
                case "Получен":
                    message = message.Insert(0, "Поставив статус на \"Получен\", его нельзя будет потом изменить.\n");

                    questionPage = new QuestionPage("Смена статуса", message);
                    GlobalVarriabels.FrontFrame.Navigate(questionPage);

                    while (!questionPage.isYes)
                    {
                        if (questionPage.isNo) return;

                        await Task.Delay(50);
                    }

                    statusID = 1;
                    break;
                case "Ожидает в отделе":
                    statusID = 6;
                    break;
                case "В пути":
                    statusID = 3;
                    break;
                case "В сборе":
                    statusID = 4;
                    break;
                case "В ожидании":
                    statusID = 5;
                    break;
            }

            if (statusID == 0) return;





            try
            {
                DBEntities.NullContext();

                Order OrderStatus = DBEntities.GetContext().Order.FirstOrDefault(u => u.OrderID == selectedOrderItem.OrderID);

                if (OrderStatus.StaffID == null)
                    OrderStatus.StaffID = GlobalVarriabels.currentUserID;

                OrderStatus.ShellLife = statusID == 6 ? (DateTime?)DateTime.Now.AddDays(10) : null;


                OrderStatus.StatusOrderID = statusID;

                DBEntities.GetContext().SaveChanges();

                UpdateList();

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка смены статуса", ex, MessageCode.Error).ShowDialog();
            }


        }



        private void OrderLV_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedOrderItem = OrderLV.SelectedItem as Order;


            StatusMI.IsEnabled = selectedOrderItem != null;

            if (!StatusMI.IsEnabled) return;

            StatusMI.IsEnabled = !(selectedOrderItem.StatusOrder.NameStatus == "Получен" || selectedOrderItem.StatusOrder.NameStatus == "Отменён");




            var items = StatusMI.Items;



            foreach (var item in items)
            {
                MenuItem selectedItem;
                if (item is MenuItem)
                {
                    selectedItem = item as MenuItem;

                    selectedItem.IsEnabled = selectedItem.Header.ToString() != selectedOrderItem.StatusOrder.NameStatus;
                }
            }

        }

        private void OrderProductDG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid selectedDataGrid = sender as DataGrid;

            ProductGroup productOrder = selectedDataGrid?.SelectedItem as ProductGroup;

            if (productOrder == null) return;

            int productID = Convert.ToInt32(productOrder.ProductOrder.ProductID);

            if (productID == 0)
            {
                new MessageWin("Данный товар удалён", MessageCode.Info).ShowDialog();
                return;
            }


            GlobalVarriabels.FrontFrame.Navigate(new PreviewProductAddPage(this, productID, false));
            selectedDataGrid.SelectedValue = null;

        }

        private void CheckStaff_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!GlobalVarriabels.isDepWorker && selectedOrderItem?.Staff != null)
                GlobalVarriabels.FrontFrame.Navigate(new StaffInfoPage(selectedOrderItem?.Staff));

        }

        private async void CancelOrderBTN_Click(object sender, RoutedEventArgs e)
        {
            DBEntities.NullContext();

            try
            {
                Order OrderStatus = OrderLV.SelectedItem as Order;

                var CancelOrder = DBEntities.GetContext().Order.FirstOrDefault(u => u.OrderID == OrderStatus.OrderID);


                if (CancelOrder == null) return;

                if (CancelOrder.StatusOrderID == 1 || CancelOrder.StatusOrderID == 2)
                {
                    UpdateList();
                    GlobalVarriabels.MainWindow.SetAmountOrder();
                    return;
                }




                CancelOrderPage cancelOrderPage = new CancelOrderPage(OrderStatus);

                GlobalVarriabels.FrontFrame.Navigate(cancelOrderPage);

                while (!cancelOrderPage.Confirmed)
                {
                    if (cancelOrderPage.Cancel) return;

                    await Task.Delay(50);
                }

                if (CancelOrder.StaffID == null && GlobalVarriabels.currentRoleName != GlobalVarriabels.RoleName.Client)
                    CancelOrder.StaffID = GlobalVarriabels.currentUserID;

                CancelOrder.StatusOrderID = 2;
                CancelOrder.ShellLife = null;
                CancelOrder.CancelDescription = cancelOrderPage.CancelDescription;

                var ProductOfOrder = DBEntities.GetContext().ProductGroup.Where(u => u.OrderID == CancelOrder.OrderID);


                foreach (var product in ProductOfOrder)
                {
                    if (product.ProductOrder?.ProductID == null) continue;

                    var StorageItemChange = DBEntities.GetContext().Storage.FirstOrDefault(u => u.DepartamentID == CancelOrder.DepartamentCompanyID && u.ProductID == product.ProductOrder.ProductID);

                    if (StorageItemChange == null) continue;
                    StorageItemChange.Amount += product.Amount;

                }


                DBEntities.GetContext().SaveChanges();

                UpdateList();
                GlobalVarriabels.MainWindow.SetAmountOrder();
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка смены статуса", ex, MessageCode.Error).ShowDialog();
            }


        }

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private async void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            UpdateList();

            GlobalVarriabels.MainWindow.SetAmountBuscket();
            GlobalVarriabels.MainWindow.SetAmountOrder();
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
