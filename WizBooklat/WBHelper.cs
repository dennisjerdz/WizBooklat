using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace WizBooklat
{
    public static class WBHelper
    {
        public static void SendEmail(string fromName, string subject, string body, string email, string firstName)
        {
            Task.Run(() =>
            {
                MailAddress fromAddress = new MailAddress("wizbooklat@gmail.com", fromName);
                MailAddress toAddress = new MailAddress(email, firstName);
                string fromPassword = "wb$123456";

                SmtpClient smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };

                MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                message.CC.Add(new MailAddress(email));
                smtp.Send(message);
            });
        }
    }
}