using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoncharovVympelSale.AppFolder.PageFolder.OrderFolder.AdditionalPage
{
    /// <summary>
    /// Логика взаимодействия для CancelOrderPage.xaml
    /// </summary>
    public partial class CancelOrderPage : Page
    {

        public string CancelDescription;
        public bool Confirmed = false;
        public bool Cancel = false;


        public CancelOrderPage(Order order)
        {
            InitializeComponent();

            NamePageLB.Content = $"Отмена заказа (№{order.UQnum})";
        }

        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cancel = true;
            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }



        private async void ConfirmBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(CancelDescriptionTB.Text))
                {
                    ErrorLB.Text = "Укажите причину";
                    CancelDescriptionTB.Tag = GlobalVarriabels.ErrorTag;
                    return;
                }


                CancelDescription = CancelDescriptionTB.Text.Trim();

                await GlobalVarriabels.FrontFrame.AnimWinClose();

                Confirmed = true;

            }
            catch (Exception ex)
            {
                ErrorLB.Text = ex.Message;
            }
        }

        private void CancelDescriptionTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLB.Text = "";
            CancelDescriptionTB.Tag = null;
        }
    }
}
