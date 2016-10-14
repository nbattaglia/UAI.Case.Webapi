using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAI.Case.Domain.Common;

namespace UAI.Case.Utilities
{
    public static class Mailer
    {
        public static bool SendActivateRequest(Usuario usuario, string currentUrl)
        {
            //TODO: Factorizar ok
            try
            {
                
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("uai.case.registros@gmail.com", "3l3c720n");



                MailMessage mm = new MailMessage(
                    "uai.case.registros@gmail.com", usuario.Mail, "UAI CASE - Activar cuenta",
                    "Su usuario es: " + usuario.Mail +
                    ", Su passwod temporal es: " + usuario.Password +
                    " haga click <a href='http://" + currentUrl + "?id=" + usuario.Id + "&activate=" + UAI.Case.Security.Cryptography.MD5Hash(usuario.RegisterToken) + "'>aquí</a> para activar su cuenta ");


                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                return true;

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw new Exception(e.Message,e);
            }
        }

        public static Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service
            return Task.FromResult(0);
        }

        public static bool SendRegistrationRequest(string currentUrl, string registerToken, string mail)
        {
            //TODO: Factorizar
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("uai.case.registros@gmail.com", "3l3c720n");



                MailMessage mm = new MailMessage(
                    "uai.case.registros@gmail.com", mail, "UAI CASE - Registrarse",
                                        
                    " haga click <a href='http://" + currentUrl + "?register=" + registerToken + "'>aquí</a> para registrarse");


                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                return true;

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public static void Send()
        {

        }
    }
}
