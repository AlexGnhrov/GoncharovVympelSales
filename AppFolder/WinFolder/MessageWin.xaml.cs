using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoncharovCarPartsAS.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для MessageWin.xaml
    /// </summary>
    public partial class MessageWin : Window
    {
        private readonly string titleText;
        private readonly string messageText;
        private readonly Exception exception;
        private readonly int messageCode;


        public MessageWin(string titleText, Exception exception,int messageCode)
        {
            InitializeComponent();

            this.titleText = titleText;
            this.exception = exception;
            this.messageCode = messageCode;
        }

        public MessageWin(string titleText,string messageText ,int messageCode)
        {
            InitializeComponent();

            this.titleText = titleText;
            this.messageText = messageText;
            this.messageCode = messageCode;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (messageCode)
            {
                case 0:
                    {
                        SystemSounds.Hand.Play();

                        Title = titleText == "" || titleText == null ? "Ошибка" : titleText;


                    }
                    break;

                case 1:
                    {
                        SystemSounds.Exclamation.Play();

                        Title = titleText == "" || titleText == null ? "Информация" : titleText;



                    }
                    break;
                case 2:
                    {

                        SystemSounds.Question.Play();
                        Title = titleText == "" || titleText == null ? "Вопрос" : titleText;

                    }
                    break;

                default:
                    Title = "Неизвестный код";
                    break;
            }


  
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWinBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
