using System.Net;
using System.Net.Mail;

namespace sstocker.core.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toEmail, string toEmailDisplayName, string subject, string body)
        {
            var mailMessage = new MailMessage(GetFromMailAddress(), new MailAddress(toEmail, toEmailDisplayName));
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;

            GetSmtpClient().Send(mailMessage);
        }

        private static MailAddress GetFromMailAddress()
        {
            var fromEmail = ConfigurationHelper.GetConfiguration("FromEmail");
            var fromEmailDisplayName = ConfigurationHelper.GetConfiguration("FromEmailDisplayName");
            return new MailAddress(fromEmail, fromEmailDisplayName);
        }

        private static SmtpClient GetSmtpClient()
        {
            var fromEmail = ConfigurationHelper.GetConfiguration("FromEmail");
            var fromEmailPassword = ConfigurationHelper.GetConfiguration("FromEmailPassword");

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
                EnableSsl = true,
            };
            return smtpClient;
        }
    }
}
