using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
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
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.ClientFolder
{
    /// <summary>
    /// Логика взаимодействия для ClientListPage.xaml
    /// </summary>
    public partial class ClientListPage : Page
    {
        public ClientListPage()
        {
            InitializeComponent();
            UpdateList();
        }


        public void UpdateList()
        {
            DBEntities.NullContext();

            try
            {

                ClientListDG.ItemsSource = DBEntities.GetContext().Client.Where(u => u.Name.Contains(SearchTB.Text) ||
                                                                                   u.Surname.Contains(SearchTB.Text) ||
                                                                                   u.PhoneNum.ToString().Contains(SearchTB.Text)).ToList();
            }
            catch (Exception ex)
            {

                new MessageWin("Ошибка выгрузки списка", ex, MessageCode.Error).ShowDialog();
            }
        }

        private void ClearLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchTB.Text = "";
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {

            UpdateList();

            ClearLB.Visibility = !string.IsNullOrWhiteSpace(SearchTB.Text) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
