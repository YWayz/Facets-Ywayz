using Facets.Core.Events.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Events.Interfaces;

public interface IEventSettingsService
{
    Task<ResponseResult> UpdatePaymentSettings(Guid eventId, UpdatePaymentSettingsDto model, CancellationToken token);
    Task<ResponseResult<PaymentSettingsDto>> GetPaymentSetting(Guid eventId, CancellationToken token);
}
