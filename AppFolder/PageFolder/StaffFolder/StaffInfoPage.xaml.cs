using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.StaffFolder
{
    /// <summary>
    /// Логика взаимодействия для StaffInfoPage.xaml
    /// </summary>
    public partial class StaffInfoPage : Page
    {

        Staff staff;
        public StaffInfoPage(Staff staff)
        {
            InitializeComponent();

            DBEntities.NullContext();

            DataContext = DBEntities.GetContext().Staff.FirstOrDefault(u => u.StaffID == staff.StaffID); 
            this.staff = staff;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                EditStaff logStaff = DBEntities.GetContext().EditStaff.FirstOrDefault(u => u.StaffID == staff.StaffID);

                InfoStaffSP.DataContext = logStaff;
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка информаци записи", ex, MessageCode.Error).ShowDialog();
            }
        }

        private async void CloseLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await GlobalVarriabels.FrontFrame.AnimWinClose();
        }








    }
}
