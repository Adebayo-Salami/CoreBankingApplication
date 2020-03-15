using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace CoreBankingApplication.Logic
{
    public class SendEmail
    {
        public void SendingEmail(string email, string password, string psd)
        {
            try
            {
                SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                server.EnableSsl = true;
                server.Credentials = new NetworkCredential("salamibolarinwa16@gmail.com", psd);

                MailMessage message = new MailMessage();
                message.From = new MailAddress("salamibolarinwa16@gmail.com", "SalamiCBA");
                message.Subject = "Salami CBA Login Credentials";
                message.To.Add(new MailAddress(email));
                message.IsBodyHtml = true;
                message.Body = "Your Username is: " + email + "\n Your Password is: " + password;

                server.Send(message);
            }
            catch (Exception)
            {
                Console.WriteLine("Error Sending mail");
            }
        }
    }
}