using Facets.Core.Common.Dtos;

namespace Facets.Core.Common.Interfaces;

public interface ISMSService
{
    Task SendSMSByQueue(SMSMessage sms);
}
