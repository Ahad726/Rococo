using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rococo.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _client;
        private readonly string _senderEmail;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            var emailConfiguration = _configuration.GetSection("EmailConfig");
            _senderEmail = emailConfiguration.GetSection("Username").Value;

            _client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            _client.Connect(_configuration["EmailConfig:Host"],
                int.Parse(_configuration["EmailConfig:Port"]),
                bool.Parse(_configuration["EmailConfig:UseSSL"]));
            _client.Authenticate(_configuration["EmailConfig:Username"], _configuration["EmailConfig:Password"]);
        }


        public async Task SendEmailAsync(List<Message> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    var mail = new MimeMessage();
                    mail.From.Add(new MailboxAddress("Rococo", _senderEmail));
                    mail.To.Add(new MailboxAddress(string.Empty, message.Receiver));
                    mail.Subject = message.Subject;
                    mail.Body = new TextPart(TextFormat.Text)
                    {
                        Text = message.Body
                    };

                   await _client.SendAsync(mail);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
