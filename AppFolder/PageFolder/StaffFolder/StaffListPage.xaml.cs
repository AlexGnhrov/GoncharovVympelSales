using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder;
using GoncharovVympelSale.AppFolder.PageFolder.StaffFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using static GoncharovVympelSale.AppFolder.WinFolder.MessageWin;

namespace GoncharovVympelSale.AppFolder.PageFolder
{
    /// <summary>
    /// Логика взаимодействия для StaffListPage.xaml
    /// </summary>
    /// 

    public partial class StaffListPage : Page
    {
        public DispatcherTimer timerForUpdate;

        public bool isFromDepList = false;
        public bool isStatusInit = false;

        Staff selectedItem;


        public StaffListPage()
        {
            InitializeComponent();

            StafListDG.Items.IsLiveSorting = true;


            timerForUpdate = new DispatcherTimer();
            timerForUpdate.Interval = TimeSpan.FromSeconds(0.3);
            timerForUpdate.Tick += Timer_Tick;

            if (GlobalVarriabels.isDepWorker)
            {
                BackDepBTN.Visibility = Visibility.Collapsed;
                GridDepBox.Visibility = Visibility.Collapsed;
            }

            if (GlobalVarriabels.isReadOnly)
            {
                AddStaffBTN.Visibility = Visibility.Collapsed;

                foreach (MenuItem item in ContextMenu.Items.OfType<MenuItem>())
                {

                    if (!(item.Name == "UpdateListMI" || item.Name == "InfoStaffOfDepMI"))
                        item.Visibility = Visibility.Collapsed;
                }

                int i = 0;

                foreach (Separator item in ContextMenu.Items.OfType<Separator>())
                {
                    if (i != 0)
                        item.Visibility = Visibility.Collapsed;

                    ++i;
                }
            }


            LoadComboBox();
            UpdateList();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateList();
            timerForUpdate.Stop();
        }

        public void LoadComboBox()
        {
            int? selectedValueDepo;

            selectedValueDepo = Convert.ToInt32(DepoCompanyCB.SelectedValue);

            try
            {


                DepoCompanyCB.ItemsSource = DBEntities.GetContext().DepartamentCompany.ToList();

                if (!isStatusInit)
                    StatusCB.ItemsSource = DBEntities.GetContext().StatusUser.ToList();


                DepoCompanyCB.SelectedValue = selectedValueDepo;


                isStatusInit = true;
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка выгрузки комбобокса", ex, MessageCode.Error).ShowDialog();
            }
        }



        public void UpdateList()
        {
            GlobalVarriabels.MainWindow.Cursor = Cursors.Wait;

            IQueryable<Staff> source = null;


            int? selectedValueDep = Convert.ToInt32(DepoCompanyCB.SelectedValue);
            int? selectedValueStatus = Convert.ToInt32(StatusCB.SelectedValue);

            try
            {


                source = DBEntities.GetContext().Staff.Where(u =>
                       u.Surname.Contains(SearchTB.Text)
                    || u.Name.Contains(SearchTB.Text)
                    || u.Patronymic.Contains(SearchTB.Text)
                    || u.Email.Contains(SearchTB.Text)
                    || u.PhoneNum.Contains(SearchTB.Text)
                    || u.User.Login.Contains(SearchTB.Text));

                if (GlobalVarriabels.isDepWorker)
                    source = source.Where(u => u.DepartamentID == GlobalVarriabels.curDepCompanyID);

                source = selectedValueDep != 0 ? source.Where(u => u.DepartamentID == selectedValueDep) : source;
                source = selectedValueStatus != 0 ? source.Where(u => u.StatusID == selectedValueStatus) : source;
                source = CanDeleteChB.IsChecked == true ? source.Where(u => u.BlockDate <= DateTime.Now || u.BlockDate == null) : source;



                StafListDG.ItemsSource = source.OrderBy(u => u.DepartamentID).ThenBy(u => u.User.RoleID).ThenBy(u => u.StatusID).ToList();

                MessageListBorder.Visibility = StafListDG.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;


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
            timerForUpdate.Stop();
            timerForUpdate.Start();


            ClearLB.Visibility = string.IsNullOrWhiteSpace(SearchTB.Text) ? Visibility.Hidden : Visibility.Visible;
        }

        private void ClearCheckLB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Label).Name == "ClearStatusLB")
            {
                StatusCB.SelectedValue = null;
            }
            else
            {
                DepoCompanyCB.SelectedValue = null;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timerForUpdate.Stop();
            timerForUpdate.Start();

            CanDeleteChB.Visibility = Convert.ToInt32(StatusCB.SelectedValue) == 3 ? Visibility.Visible : Visibility.Collapsed;

            if (Convert.ToInt32(StatusCB.SelectedValue) != 3)
                CanDeleteChB.IsChecked = false;

            ClearCheckLB.Visibility = DepoCompanyCB.SelectedValue != null && !isFromDepList ? Visibility.Visible : Visibility.Hidden;
            ClearStatusLB.Visibility = StatusCB.SelectedValue != null ? Visibility.Visible : Visibility.Hidden;
        }

        private void BackDepBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddStaffBTN_Click(object sender, RoutedEventArgs e)
        {
            GlobalVarriabels.FrontFrame.Navigate(new AEStaffPage(this, 0));
        }



        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = StafListDG.SelectedItem as Staff;


            switch ((sender as MenuItem).Name)
            {

                case "UpdateListMI":
                    {
                        DBEntities.NullContext();

                        LoadComboBox();
                        UpdateList();
                    }
                    break;
                case "EditMI":
                    {
                        GlobalVarriabels.FrontFrame.Navigate(new AEStaffPage(this, selectedItem.StaffID));
                    }
                    break;
                case "InfoStaffOfDepMI":
                    {
                        GlobalVarriabels.FrontFrame.Navigate(new StaffInfoPage(selectedItem));
                    }
                    break;
                case "WorkingStatusMI":
                    {
                        ChangeStatus(1, false);
                    }
                    break;
                case "VacationStatusMI":
                    {
                        ChangeStatus(2, false);
                    }
                    break;
                case "StoptedStatusMI":
                    {
                        ChangeStatus(3, false);
                    }
                    break;
                case "DeleteMI":
                    {
                        RemoveStaff();
                    }
                    break;
            }



        }

        private async void RemoveStaff()
        {
            string message = "";
            QuestionPage questionPage;

            try
            {

                DBEntities.NullContext();

                Staff removeStaff = DBEntities.GetContext().Staff.FirstOrDefault(u=>u.StaffID == selectedItem.StaffID);

                if (removeStaff?.StatusID != 3)
                {
                    message = "Чтобы удалить сотрудника, нужно чтобы был статус \"Заблокирован\" и пройти с момента блокировки полгода\n\n";
                    message += "Хотите сменить статус на \"Заблокирован\"?";


                    questionPage = new QuestionPage("Смена статуса", message);

                    GlobalVarriabels.FrontFrame.Navigate(questionPage);

                    while (!questionPage.isYes)
                    {
                        if (questionPage.isNo) return;

                        await Task.Delay(50);
                    }

                    ChangeStatus(3, true);

                    return;
                }




                if (removeStaff?.BlockDate > DateTime.Now)
                {

                    message = "Сотрудника можно будет удалить: " + selectedItem.BlockDate;


                    MessageWin messageWin = new MessageWin(message, MessageCode.Info);
                    messageWin.ShowDialog();


                    return;
                }



                message = "Вы действительно хотите удалить сотрудника?";
                questionPage = new QuestionPage("Удаление сотрудника", message);

                GlobalVarriabels.FrontFrame.Navigate(questionPage);

                while (!questionPage.isYes)
                {
                    if (questionPage.isNo) return;

                    await Task.Delay(50);
                }


                if (removeStaff == null)
                {
                    UpdateList();
                    throw new Exception("Сотрудник уже удалён.");
                }

                Passport removePassport = DBEntities.GetContext().Passport.FirstOrDefault(u => u.PassportID == removeStaff.PassportID);
                Adress removeAdresPassport = DBEntities.GetContext().Adress.FirstOrDefault(u => u.AdressID == removePassport.AdressRegID);
                EditStaff LogStaff = DBEntities.GetContext().EditStaff.FirstOrDefault(u => u.StaffID == removeStaff.StaffID);
                User user = DBEntities.GetContext().User.FirstOrDefault(u => u.UserID == removeStaff.UserID);


                DBEntities.GetContext().Staff.Remove(removeStaff);
                DBEntities.GetContext().Passport.Remove(removePassport);
                DBEntities.GetContext().Adress.Remove(removeAdresPassport);
                DBEntities.GetContext().User.Remove(user);

                if (LogStaff != null)
                    DBEntities.GetContext().EditStaff.Remove(LogStaff);

                DBEntities.GetContext().SaveChanges();

                UpdateList();

            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка при удалении сотрудника", ex, MessageCode.Error).ShowDialog();
            }
        }

        private async void ChangeStatus(int statusID, bool ignoremessages)
        {
            try
            {

                if (!ignoremessages)
                {
                    string message = "";


                    if (statusID == 3)
                    {
                        message += "При статусе \"Заблокирован\", сотрудник не сможет войти в свою запись." +
                            "\nЧерез полгода можно будет удалить запись.\n\n";
                    }

                    if (selectedItem.StatusID == 3 && statusID != 3)
                    {
                        message += "Сменив статуc с \"Заблокирован\", сброситься дата возможности удаления.\n\n";
                    }

                    message += "Вы действительно хотите сменить статус сотрудника?";

                    QuestionPage questionPage = new QuestionPage("Смена статуса", message);

                    GlobalVarriabels.FrontFrame.Navigate(questionPage);

                    while (!questionPage.isYes)
                    {
                        if (questionPage.isNo) return;

                        await Task.Delay(50);
                    }
                }

                Staff editStaff = DBEntities.GetContext().Staff.
                    FirstOrDefault(u => u.StaffID == selectedItem.StaffID);



                editStaff.StatusID = statusID;

                editStaff.BlockDate = statusID == 3 ? (DateTime?)DateTime.Now.AddDays(180) : null;

                EditStaff LogStaff = DBEntities.GetContext().EditStaff.FirstOrDefault(u => u.StaffID == editStaff.StaffID);

                if (LogStaff == null)
                {
                    LogStaff = new EditStaff();

                    LogStaff.StaffID = editStaff.StaffID;

                    DBEntities.GetContext().EditStaff.Add(LogStaff);
                }

                LogStaff.WhoEditStaffID = GlobalVarriabels.currentUserID;
                LogStaff.DateEdit = Convert.ToDateTime(DateTime.Now);


                DBEntities.GetContext().SaveChanges();

                UpdateList();
            }
            catch (Exception ex)
            {
                new MessageWin("Ошибка смены статуса", ex, MessageCode.Error).ShowDialog();
            }
        }



        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {


            EditMI.IsEnabled = false;
            InfoStaffOfDepMI.IsEnabled = false;
            StatusMI.IsEnabled = false;

            WorkingStatusMI.IsEnabled = false;
            VacationStatusMI.IsEnabled = false;
            StoptedStatusMI.IsEnabled = false;

            DeleteMI.IsEnabled = false;

            selectedItem = StafListDG.SelectedItem as Staff;

            if (selectedItem != null)
            {
                if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.MainAdmin)
                {
                    if (selectedItem.StaffID == GlobalVarriabels.currentUserID && selectedItem.StaffID != 1
                        || selectedItem.StaffID == 1 && GlobalVarriabels.currentUserID != 1)
                    {
                        EditMI.IsEnabled = false;
                        InfoStaffOfDepMI.IsEnabled = false;

                        StatusMI.IsEnabled = false;

                        DeleteMI.IsEnabled = false;
                    }
                    else
                    {
                        EditMI.IsEnabled = true;
                        InfoStaffOfDepMI.IsEnabled = true;


                        if (selectedItem.StaffID == 1)
                        {
                            StatusMI.IsEnabled = false;
                            DeleteMI.IsEnabled = false;
                        }
                        else
                        {
                            StatusMI.IsEnabled = true;
                            DeleteMI.IsEnabled = true;
                        }
                    }
                }
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.MainManager)
                {
                    InfoStaffOfDepMI.IsEnabled = selectedItem.User.RoleID != (int)GlobalVarriabels.RoleName.MainAdmin;
                }
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepAdmin)
                {
                    if (selectedItem.User.RoleID == (int)GlobalVarriabels.RoleName.MainAdmin ||
                       selectedItem.User.RoleID == (int)GlobalVarriabels.RoleName.MainManager ||
                        selectedItem.User.RoleID == (int)GlobalVarriabels.currentRoleName ||
                        selectedItem.StaffID == GlobalVarriabels.currentUserID)
                    {
                        EditMI.IsEnabled = false;
                        InfoStaffOfDepMI.IsEnabled = false;
                        StatusMI.IsEnabled = false;
                        DeleteMI.IsEnabled = false;
                    }
                    else
                    {
                        EditMI.IsEnabled = true;
                        InfoStaffOfDepMI.IsEnabled = true;
                        StatusMI.IsEnabled = true;
                        DeleteMI.IsEnabled = true;
                    }
                }
                else if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepManager)
                {
                    InfoStaffOfDepMI.IsEnabled =
                        selectedItem.User.RoleID != (int)GlobalVarriabels.RoleName.MainAdmin &&
                        selectedItem.User.RoleID != (int)GlobalVarriabels.RoleName.MainManager &&
                        selectedItem.User.RoleID != (int)GlobalVarriabels.RoleName.DepAdmin;
                }


                WorkingStatusMI.IsEnabled = !(selectedItem.StatusID == 1);
                VacationStatusMI.IsEnabled = !(selectedItem.StatusID == 2);
                StoptedStatusMI.IsEnabled = !(selectedItem.StatusID == 3);



            }
        }

        private void CanDeleteChB_Click(object sender, RoutedEventArgs e)
        {
            timerForUpdate.Stop();
            timerForUpdate.Start();
        }

        private void StafListDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (StafListDG.SelectedItem != null)
            //    GlobalVarriabels.FrontFrame.Navigate(new StaffInfoPage(StafListDG.SelectedItem as Staff));
        }
    }
}
