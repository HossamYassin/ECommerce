using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ECommerce.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            // If email settings are not configured, log and return (for development/testing)
            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer) || 
                string.IsNullOrWhiteSpace(_emailSettings.FromEmail))
            {
                _logger.LogWarning(
                    "Email not sent to {To}. Email settings not configured. Subject: {Subject}", 
                    to, subject);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            // Determine if body is HTML or plain text
            var bodyBuilder = new BodyBuilder();
            if (body.Contains("<!DOCTYPE html>") || body.Contains("<html>"))
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            // Connect to SMTP server
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, 
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, 
                cancellationToken);

            // Authenticate if credentials are provided
            if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUsername))
            {
                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword, cancellationToken);
            }

            // Send email
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}. Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}. Subject: {Subject}", to, subject);
            throw;
        }
    }
}

