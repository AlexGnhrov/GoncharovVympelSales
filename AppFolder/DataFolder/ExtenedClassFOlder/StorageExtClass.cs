using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Windows;

namespace GoncharovVympelSale.AppFolder.DataFolder
{
    public partial class Storage
    {
        public Visibility ButtonEnable
        {
            get
            {
                return GlobalVarriabels.isReadOnly || GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepAdmin ? Visibility.Hidden : Visibility.Visible ;
            }
        }

        public Visibility EditButtonDepAdmin
        {
            get
            {
                return GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.DepAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool AddToCartEnable => Amount != 0;


    }
}
