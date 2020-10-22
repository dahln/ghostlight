using template.Server.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace template.Server.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _options;

        public EmailSender(EmailConfiguration options)
        {
            _options = options;
        }
        
        async public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(_options.SmtpFromName, _options.SmtpFromEmail));
            mail.To.Add(new MailboxAddress(email, email));
            mail.Bcc.Add(new MailboxAddress(_options.SmtpFromEmail, _options.SmtpFromEmail));
            mail.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            bodyBuilder.TextBody = message;
            mail.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {

                    if (_options.SmtpStartTLS)
                    {
                        client.Connect(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.StartTls);
                    }
                    else
                    {
                        client.Connect(_options.SmtpServer, _options.SmtpPort);
                    }

                    client.Authenticate(_options.SmtpUsername, _options.SmtpPassword);

                    await client.SendAsync(mail);
                    client.Disconnect(true);
                }
            }
            catch
            {
            }

            return;
        }
    }
}
