using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GoncharovVympelSale.AppFolder.GlobalClassFolder
{
    class EmailClass
    {
        public static void sendMessage(string EmailToSend,string caption,string message)
        {

            try
            {
                Random ranCode = new Random();

                int EmailCode = ranCode.Next(100000, 999999);



                //DBEntities.GetContext().SaveChanges();

                // отправитель - устанавливаем адрес и отображаемое в письме имя
                //MailAddress fromEmail = new MailAddress("social.activity.supp@yandex.ru", "АвтоРусь");
                MailAddress fromEmail = new MailAddress("VympelSale@yandex.ru", "Вымпел продажи");
                MailAddress toEmail = new MailAddress($"{EmailToSend}");
                MailMessage messageEmail = new MailMessage(fromEmail, toEmail);

                messageEmail.Subject = caption;
                messageEmail.Body = message;

                // письмо представляет код html
                messageEmail.IsBodyHtml = true;

                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru",25);
                
                // логин и пароль
                smtp.EnableSsl = true;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("VympelSale", "sxqccgtsvkmcobqc");

                smtp.Send(messageEmail);
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
