using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.PageFolder.OrderFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.CatalogFolder
{
    /// <summary>
    /// Логика взаимодействия для PreviewProductAddPage.xaml
    /// </summary>
    public partial class PreviewProductAddPage : Page
    {
        int productID;
        object listFrom;
        int MaxAmount = 0;
        decimal PriceForOne = 0;

        bool outOfStock = false;

        public PreviewProductAddPage(object listFrom, int productID, bool isBuy)
        {
            InitializeComponent();

            this.productID = productID;
            this.listFrom = listFrom;

            //AmountTB.Text = "1";

            if (!isBuy)
            {
                ErrorLB.Visibility = Visibility.Collapsed;
                BottomPanel.Visibility = Visibility.Collapsed;
            }


            Visibility = Visibility.Hidden;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DBEntities.NullContext();



                Product product = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == productID);

                if (product == null)
                {
                    throw new Exception("Не удалось загрузить товар");
                }

                DataContext = product;


                PriceForOne = product.Price;
                PriceTBL.Text = PriceForOne.ToString() + " ₽";

                var inStorage = DBEntities.GetContext().Storage.FirstOrDefault(u => u.ProductID == productID && u.DepartamentID == GlobalVarriabels.curDepCompanyID);

                if (inStorage == null)
                {
                    throw new Exception("Не удалось загрузить товар со склада");
                }

                MaxAmount = inStorage.Amount;

                DepAmountTBL.Text = $"Кол. в отделе: {inStorage.Amount} штук";



                AmountTB.Text = "1";

                if (MaxAmount == 0)
                {
                    ErrorLB.Text = "Товар закончился.";

                    AmountTB.IsEnabled = false;
                    IncrementBTN.IsEnabled = false;
                    DecrementBTN.IsEnabled = false;
                    AddToPurchaseBTN.IsEnabled = false;

                    outOfStock = true;
                }

            }
            catch (Exception ex)
            {

                new MessageWin("Ошибка выгрузки данных", ex, MessageCode.Error).ShowDialog();





                GlobalVarriabels.FrontFrame.FrameErrorBack();

                if (listFrom is OrderListPage)
                    GlobalVarriabels.MainWindow.orderListPage.UpdateList();
                else if (listFrom is CatalogListPage)
                {
                    if (GlobalVarriabels.isDepWorker)
                        GlobalVarriabels.MainWindow.catalogListPage.UpdateStaffList();
                    else
                        GlobalVarriabels.MainWindow.catalogListPage.UpdateClientList();
                }


                return;
            }


            Visibility = Visibility.Visible;
        }

        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await GlobalVarriabels.FrontFrame.AnimWinClose();

            if (outOfStock)
            {
                if (listFrom is OrderListPage)
                    GlobalVarriabels.MainWindow.orderListPage.UpdateList();
                else if (listFrom is CatalogListPage)
                {
                    if (GlobalVarriabels.isDepWorker)
                        GlobalVarriabels.MainWindow.catalogListPage.UpdateStaffList();
                    else
                        GlobalVarriabels.MainWindow.catalogListPage.UpdateClientList();
                }
            }
        }

        private void AmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.OnlyNumsTB();
        }



        private void AmountBTN_Click(object sender, RoutedEventArgs e)
        {


            int num = 0;


            if (!int.TryParse(AmountTB.Text, out num)) return;


            if ((sender as Button).Name == "IncrementBTN")
            {
                if (num == 999 || MaxAmount <= num) return;

                AmountTB.Text = (++num).ToString();
            }
            else
            {
                if (num == 1) return;

                AmountTB.Text = (--num).ToString();
            }

            PriceTBL.Text = PriceForOne * num + " ₽";


        }

        private void AmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(AmountTB.Text)) return;

            int num = 0;

            if (!int.TryParse(AmountTB.Text, out num)) return;



            if (num < 1)
            {
                AmountTB.Text = "1";
                AmountTB.CaretIndex = AmountTB.Text.Length;
            }
            else
            {
                if (num > MaxAmount && MaxAmount > 0)
                {
                    AmountTB.Text = MaxAmount.ToString();
                    AmountTB.CaretIndex = AmountTB.Text.Length;
                }
                else if (num > 999)
                {
                    AmountTB.Text = Convert.ToString(999);
                    AmountTB.CaretIndex = AmountTB.Text.Length;
                }
            }



            PriceTBL.Text = PriceForOne * Convert.ToInt32(AmountTB.Text) + " ₽";
            ErrorLB.Text = "";

            IncrementBTN.IsEnabled = !(num == 999 || MaxAmount <= num);
            DecrementBTN.IsEnabled = !(num <= 1);

        }

        private async void AddToPurchaseBTN_Click(object sender, RoutedEventArgs e)
        {
            DBEntities.NullContext();
            Keyboard.ClearFocus();

            int amount;
            outOfStock = false;

            try
            {
                var inStorage = DBEntities.GetContext().Storage.FirstOrDefault(u => u.ProductID == productID && u.DepartamentID == GlobalVarriabels.curDepCompanyID);

                MaxAmount = inStorage.Amount;
                DepAmountTBL.Text = $"Кол. в отделе: {MaxAmount} штук";

                amount = Convert.ToInt32(AmountTB.Text);

                if (MaxAmount == 0)
                {
                    ErrorLB.Text = "Товар закончился.";

                    AmountTB.IsEnabled = false;
                    IncrementBTN.IsEnabled = false;
                    DecrementBTN.IsEnabled = false;
                    AddToPurchaseBTN.IsEnabled = false;

                    outOfStock = true;
                    return;
                }

                if (amount > MaxAmount)
                {
                    ErrorLB.Text = "Количество товаров в отеде изменилось.\nИзмените количество.";
                    return;
                }


                Product newProduct = DBEntities.GetContext().Product.FirstOrDefault(u => u.ProductID == productID);

                Busket busket;

                if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                    busket = DBEntities.GetContext().Busket
                        .FirstOrDefault(u => u.ProductID == productID && u.ClientID == GlobalVarriabels.currentUserID);
                else
                    busket = DBEntities.GetContext().Busket
                        .FirstOrDefault(u => u.ProductID == productID && u.StaffID == GlobalVarriabels.currentUserID);




                if (busket == null)
                {


                    busket = new Busket();

                    busket.ProductID = newProduct.ProductID;
                    busket.Amount = amount;

                    if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                        busket.ClientID = GlobalVarriabels.currentUserID;
                    else
                        busket.StaffID = GlobalVarriabels.currentUserID;

                    DBEntities.GetContext().Busket.Add(busket);
                }
                else
                {
                    busket.Amount += amount;
                }


                if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                    busket.ClientID = GlobalVarriabels.currentUserID;
                else
                    busket.StaffID = GlobalVarriabels.currentUserID;



                DBEntities.GetContext().SaveChanges();


                await GlobalVarriabels.FrontFrame.AnimWinClose();

                GlobalVarriabels.MainWindow.SetAmountBuscket();
                GlobalVarriabels.MainWindow.SetAmountOrder();
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки данных", ex, MessageCode.Error).ShowDialog();
            }
        }

        private void AmountTB_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(AmountTB.Text))
            {
                AmountTB.Text = "1";
            }

        }


    }
}
