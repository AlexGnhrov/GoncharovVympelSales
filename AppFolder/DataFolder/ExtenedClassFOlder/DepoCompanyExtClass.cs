using GoncharovVympelSale.AppFolder.ClassFolder;
using System.Linq;

namespace GoncharovVympelSale.AppFolder.DataFolder
{
    public partial class DepartamentCompany
    {
        public string AmoutOfRoles
        {
            get
            {
                string text = "";

                var staff = DBEntities.GetContext().Staff.Where(u => u.DepartamentID == DepartamentID);



                text += "Глав. Админ - " + staff.Where(u => u.RoleID == (int)GlobalVarriabels.RoleName.MainAdmin).ToArray().Length.ToString();
                text += "\nГлав. Менеджер - " + staff.Where(u => u.RoleID == (int)GlobalVarriabels.RoleName.MainManager).ToArray().Length.ToString();
                text += "\n\nАдмин - " + staff.Where(u => u.RoleID == (int)GlobalVarriabels.RoleName.DepAdmin).ToArray().Length.ToString();
                text += "\nМенеджер - " + staff.Where(u => u.RoleID == (int)GlobalVarriabels.RoleName.DepManager).ToArray().Length.ToString();
                text += "\nСотрудников - " + staff.Where(u => u.RoleID == (int)GlobalVarriabels.RoleName.DepWork).ToArray().Length.ToString();


                return text;
            }
        }

        public string AmountOfStaff
        {
            get
            {
                return "Общее количество - " + DBEntities.GetContext().Staff.Where(u => u.DepartamentID == DepartamentID).ToArray().Length.ToString();
            }
        }

    }
}
