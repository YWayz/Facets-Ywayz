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

    //[Function(nameof(SendEmailFunc))]
    //public async Task SendEmailFunc([QueueTrigger(AppConstants.QueueStorage.QueueName.EmailQueue)] EmailMessage emailMessage)
    //{
    //    try
    //    {
    //        _logger.LogInformation($"Sending email to: {emailMessage.To} | Subject: {emailMessage.Subject}");

    //        string from = "donotreply@facetssrilanka.com";
    //        string fromPassword = "Facets36@";

    //        MimeMessage message = new();

    //        message.From.Add(MailboxAddress.Parse(from));

    //        message.To.Add(MailboxAddress.Parse(emailMessage.To));

    //        message.Subject = emailMessage.Subject;

    //        message.Body = new TextPart(TextFormat.Html)
    //        {
    //            Text = emailMessage.Body
    //        };

    //        using var client = new SmtpClient();

    //        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
    //        await client.AuthenticateAsync(from, fromPassword);

    //        await client.SendAsync(message);
    //        await client.DisconnectAsync(true);

    //        _logger.LogInformation($"Sent email to: {emailMessage.To} | Subject: {emailMessage.Subject}");
    //    }

    //    catch (Exception ex)
    //    {
    //        string message = $"""
    //            Failed sending email to: {emailMessage.To} | Subject: {emailMessage.Subject} |
    //            Error Msg: {ex.InnerException?.Message ?? ex.Message}
    //            """;
    //        _logger.LogError(message);

    //        throw;
    //    }
    //}
    [Function(nameof(SendEmailFunc))]
    public async Task SendEmailFunc([QueueTrigger(AppConstants.QueueStorage.QueueName.EmailQueue)] EmailMessage emailMessage)
    {
        try
        {
            _logger.LogInformation($"Sending email to: {emailMessage.To} | Subject: {emailMessage.Subject}");
            //string from = "donotreply@facetssrilanka.com";
            //string fromPassword = "Facets36@";
            string from = "donotreply@facetssrilanka.com";
            string fromPassword = "tzom qyhp tpyf rusx";
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
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(from, fromPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation($"Sent email to: {emailMessage.To} | Subject: {emailMessage.Subject}");
        }
        catch (Exception ex)
        {
            string message = $"""
            Failed sending email to: {emailMessage.To} | Subject: {emailMessage.Subject} |
            Error Msg: {ex.InnerException?.Message ?? ex.Message}
            """;
            _logger.LogError(message);
            throw;
        }
    }
}
