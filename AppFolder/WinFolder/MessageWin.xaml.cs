using GoncharovVympelSale.AppFolder.ClassFolder;
using GoncharovVympelSale.AppFolder.ResourceFolder.ClassFolder;
using System;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace GoncharovVympelSale.AppFolder.WinFolder
{
    /// <summary>
    /// Логика взаимодействия для MessageWin.xaml
    /// </summary>
    public partial class MessageWin : Window
    {

        //private readonly int messageCode;
        Exception exception;


        public enum MessageCode
        {
            Error,
            Info,
            Question,
        }

        private MessageCode messageCode;


        public MessageWin(string titleText, Exception exception, MessageCode messageCode)
        {
            InitializeComponent();

            DataContext = this;
            this.exception = exception;


            Owner = GlobalVarriabels.MainWindow;


            Title = titleText;
            this.messageCode = messageCode;


            WhatException(exception);
        }



        public MessageWin(Exception exception, MessageCode messageCode)
        {
            InitializeComponent();

            DataContext = this;
            this.exception = exception;

            Owner = GlobalVarriabels.MainWindow;
            this.messageCode = messageCode;

            WhatException(exception);
        }

        public MessageWin(string titleText, string messageText, MessageCode messageCode)
        {
            InitializeComponent();

            DataContext = this;
            MessageTBl.ContextMenu.Visibility = Visibility.Hidden;

            Owner = GlobalVarriabels.MainWindow;
            

            Title = titleText;
            MessageTBl.Text = messageText;
            this.messageCode = messageCode;
        }

        public MessageWin(string messageText, MessageCode messageCode)
        {
            InitializeComponent();

            DataContext = this;

            Owner = GlobalVarriabels.MainWindow;
            MessageTBl.ContextMenu.Visibility = Visibility.Hidden;

            MessageTBl.Text = messageText;
            this.messageCode = messageCode;
        }



        private void WhatException(Exception exception)
        {

            if (exception is EntityException)
            {

                EntityException entityException = exception as EntityException;

                MessageTBl.Text = entityException.Message;
            }
            else if (exception is DbUpdateException)
            {
                DbUpdateException entityException = exception as DbUpdateException;

                MessageTBl.Text = entityException.Message;
            }
            else
            {
                MessageTBl.Text = exception.Message;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            switch (messageCode)
            {
                case MessageCode.Error:
                    {
                        SystemSounds.Hand.Play();
                        if (Title == "") Title = "Ошибка";

                    }
                    break;

                case MessageCode.Info:
                    {
                        SystemSounds.Exclamation.Play();

                        if (Title == "") Title = "Информация";
                    }
                    break;
                case MessageCode.Question:
                    {
                        SystemSounds.Question.Play();
                        if (Title == "") Title = "Вопрос";

                        QuestionButtonsSP.Visibility = Visibility.Visible;
                        OkBTN.Visibility = Visibility.Hidden;
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

        private async void CloseWinBTN_Click(object sender, RoutedEventArgs e)
        {
            await this.AnimWinClose();
            Close();
        }

        private async void OkBTN_Click(object sender, RoutedEventArgs e)
        {


            await this.AnimWinClose();
            Close();
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(exception.ToString());
        }

        private async void YesBTN_Click(object sender, RoutedEventArgs e)
        {
            await this.AnimWinClose();
            DialogResult = true;
            Close();
        }

        private async void NoBTN_Click(object sender, RoutedEventArgs e)
        {
            await this.AnimWinClose();
            DialogResult = false;
            Close();
        }
    }
}
