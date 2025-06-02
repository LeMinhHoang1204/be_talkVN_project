using System.Net;
using System.Net.Mail;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TalkVN.Application.Config;
using TalkVN.Application.Services.Interface;
namespace TalkVN.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    private readonly SMTPSettings _smtp;

    public EmailService(IConfiguration config, ILogger<EmailService> logger, IOptions<SMTPSettings> smtpOptions)
    {
        _config = config;
        _logger = logger;
        _smtp = smtpOptions.Value;
    }


    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.Log(LogLevel.Information, $"Sending email from {_smtp.From}");
        var message = new MailMessage();
        message.From = new MailAddress(_smtp.From);
        message.To.Add(to);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
