using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.SharedKernal;

namespace Facets.Infrastructure.NotificationServices;

public sealed class EmailService : IEmailService
{
    private readonly IQueueService _queueService;

    public EmailService(IQueueService queueService)
    {
        _queueService = queueService;
    }

    public async Task<string> GetEmailTemplate(string emailTemplateName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, @$"EmailTemplates/{emailTemplateName}");

        using var reader = new StreamReader(path);

        string rawHTML = await reader.ReadToEndAsync();
        return rawHTML;
    }

    public async Task SendEmailByQueue(EmailModel email)
    {
        await _queueService.Enqueue(AppConstants.QueueStorage.QueueName.EmailQueue, email);
    }
}
