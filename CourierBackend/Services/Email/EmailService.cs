using CourierBackend.Configurations.Services;
using CourierBackend.Services.Email.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CourierBackend.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail
            )
        );

        message.To.Add(MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);

        await client.AuthenticateAsync(
            _settings.Username,
            _settings.Password,
            cancellationToken
        );

        await client.SendAsync(
            message,
            cancellationToken
        );

        await client.DisconnectAsync(
            true,
            cancellationToken
        );

        _logger.LogInformation(
            "Email sent successfully to {Email}",
            to
        );
    }
}