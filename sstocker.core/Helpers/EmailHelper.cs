using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace sstocker.core.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toEmail, string toEmailDisplayName, string subject, string body, List<(string, string)> images)
        {
            var mailMessage = new MailMessage(GetFromMailAddress(), new MailAddress(toEmail, toEmailDisplayName));
            mailMessage.Subject = subject;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;

            if (images != null && images.Any())
            {
                mailMessage.AlternateViews.Add(GetView(body, images));
            }
            else
            {
                mailMessage.Body = body;
            }

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

        private static AlternateView GetView(string body, List<(string, string)> images)
        {
            var av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

            foreach (var image in images)
            {
                string contentType;

                switch (Path.GetExtension(image.Item2))
                {
                    case ".jpg":
                    case ".jpeg":
                        contentType = MediaTypeNames.Image.Jpeg;
                        break;
                    case ".gif":
                        contentType = MediaTypeNames.Image.Gif;
                        break;
                    default:
                        throw new Exception($"Unrecognized extension type: {Path.GetExtension(image.Item2)}");
                }

                var res = new LinkedResource(image.Item2, contentType);
                res.ContentId = image.Item1;
                av.LinkedResources.Add(res);
            }

            return av;
        }
    }
}
