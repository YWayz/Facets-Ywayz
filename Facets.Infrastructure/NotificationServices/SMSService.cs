using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.SharedKernal;

namespace Facets.Infrastructure.NotificationServices;

internal sealed class SMSService : ISMSService
{
    private readonly IQueueService _queueService;

    public SMSService(IQueueService queueService)
    {
        _queueService = queueService;
    }

    public async Task SendSMSByQueue(SMSMessage sms)
    {
        await _queueService.Enqueue(AppConstants.QueueStorage.QueueName.SMSQueue, sms);
    }
}
