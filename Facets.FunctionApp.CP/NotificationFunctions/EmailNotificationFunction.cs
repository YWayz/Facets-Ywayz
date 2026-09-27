using Facets.FunctionApp.CP.NotificationFunctions.Models;
using Facets.SharedKernal;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Facets.FunctionApp.CP.NotificationFunctions;

public sealed class EmailNotificationFunction
{
    private readonly ILogger _logger;

    public EmailNotificationFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmailNotificationFunction>();
    }

    [Function(nameof(SendEmailFunc))]
    public async Task SendEmailFunc([QueueTrigger(AppConstants.QueueStorage.QueueName.EmailQueue)] EmailMessage emailMessage)
    {
        try
        {
            _logger.LogInformation($"Sending email to: {MaskEmail(emailMessage.To)} | Subject: {emailMessage.Subject}");
            // SMTP credentials come from Function App settings, never from source code.
            string from = RequiredSetting("Smtp_From");
            string fromPassword = RequiredSetting("Smtp_Password");
            string host = Environment.GetEnvironmentVariable("Smtp_Host") is { Length: > 0 } h ? h : "smtp.gmail.com";
            int port = int.TryParse(Environment.GetEnvironmentVariable("Smtp_Port"), out int p) ? p : 587;
            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(emailMessage.To));
            message.Subject = emailMessage.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = emailMessage.Body
            };

            // Add attachments
            foreach (var attachment in emailMessage.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(from, fromPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation($"Sent email to: {MaskEmail(emailMessage.To)} | Subject: {emailMessage.Subject}");
        }
        catch (Exception ex)
        {
            string message = $"""
            Failed sending email to: {MaskEmail(emailMessage.To)} | Subject: {emailMessage.Subject} |
            Error Msg: {ex.InnerException?.Message ?? ex.Message}
            """;
            _logger.LogError(message);
            throw;
        }
    }

    private static string RequiredSetting(string name) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"Function App setting '{name}' is missing. Add it under Settings > Environment variables. See DEPLOYMENT.md.");

    // Logs go to Application Insights and are kept for months; keep personal data out of them.
    private static string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "(none)";
        int at = email.IndexOf('@');
        if (at <= 1) return "***" + (at >= 0 ? email[at..] : string.Empty);
        return email[0] + "***" + email[(at - 1)..];
    }
}
