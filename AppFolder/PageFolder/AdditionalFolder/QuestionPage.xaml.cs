using GoncharovCarPartsAS.AppFolder.ResourceFolder.ClassFolder;
using GoncharovCarPartsAS.AppFolder.WinFolder;
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

namespace GoncharovCarPartsAS.AppFolder.PageFolder.AdditionalFolder
{
    /// <summary>
    /// Логика взаимодействия для QuestionPage.xaml
    /// </summary>
    public partial class QuestionPage : Page
    {

        Frame frontFrame;
        Window window;
        bool islogOut;

        public QuestionPage(Frame frontFrame, string question, Window window, bool islogOut)
        {
            InitializeComponent();

            this.frontFrame = frontFrame;
            QuestionTBl.Text = question;

            this.window = window;
            this.islogOut = islogOut;

            //window.IsEnabled = false;


        }
        DataGrid dataGrid;

        public QuestionPage(Frame frontFrame, string Question, DataGrid dataGrid)
        {
            InitializeComponent();

            this.frontFrame = frontFrame;
            QuestionTBl.Text = QuestionTBl.Text;
            this.dataGrid = dataGrid;
        }

        ListView listView;

        public QuestionPage(Frame frontFrame, string Question, ListView listView)
        {
            InitializeComponent();

            this.frontFrame = frontFrame;
            QuestionTBl.Text = QuestionTBl.Text;
            this.listView = listView;
        }


        private async void YesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (window != null)
            {


                await window.AnimWinClose();

                if (islogOut)
                {
                    new AuthorizationWin().Show();
                    window.Close();
                    return;
                }

                App.Current.Shutdown();
                window.Close();

            }
        }



        private async void NoBTN_Click(object sender, RoutedEventArgs e)
        {
            await frontFrame.AnimWinClose();

            frontFrame.Navigate(null);
        }


    }
}
