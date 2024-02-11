using GoncharovCarPartsAS.AppFolder.ClassFolder;
using GoncharovCarPartsAS.AppFolder.PageFolder;
using GoncharovCarPartsAS.AppFolder.PageFolder.AdditionalFolder;
using GoncharovCarPartsAS.AppFolder.ResourceFolder.ClassFolder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;


namespace GoncharovCarPartsAS.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {


        private readonly string VKlink = "https://vk.me/club156570033";
        private readonly string TGlink = "https://t.me/autorus_bot";

        private readonly BitmapImage LeftArrowImage = new BitmapImage(new Uri("pack://application:,,,/GoncharovCarPartsAS;component/AppFolder/ResourceFolder/ImageFolder/IconForMenuButton/ShrinkArrowIcon1.png"));
        private readonly BitmapImage RightArrowImage = new BitmapImage(new Uri("pack://application:,,,/GoncharovCarPartsAS;component/AppFolder/ResourceFolder/ImageFolder/IconForMenuButton/ShrinkArrowIcon.png"));

        private readonly double maxWidthMenu = 205;
        private readonly double minWidthMenu = 70;

        public MainWin()
        {
            InitializeComponent();


            FrontFrame.Visibility = Visibility.Visible;

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

            //MainFrame.Navigate(new StaffListPage());



        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }







        private async void WinClickBTN_Click(object sender, RoutedEventArgs e)
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

                    FrontFrame.Navigate(new QuestionPage(FrontFrame, question, this, false));
                    break;
            }

        }



        Button usingBtn = null;

        private void MenuButtons_Click(object sender, RoutedEventArgs e)
        {


            Button selectedBtn = sender as Button;


            if (usingBtn != null && (selectedBtn.Content == usingBtn.Content)) return;



            switch (selectedBtn.Name)
            {
                case "DeportamenListBtn":

                    //MainFrame.Navigate(new UserListPage(this, AddEditFrame));

                    break;
                case "StaffListBtn":

                    //MainFrame.Navigate(new MenuListPage(this, AddEditFrame));

                    break;
                case "ClientListBtn":

                    //MainFrame.Navigate(orderListPage);

                    break;

                case "CatalogListBtn":

                    //MainFrame.Navigate(orderListPage);

                    break;
                case "OrderListBtn":

                    //MainFrame.Navigate(orderListPage);

                    break;

                case "PurchCartListBtn":

                    //MainFrame.Navigate(orderListPage);

                    break;


            }

            if (usingBtn != null)
            {
                usingBtn.Tag = GlobalVarriabels.NotUsingBtnTag;
            }

            usingBtn = selectedBtn;
            usingBtn.Tag = GlobalVarriabels.UsingBtnTag;

            //UpdateUserData();
        }

        private void VKimageGO_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo(VKlink));
        }

        private void TGimageGO_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo(TGlink));
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void ShrinkBorderArrowIm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation columnAnimation = new DoubleAnimation();


            PowerEase elasticEase = new PowerEase();
            elasticEase.Power = 2.6;

            var time = TimeSpan.FromSeconds(0.386);

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

        private async void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            string question = "Вы действительно хотите сменить запись?";

            FrontFrame.Navigate(new QuestionPage(FrontFrame, question, this,true));

        }




        private void FrontFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
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
    }
}
