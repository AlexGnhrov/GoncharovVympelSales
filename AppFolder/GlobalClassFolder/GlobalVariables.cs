using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoncharovCarPartsAS.AppFolder.ClassFolder
{
    class GlobalVarriabels
    {
        public static int currentUserID;
        public static int? curDepCompanyID;


        public static readonly string ErrorTag = "InvalidData";
        public static readonly string PassTag = "PassData";

        public static readonly string UsingBtnTag = "Using";
        public static readonly string NotUsingBtnTag = "NoUse";



        public enum RoleName
        {
            MainAdmin = 1,
            MainManager,
            DepAdmin,
            DepManager,
            DepWork,
        }

        public static RoleName CurrentRoleID;


        public enum MessageCode
        {
            Error,
            Info,
            Question,

        }

    }
}
