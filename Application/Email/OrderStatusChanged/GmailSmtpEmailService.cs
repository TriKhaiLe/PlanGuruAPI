using System.Net;
using System.Net.Mail;
using Application.Email.Common;
using Microsoft.Extensions.Configuration;

namespace Application.Email.OrderStatusChanged;

public class GmailSmtpEmailService(IConfiguration configuration) : IEmailService
{
    private readonly string _username = configuration["Mail:Gmail:Username"]!;
    private readonly string _password = configuration["Mail:Gmail:Password"]!;

    public async Task SendMailAsync(EmailRequest request)
    {
        try
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };
        
            using (client)
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_username),
                    Subject = request.Subject,
                    Body = request.HtmlContent,
                    IsBodyHtml = true
                };

                foreach (var recipient in request.To)
                {
                    message.To.Add(recipient.Email);
                }
            
                using (message)
                {
                    await client.SendMailAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}