using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.DataFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.WinFolder;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoncharovVympelSale.AppFolder.PageFolder.AdditionalFolder
{
    /// <summary>
    /// Логика взаимодействия для QuestionPage.xaml
    /// </summary>
    public partial class QuestionPage : Page
    {

        private MainWin mainWin;
        private bool islogOut;



        public bool isYes = false;
        public bool isNo = false;

        public QuestionPage(string CaptionName ,string question, MainWin mainWin, bool islogOut)
        {
            InitializeComponent();


            CaptionLB.Content = CaptionName;
            QuestionTBl.Text = question;
            this.mainWin = mainWin;
            this.islogOut = islogOut;

        }


        //ListView listView;

        public QuestionPage(string CaptionName, string Question)
        {
            InitializeComponent();


            QuestionTBl.Text = Question;
            CaptionLB.Content = CaptionName;
            //this.listView = listView;
        }





        private async void YesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (mainWin != null)
            {

                GlobalVarriabels.isReadOnly = false;
                await mainWin.AnimWinClose();

                if (islogOut)
                {
                    if (GlobalVarriabels.currentRoleName == GlobalVarriabels.RoleName.Client)
                    {
                        try
                        {
                            var setPC = DBEntities.GetContext().ClientEnterPC.Where(u => u.ClientID == GlobalVarriabels.currentUserID && u.NamePC == GlobalVarriabels.currentClientPC);

                            foreach (var item in setPC)
                            {
                                item.NamePC = null;
                            }


                            DBEntities.GetContext().SaveChanges();
                        }
                        catch { }

                    }

                    new AuthorizationWin().Show();
                    mainWin.Close();
                    return;
                }

                App.Current.Shutdown();

            }
            else
            {
                isYes = true;
                await GlobalVarriabels.FrontFrame.AnimWinClose();
            }

            
        }



        private async void NoBTN_Click(object sender, RoutedEventArgs e)
        {
            if (mainWin != null)
            {
                await GlobalVarriabels.FrontFrame.AnimWinClose();
            }
            else
            {
                isYes = false;
                await GlobalVarriabels.FrontFrame.AnimWinClose();
            }
        }






        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            Focus();

            if (e.Key == Key.Enter)
            {
                YesBTN_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {

                NoBTN_Click(null, null);

            }
        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            Focus();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
        }
    }
}
