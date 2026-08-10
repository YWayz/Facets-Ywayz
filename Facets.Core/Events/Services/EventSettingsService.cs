using Facets.Core.Common.Validators;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Interfaces;
using Facets.Core.Events.Specs;
using Facets.Core.Events.Validators;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace Facets.Core.Events.Services;

internal sealed class EventSettingsService : IEventSettingsService
{
    private readonly IEventRepository _eventRepository;
    private readonly IModelValidator _validator;
    private readonly IMemoryCache _memoryCache;

    public EventSettingsService(IEventRepository eventRepository, IModelValidator validator, IMemoryCache memoryCache)
    {
        _eventRepository = eventRepository;
        _validator = validator;
        _memoryCache = memoryCache;
    }

    public async Task<ResponseResult> UpdatePaymentSettings(Guid eventId, UpdatePaymentSettingsDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdatePaymentSettingsDtoValidator, UpdatePaymentSettingsDto>(model, token);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var @event = await _eventRepository.GetEventBySpec(new EventPaymentSettingsUpdateSpec(eventId), token, asTracking: true);

        if (@event is null) return new(new NotFoundException(nameof(eventId), "Event", eventId));

        @event.SetPaymentSettings(model.OnSitePayingMode, model.PayLaterForOnlineRegistration);

        await _eventRepository.SaveChangesAsync(token);

        _memoryCache.Remove(eventId);

        return new();
    }

    public async Task<ResponseResult<PaymentSettingsDto>> GetPaymentSetting(Guid eventId, CancellationToken token)
    {
        var paymentSetting = await _memoryCache
                                    .GetOrCreateAsync(eventId,
                                                      entry =>
                                                      {
                                                          entry.SetAbsoluteExpiration(TimeSpan.FromDays(3));
                                                          return _eventRepository.GetProjectedEventBySpec(new EventPaymentSettingsSpec(eventId),
                                                                                                          token);
                                                      });

        if (paymentSetting is null) return new(new NotFoundException(nameof(eventId), "Event Settings", eventId));

        return new(paymentSetting);
    }
}
