using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.GlobalClassFolder;
using GoncharovVympelSale.AppFolder.PageFolder;
using GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder;
using GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder;
using GoncharovVympelSale.AppFolder.PageFolder.ClientFolder;
using GoncharovVympelSale.AppFolder.PageFolder.DeportamentFolder;
using GoncharovVympelSale.AppFolder.PageFolder.OrderFolder;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GoncharovVympelSale.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {



        private readonly BitmapImage LeftArrowImage = new BitmapImage(new Uri("pack://application:,,,/GoncharovVympelSale;component/AppFolder/ResourceFolder/ImageFolder/IconForMenuButton/ShrinkArrowIcon1.png"));
        private readonly BitmapImage RightArrowImage = new BitmapImage(new Uri("pack://application:,,,/GoncharovVympelSale;component/AppFolder/ResourceFolder/ImageFolder/IconForMenuButton/ShrinkArrowIcon.png"));

        private readonly double maxWidthMenu = 205;
        private readonly double minWidthMenu = 70;


        private bool isInt = false;

        DispatcherTimer TimerSetContextNull;

        public MainWin(Staff staff, Client client)
        {

            InitializeComponent();

            GlobalVarriabels.MainWindow = this;
            GlobalVarriabels.FrontFrame = FrontFrame;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            this.LoadSettings();


            FrontFrame.Visibility = Visibility.Visible;

            TimerSetContextNull = new DispatcherTimer();
            TimerSetContextNull.Interval = TimeSpan.FromMinutes(5);
            TimerSetContextNull.Tick += Timer_Tick;


            if (Properties.Settings.Default.LeftMenuShrinked)
            {
                ShrinkBorderArrowIm.Source = LeftArrowImage;
                LeftMenuBorder.Width = minWidthMenu;
            }
            else
            {
                ShrinkBorderArrowIm.Source = RightArrowImage;
                LeftMenuBorder.Width = maxWidthMenu;
            }

            Tag = "";

            if (staff != null)
                UserSettingsBTN.Visibility = Visibility.Collapsed;

            switch (GlobalVarriabels.currentRoleName)
            {
                case GlobalVarriabels.RoleName.MainAdmin:
                    {
                        CartBTN.Visibility = Visibility.Collapsed;

                        deportamentListPage = new DeportamentListPage(staffListPage);
                        MainFrame.Navigate(deportamentListPage);
                    }
                    break;
                case GlobalVarriabels.RoleName.MainManager:
                    {
                        GlobalVarriabels.isReadOnly = true;
                        CartBTN.Visibility = Visibility.Collapsed;


                        deportamentListPage = new DeportamentListPage(staffListPage);
                        MainFrame.Navigate(deportamentListPage);
                    }
                    break;
                case GlobalVarriabels.RoleName.DepAdmin:
                    {
                        DeportamenListBtn.Visibility = Visibility.Collapsed;
                        CartBTN.Visibility = Visibility.Collapsed;

                        staffListPage = new StaffListPage();
                        MainFrame.Navigate(staffListPage);

                    }
                    break;
                case GlobalVarriabels.RoleName.DepManager:
                    {
                        DeportamenListBtn.Visibility = Visibility.Collapsed;
                        CartBTN.Visibility = Visibility.Collapsed;

                        staffListPage = new StaffListPage();
                        MainFrame.Navigate(staffListPage);
                        GlobalVarriabels.isReadOnly = true;

                    }
                    break;
                case GlobalVarriabels.RoleName.DepWork:
                    {
                        DeportamenListBtn.Visibility = Visibility.Collapsed;
                        StaffListBtn.Visibility = Visibility.Collapsed;

                        if (catalogListPage == null)
                            catalogListPage = new CatalogListPage();

                        MainFrame.Navigate(catalogListPage);

                    }
                    break;
                case GlobalVarriabels.RoleName.Client:
                    {
                        DeportamenListBtn.Visibility = Visibility.Collapsed;
                        StaffListBtn.Visibility = Visibility.Collapsed;
                        ClientListBtn.Visibility = Visibility.Collapsed;

                        MenuSeparator.Visibility = Visibility.Collapsed;



                        if (catalogListPage == null)
                            catalogListPage = new CatalogListPage();

                        MainFrame.Navigate(catalogListPage);
                    }
                    break;

            }

            TimerSetContextNull.Start();



            if (staff != null)
            {
                DataContext = staff;
                PhotoUserIB.ImageSource = PhotoImageClass.GetImageFromBytes(staff.Photo);
            }
            else
            {
                StaffDetails.Visibility = Visibility.Collapsed;

                setClientContext(client);


                UserPicture.Cursor = Cursors.Hand;
            }
        }

        public void setClientContext(Client client)
        {

            DataContext = null;
            DataContext = client;

            if (client.Photo != null)
                PhotoUserIB.ImageSource = PhotoImageClass.GetImageFromBytes(client.Photo);
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            DBEntities.NullContext();
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }







        private void WinClickBTN_Click(object sender, RoutedEventArgs e)
        {


            switch ((sender as Button).Name)
            {
                case "HideWinBTN":
                    WindowState = WindowState.Minimized;
                    break;
                case "FullScreenBTN":
                    WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                    break;
                case "CloseWinBTN":
                    string question = "Вы действительно хотите закрыть приложение?";

                    FrontFrame.Focus();
                    FrontFrame.Navigate(new QuestionPage("Выход", question, this, false));
                    break;
            }

        }



        public Button usingBtn = null;


        public DeportamentListPage deportamentListPage;
        public StaffListPage staffListPage;
        public ClientListPage clientListPage;
        public CatalogListPage catalogListPage;
        public OrderListPage orderListPage;


        private void MenuButtons_Click(object sender, RoutedEventArgs e)
        {


            Button selectedBtn = sender as Button;


            if (usingBtn != null && (selectedBtn.Content == usingBtn.Content)) return;



            switch (selectedBtn.Name)
            {
                case "DeportamenListBtn":
                    {

                        if (deportamentListPage == null)
                            deportamentListPage = new DeportamentListPage(staffListPage);



                        MainFrame.Navigate(deportamentListPage);
                    }
                    break;
                case "StaffListBtn":
                    {
                        if (staffListPage == null) staffListPage = new StaffListPage();


                        staffListPage.BackDepBTN.Visibility = Visibility.Collapsed;

                        if (staffListPage.isFromDepList)
                        {
                            staffListPage.DepoCompanyCB.SelectedValue = null;
                            staffListPage.DepoCompanyCB.IsEnabled = true;
                            staffListPage.ClearCheckLB.Visibility = Visibility.Hidden;
                            staffListPage.isFromDepList = false;
                            staffListPage.Title = "Список сотрудников";
                        }

                        MainFrame.Navigate(staffListPage);
                    }
                    break;
                case "ClientListBtn":
                    {
                        if (clientListPage == null) clientListPage = new ClientListPage();

                        MainFrame.Navigate(clientListPage);
                    }
                    break;

                case "CatalogListBtn":
                    {
                        if (catalogListPage == null) catalogListPage = new CatalogListPage();


                        MainFrame.Navigate(catalogListPage);
                    }
                    break;
                case "OrderListBtn":
                    {
                        if (orderListPage == null) orderListPage = new OrderListPage();


                        MainFrame.Navigate(orderListPage);
                    }

                    break;


            }

        }




        private void ShrinkBorderArrowIm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation columnAnimation = new DoubleAnimation();


            PowerEase elasticEase = new PowerEase();

            elasticEase.EasingMode = EasingMode.EaseInOut;

            elasticEase.Power = 3;

            TimeSpan time = TimeSpan.FromSeconds(0.4);

            if (LeftMenuBorder.Width == minWidthMenu)
            {
                ShrinkBorderArrowIm.Source = RightArrowImage;


                columnAnimation.To = maxWidthMenu;
                columnAnimation.Duration = time;



                Properties.Settings.Default.LeftMenuShrinked = false;
            }
            else
            {
                ShrinkBorderArrowIm.Source = LeftArrowImage;

                columnAnimation.To = minWidthMenu;
                columnAnimation.Duration = time;


                Properties.Settings.Default.LeftMenuShrinked = true;
            }



            columnAnimation.EasingFunction = elasticEase;


            LeftMenuBorder.BeginAnimation(Border.WidthProperty, columnAnimation);

            Properties.Settings.Default.Save();

        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            string question = "Вы действительно хотите сменить запись?";


            FrontFrame.Navigate(new QuestionPage("Сменить запись", question, this, true));


        }




        private void FrontFrame_Navigated(object sender, NavigationEventArgs e)
        {

            if (!(e.Content is null))
            {
                WinBorder.IsEnabled = false;
                LeftToolBarSP.IsEnabled = false;
                CloseWinBTN.IsEnabled = false;
            }


            FrontFrame.IsEnabled = false;
            FrontFrame.IsEnabled = true;



        }

        private void FrontFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
            {
                e.Handled = true;
            }
        }


        private void LeftMenuBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                ShrinkBorderArrowIm_MouseLeftButtonUp(sender, e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {


            switch (MainFrame.Content)
            {
                case DeportamentListPage _:
                    setActiveMenuButton(DeportamenListBtn);
                    break;
                case StaffListPage _:
                    setActiveMenuButton(StaffListBtn);
                    break;

                case ClientListPage _:
                    setActiveMenuButton(ClientListBtn);
                    break;
                case CatalogListPage _:
                    setActiveMenuButton(CatalogListBtn);
                    break;
                case OrderListPage _:
                    setActiveMenuButton(OrderListBtn);
                    break;
            }


            SetAmountBuscket();
            SetAmountOrder();

        }


        public void SetAmountBuscket()
        {
            try
            {

                if (!CartBTN.IsVisible) return;

                var buscketCount = GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client
                    ? DBEntities.GetContext().Busket.Where(u => u.ClientID == GlobalVarriabels.currentUserID).Count()
                    : DBEntities.GetContext().Busket.Where(u => u.StaffID == GlobalVarriabels.currentUserID).Count();


                TextBlock textbloc = CartBTN.Template.FindName("CartCountTBL", CartBTN) as TextBlock;



                if (buscketCount > 99)
                {
                    textbloc.Text = "+99";
                    OrderCountTBL.Width = textbloc.Width = 25;
                }
                else if (buscketCount == 0)
                {
                    textbloc.Text = "";
                    textbloc.Width = 0;
                }
                else
                {

                    textbloc.Text = buscketCount.ToString();


                    if (buscketCount > 10)
                        textbloc.Width = 17.5;
                    else
                        textbloc.Width = 20;

                }

            }
            catch { }
        }

        public void SetAmountOrder()
        {
            try
            {

                var orderCount = GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client
                    ? DBEntities.GetContext().Order.Where(u => u.StatusOrderID != 1 && u.StatusOrderID != 2 && u.ClientID == GlobalVarriabels.currentUserID).Count()
                    : DBEntities.GetContext().Order.Where(u => u.StatusOrderID != 1 && u.StatusOrderID != 2 && u.DepartamentCompanyID == GlobalVarriabels.curDepCompanyID).Count();



                TextBlock textbloc = OrderBTN.Template.FindName("OrderCountTBL", OrderBTN) as TextBlock;


                if (orderCount > 99)
                {
                    OrderCountTBL.Text = textbloc.Text = "+99";
                    OrderCountTBL.Width = textbloc.Width = 25;
                }
                else if (orderCount == 0)
                {
                    OrderCountTBL.Text = textbloc.Text = "";
                    OrderCountTBL.Width = textbloc.Width = 0;
                }
                else
                {

                    OrderCountTBL.Text = textbloc.Text = orderCount.ToString();

                    if (orderCount > 10)
                        OrderCountTBL.Width = textbloc.Width = 17.5;
                    else
                        OrderCountTBL.Width = textbloc.Width = 20;

                }




            }
            catch { }
        }


        private void setActiveMenuButton(Button selectedBTN)
        {
            if (usingBtn != null)
            {
                usingBtn.Tag = GlobalVarriabels.NotUsingBtnTag;
            }

            usingBtn = selectedBTN;
            usingBtn.Tag = GlobalVarriabels.UsingBtnTag;
        }

        private void MainLogo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (catalogListPage == null)
                catalogListPage = new CatalogListPage();

            MainFrame.Navigate(catalogListPage);
        }


        private void OrderBTN_Click(object sender, RoutedEventArgs e)
        {
            if (orderListPage == null)
            {
                orderListPage = new OrderListPage();
            }
            MainFrame.Navigate(orderListPage);
        }

        PurchaseListPage purchaseListPage;

        private void CartBTN_Click(object sender, RoutedEventArgs e)
        {
            if (purchaseListPage?.orderDone == true)
            {
                purchaseListPage = null;
            }

            if (purchaseListPage == null)
            {
                purchaseListPage = new PurchaseListPage();
            }

            FrontFrame.Navigate(purchaseListPage);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(100);

            //isInt = true;

        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (UserPicture.Cursor == Cursors.Hand)
            {
                FrontFrame.Navigate(new EditUserPage());
            }
        }

        private void UserSettings_Click(object sender, RoutedEventArgs e)
        {

            FrontFrame.Navigate(new EditUserPage());
        }
    }
}
