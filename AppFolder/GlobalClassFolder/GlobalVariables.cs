using GoncharovVympelSale.AppFolder.WinFolder;
using System.Windows;
using System.Windows.Controls;

namespace GoncharovVympelSale.AppFolder.ClassFolder
{
    class GlobalVarriabels
    {

        public static Frame FrontFrame;
        public static MainWin MainWindow;

        public static bool IsFrontFrameActive = false;
        public static bool isReadOnly = false;

        public static int currentUserID;
        public static int curDepCompanyID;


        public static readonly string ErrorTag = "InvalidData";
        public static readonly string PassTag = "PassData";

        public static readonly string UsingBtnTag = "Using";
        public static readonly string NotUsingBtnTag = "NoUse";

        public static readonly string IsEditBtnTag = "IsEdit";


        public static bool isDepWorker = false;
        public static bool isClient = false;
        public static string currentClientPC = "";

        public enum RoleName
        {
            MainAdmin = 1,
            MainManager = 2,
            DepAdmin = 3,
            DepManager = 4,
            DepWork = 5,
            Client = 6,

        }

        public static RoleName currentRoleName;



    }
}
