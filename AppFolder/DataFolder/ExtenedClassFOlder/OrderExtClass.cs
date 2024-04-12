using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Windows;

namespace GoncharovVympelSale.AppFolder.DataFolder
{
    public partial class Order
    {

        private bool needClientSeparator;

        public bool GridIsEnabled { get; set; }


        public void SetGridVisibility()
        {
            if (StatusOrderID == 1 || StatusOrderID == 2)
                GridIsEnabled = true;
        }


        public Visibility CancelButtonVisibility => StatusOrderID == 2 || StatusOrderID == 1 || GlobalVarriabels.isReadOnly
            ? Visibility.Collapsed : Visibility.Visible;



        public Visibility TextIssueVibility => OrderIssue == null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility TextShellLifeVibility => ShellLife == null ? Visibility.Collapsed : Visibility.Visible;


        public Visibility ExpanderCancelVisbiility => StatusOrderID == 2
            ? Visibility.Visible : Visibility.Collapsed;

        public Visibility NumDepVisibility => GlobalVarriabels.isDepWorker || GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client
            ? Visibility.Collapsed : Visibility.Visible;

        public Visibility StaffVisibility => GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client
            ? Visibility.Collapsed : Visibility.Visible;

        public Visibility TypeOfIssueVisibility => TypeOfIssueD == null
            ? Visibility.Collapsed : Visibility.Visible;


        public Visibility AdresClientVisibiliy
        {
            get
            {
                needClientSeparator = true;
                return DeliveryAdress != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility PhoneEmailVisibility
        {
            get
            {
                if((TypeOfIssueD == 1 || TypeOfIssueD == 3 )
                   && GlobalVarriabels.currentRoleName != GlobalVarriabels.RoleName.Client)
                {
                    needClientSeparator = true;
                    return Visibility.Visible;
                }
                needClientSeparator = false;
                return Visibility.Collapsed;
            }
        }


        public Visibility VisibilityDateIssue
        {
            get
            {
                if (TypeOfIssueD == 2)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public Visibility DepAdresClientVisibiliy => GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client && TypeOfIssueD == 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;


        public Visibility VisibilitySeparatorDataClient
        {
            get
            {
                if (needClientSeparator)
                {
                   return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }


        public string TextForStaff
        {
            get
            {
                if (StaffID == null && StatusOrderID == 5)
                {
                    return "Сотрудник ещё не выбран: ";
                }
                if (StaffID == null)
                {
                    return "Сотрудник удалён или не был выбран - ";
                }


                return StatusOrderID == 2 || StatusOrderID == 1 ? "Обслужил: " : "Обслуживает: ";
            }
        }

        public string TextOfOrderIssue => StatusOrderID == 2
            ? "Дата отмены: " : "Дата выдачи: ";


    }
}
