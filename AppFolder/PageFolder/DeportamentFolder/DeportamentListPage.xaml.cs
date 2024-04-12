using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder.DeportamentFolder
{
    /// <summary>
    /// Логика взаимодействия для DeportamentListPage.xaml
    /// </summary>
    public partial class DeportamentListPage : Page
    {

        StaffListPage staffListPage;

        public DeportamentListPage(StaffListPage staffListPage)
        {
            InitializeComponent();

            this.staffListPage = staffListPage;


            if (GlobalVarriabels.isReadOnly)
            {
                addDepartamentBTN.Visibility = Visibility.Collapsed;

                foreach (MenuItem item in contextMenu.Items.OfType<MenuItem>())
                {

                    if (!(item.Name == "UpdateListMI" || item.Name == "InfoStaffOfDepMI"))
                        item.Visibility = Visibility.Collapsed;
                }

                int i = 0;

                foreach (Separator item in contextMenu.Items.OfType<Separator>())
                {
                    if (i != 0)
                        item.Visibility = Visibility.Collapsed;

                    ++i;
                }
            }


            UpdateList();
        }

        int? curentItem;

        public void UpdateList()
        {
            GlobalVarriabels.MainWindow.Cursor = Cursors.Wait;

            try
            {


                DepCompayListDG.ItemsSource = DBEntities.GetContext().DepartamentCompany.
                     Where(u => u.DepartamentID.ToString().Contains(SearchTB.Text) ||
                     u.Adress.Region.NameRegion.Contains(SearchTB.Text) ||
                     u.Adress.City.NameCity.Contains(SearchTB.Text) ||
                     u.Adress.Street.NameStreet.Contains(SearchTB.Text)).OrderBy(u => u.StatusDepartamentID).ToList();

                MessageListBorder.Visibility = DepCompayListDG.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки списка", ex, MessageCode.Error).ShowDialog();
            }
            finally
            {
                GlobalVarriabels.MainWindow.Cursor = Cursors.Arrow;
            }
        }


        private void ClearLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchTB.Text = "";
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {

            UpdateList();

            ClearLB.Visibility = string.IsNullOrWhiteSpace(SearchTB.Text) ? Visibility.Hidden : Visibility.Visible;
        }

        private void DepCompayListDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DepCompayListDG.SelectedItem == null) return;

            GoStaffoFDep();
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            departamentCompany = DepCompayListDG.SelectedItem as DepartamentCompany;


            switch ((sender as MenuItem).Name)
            {

                case "UpdateListMI":
                    {
                        DBEntities.NullContext();

                        UpdateList();
                    }
                    break;
                case "EditMI":
                    {
                        GlobalVarriabels.FrontFrame.Navigate(new AEDeportamenPage(this, departamentCompany.DepartamentID));
                    }
                    break;
                case "InfoStaffOfDepMI":
                    {
                        GoStaffoFDep();
                    }
                    break;
                case "WorkingStatusMI":
                    {
                        ChangeStatus(1);
                    }
                    break;
                case "StoptedStatusMI":
                    {
                        ChangeStatus(2);
                    }
                    break;
                case "DeleteMI":
                    {
                        RemoveDeportament();
                    }
                    break;
            }



        }



        private async void RemoveDeportament()
        {
            try
            {

                DepartamentCompany editDeportamentCompany = DBEntities.GetContext().DepartamentCompany.
                    FirstOrDefault(u => u.DepartamentID == departamentCompany.DepartamentID);


                Order checkDep = DBEntities.GetContext().Order.FirstOrDefault(u => u.DepartamentCompanyID == editDeportamentCompany.DepartamentID);
                Staff checkStaff = DBEntities.GetContext().Staff.FirstOrDefault(u => u.DepartamentID == editDeportamentCompany.DepartamentID);


                if (checkDep != null)
                {
                    new MessageWin("Удаление департамента", "Данный депортамент находиться в заказах.\nУдаление его невозможно.", MessageCode.Info).ShowDialog();

                    return;
                }
                if (checkStaff != null)
                {
                    new MessageWin("Удаление департамента",
                                   "К данному департаменту привязаны сотрудники.\nПереместите сотрудников в другой отдел",
                                   MessageCode.Info).ShowDialog();

                    return;
                }



                QuestionPage questionPage = new QuestionPage("Удалить", "Вы действительно хотите удалить данный департамент");

                GlobalVarriabels.FrontFrame.Navigate(questionPage);

                while (!questionPage.isYes)
                {
                    if (questionPage.isNo) return;

                    await Task.Delay(50);
                }



                editDeportamentCompany = DBEntities.GetContext().DepartamentCompany.
                    FirstOrDefault(u => u.DepartamentID == departamentCompany.DepartamentID);

                if(editDeportamentCompany == null)
                {
                    UpdateList();
                    throw new Exception("Данный депортамент уже удалён.");
                }


                var DepoAdress = DBEntities.GetContext().Adress.FirstOrDefault(u => u.AdressID == editDeportamentCompany.AdressID);
                var client = DBEntities.GetContext().Client.Where(u => u.SelectedDepID == editDeportamentCompany.DepartamentID).ToArray();

                foreach (var item in client)
                {
                    item.SelectedDepID = null;
                }

                DBEntities.GetContext().Adress.Remove(DepoAdress);
                DBEntities.GetContext().DepartamentCompany.Remove(editDeportamentCompany);

                DBEntities.GetContext().SaveChanges();

                UpdateList();
            }

            catch (Exception ex)
            {
                new MessageWin("Ошибка удаления", ex, MessageCode.Error).ShowDialog();
            }
        }

        private async void ChangeStatus(int statusID)
        {
            try
            {
                string message = "";


                if (statusID == 2)
                {
                    message += "При статусе \"Приостановлено\", клиентам не будет доступен самовывоз или покупка в департаменте.\n\n";
                }

                message += "Вы действительно хотите сменить статус департамента?";


                QuestionPage questionPage = new QuestionPage("Смена статуса", message);

                GlobalVarriabels.FrontFrame.Navigate(questionPage);

                while (!questionPage.isYes)
                {
                    if (questionPage.isNo) return;

                    await Task.Delay(50);
                }
                departamentCompany = DepCompayListDG.SelectedItem as DepartamentCompany;


                DepartamentCompany editDeportamentCompany = DBEntities.GetContext().DepartamentCompany.
                    FirstOrDefault(u => u.DepartamentID == departamentCompany.DepartamentID);

                editDeportamentCompany.StatusDepartamentID = statusID;

                DBEntities.GetContext().SaveChanges();

                UpdateList();
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка смены статуса", ex, MessageCode.Error).ShowDialog();
            }
        }

        DepartamentCompany departamentCompany;
        private void GoStaffoFDep()
        {

            departamentCompany = DepCompayListDG.SelectedItem as DepartamentCompany;

            if (staffListPage == null) staffListPage = new StaffListPage();


            staffListPage.Title = $"Список сотрудников (Отдел №{departamentCompany.DepartamentID})";

            staffListPage.DepoCompanyCB.ItemsSource = DBEntities.GetContext().DepartamentCompany.ToList();
            staffListPage.DepoCompanyCB.SelectedValue = Convert.ToInt32(departamentCompany.DepartamentID);

            staffListPage.BackDepBTN.Visibility = Visibility.Visible;
            staffListPage.DepoCompanyCB.IsEnabled = false;
            staffListPage.ClearCheckLB.Visibility = Visibility.Hidden;
            staffListPage.isFromDepList = true;

            NavigationService.Navigate(staffListPage);
        }




        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            EditMI.IsEnabled = false;
            InfoStaffOfDepMI.IsEnabled = false;
            StatusMI.IsEnabled = false;

            WorkingStatusMI.IsEnabled = false;
            StoptedStatusMI.IsEnabled = false;

            DeleteMI.IsEnabled = false;


            if (DepCompayListDG.SelectedItem != null)
            {
                EditMI.IsEnabled = true;
                InfoStaffOfDepMI.IsEnabled = true;
                StatusMI.IsEnabled = true;
                DeleteMI.IsEnabled = true;


                departamentCompany = DepCompayListDG.SelectedItem as DepartamentCompany;

                if (departamentCompany.StatusDepartamentID == 2)
                {
                    WorkingStatusMI.IsEnabled = true;
                }
                else
                {
                    StoptedStatusMI.IsEnabled = true;
                }

            }
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            GlobalVarriabels.FrontFrame.Navigate(new AEDeportamenPage(this, 0));
        }
    }
}
