using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using CollectIQ.Core.Models;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Services
{
    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAmazonSimpleEmailService _sesClient;

        public EmailManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAmazonSimpleEmailService sesClient)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _sesClient = sesClient;
        }

        public async Task CreatedAccountEmail(EmailInfo emailUser)
        {
            var pwd = GenerateToken();
            var subject = "Confirm Your Email";

            await SendEmailAsync(new EmailInfo
            {
                Sender = emailUser.Sender,
                Receiver = emailUser.Receiver,
                Subject = subject,
                HtmlContent = emailUser.HtmlContent
            });
        }

        public async Task SendEmailAsync(EmailInfo emailUser)
        {
            var sendRequest = new SendEmailRequest
            {
                Source = emailUser.Sender,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { emailUser.Receiver }
                },
                Message = new Message
                {
                    Subject = new Content(emailUser.Subject),
                    Body = new Body
                    {
                        Html = new Content(emailUser.HtmlContent)
                    }
                }
            };

            try
            {
                var response = await _sesClient.SendEmailAsync(sendRequest);
                Console.WriteLine("Email sent! Message ID: " + response.MessageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent. Error message: " + ex.Message);
                throw;
            }
        }

        public string GenerateToken()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
