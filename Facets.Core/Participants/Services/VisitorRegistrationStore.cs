using Facets.Core.Common.Interfaces;
using Facets.Core.Common.Validators;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Events.Interfaces;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Validators;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Services;

internal sealed class VisitorRegistrationStore : IVisitorRegistrationStore
{
    private readonly ILoggedInUserService _loggedInUser;
    private readonly IPassCategoryService _passCategoryService;
    private readonly IVisitorService _visitorService;
    private readonly IRegistrationCounterService _registrationCounterService;
    private readonly IVisitorRegistrationService _visitorRegistrationService;
    private readonly IUnitOfWork _uow;
    private readonly IModelValidator _validator;
    private readonly IEventService _eventService;
    private readonly IVisitorPavilionService _visitorPavilionService;
    private readonly IEventSettingsService _eventSettingsService;

    public VisitorRegistrationStore(ILoggedInUserService loggedInUser,
                                    IPassCategoryService passCategoryService,
                                    IVisitorService visitorService,
                                    IRegistrationCounterService registrationCounterService,
                                    IVisitorRegistrationService visitorRegistrationService,
                                    IUnitOfWork uow,
                                    IModelValidator validator,
                                    IEventService eventService,
                                    IVisitorPavilionService visitorPavilionService,
                                    IEventSettingsService eventSettingsService)
    {
        _loggedInUser = loggedInUser;
        _passCategoryService = passCategoryService;
        _visitorService = visitorService;
        _registrationCounterService = registrationCounterService;
        _visitorRegistrationService = visitorRegistrationService;
        _uow = uow;
        _validator = validator;
        _eventService = eventService;
        _visitorPavilionService = visitorPavilionService;
        _eventSettingsService = eventSettingsService;
    }

    public async Task<ResponseResult<RegisteredVisitorDto>> RegisterOnsiteVisitorToEvent(VisitorOnsiteEventRegistrationDto model,
                                                                                         CancellationToken cancellationToken)
    {

        var validationResult = await _validator.ValidateAsync<VisitorOnsiteEventRegistrationDtoValidator, VisitorOnsiteEventRegistrationDto>
                                                              (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var registrationResponse = await RegisterToEventDates();

        if (registrationResponse.Success is false) return registrationResponse;

        var pavilionSessionRegistrationResult = await RegisterToPavilionSessions();

        if (pavilionSessionRegistrationResult.Success is false) return new(pavilionSessionRegistrationResult.Errors);

        await _uow.SaveChangesAsync(cancellationToken);

        return registrationResponse;

        async Task<ResponseResult<RegisteredVisitorDto>> RegisterToEventDates()
        {
            var userAssignedCounterResponse = await _registrationCounterService.GetUserAssignedRegistrationCounter(_loggedInUser.FacetsEventId,
                                                                                                                   cancellationToken);

            if (userAssignedCounterResponse.Success is false) return new(userAssignedCounterResponse.Errors);

            var counterValidation = await ValidateCounter(userAssignedCounterResponse.Data!, cancellationToken);

            if (counterValidation.Success is false) return new(counterValidation.Errors);

            var registrationResponse = await _visitorRegistrationService
                                                .RegisterOnsiteVisitorToEvent(model,
                                                                              userAssignedCounterResponse.Data!.RegistrationCounterId,
                                                                              cancellationToken);

            return registrationResponse;
        }

        async Task<ResponseResult> RegisterToPavilionSessions()
        {
            if (model.PavilionSessionIDs.Count() is 0) return new();

            AddVisitorToPavilionSessionInternalDto addVisitorToPavilsionSession = new()
            {
                PavilionSessionIDs = model.PavilionSessionIDs,
                VisitorRegistrationId = registrationResponse.Data!.Id,
                EventDateIDs = model.EventDateIDs,
                PassCategoryId = model.PassCategoryId
            };

            var pavilionSessionRegistrationResult = await _visitorPavilionService.RegistorVisitorToPavilionSessions(addVisitorToPavilsionSession,
                                                                                                                   cancellationToken);

            return pavilionSessionRegistrationResult;
        }
    }

    public async Task<ResponseResult> UpdateOnsiteVisitorRegistration(Guid visitorRegistrationId,
                                                                      UpdateVisitorOnsiteEventRegistrationDto model,
                                                                      CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<UpdateVisitorOnsiteEventRegistrationDtoValidator, UpdateVisitorOnsiteEventRegistrationDto>
                                                             (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var userAssignedCounterResponse = await _registrationCounterService.GetUserAssignedRegistrationCounter(_loggedInUser.FacetsEventId,
                                                                                                               cancellationToken);

        if (userAssignedCounterResponse.Success is false) return new(userAssignedCounterResponse.Errors);

        var counterValidation = await ValidateCounter(userAssignedCounterResponse.Data!, cancellationToken);

        if (counterValidation.Success is false) return new(counterValidation.Errors);

        var registrationUpdateResponse = await UpdateEventDates(visitorRegistrationId, model, userAssignedCounterResponse.Data!, cancellationToken);

        if (registrationUpdateResponse.Success is false) return registrationUpdateResponse;

        var pavilionSessionRegistrationResult = await UpdateVisitorPavilionSessions();

        if (pavilionSessionRegistrationResult.Success is false) return new(pavilionSessionRegistrationResult.Errors);

        await _uow.SaveChangesAsync(cancellationToken);

        return registrationUpdateResponse;

        async Task<ResponseResult> UpdateEventDates(Guid id,
                                                    UpdateVisitorOnsiteEventRegistrationDto model,
                                                    UserAssignedRegistrationCounterDto userAssignedCounterResponse,
                                                    CancellationToken cancellationToken)
        {
            return await _visitorRegistrationService.UpdateOnsiteVisitorRegistration(id,
                                                                                     model,
                                                                                     userAssignedCounterResponse.RegistrationCounterId,
                                                                                     cancellationToken);

        }

        async Task<ResponseResult> UpdateVisitorPavilionSessions()
        {
            if (model.PavilionSessionIDs.Count() is 0) return new();

            UpdateVisitorToPavilionSessionInternalDto addVisitorToPavilsionSession = new()
            {
                PavilionSessionIDs = model.PavilionSessionIDs,
                VisitorRegistrationId = visitorRegistrationId,
            };

            var pavilionSessionRegistrationResult = await _visitorPavilionService.UpdateVisitorPavilionSessions(addVisitorToPavilsionSession,
                                                                                                                cancellationToken);

            return pavilionSessionRegistrationResult;
        }
    }

    private async Task<ResponseResult> ValidateCounter(UserAssignedRegistrationCounterDto counterDto, CancellationToken cancellationToken)
    {
        var eventPaymentSettings = await _eventSettingsService.GetPaymentSetting(_loggedInUser.FacetsEventId, cancellationToken);

        if (eventPaymentSettings.Success is false) return new(eventPaymentSettings.Errors);

        if (eventPaymentSettings.Data!.OnSitePayingMode is OnSitePayingMode.PayAtRegistration)
        {

            if (counterDto.CounterType is CounterType.RegistrationAndPayment) return new();

            return new(new OperationFailedException("Counter Type", "Registration must be made/updated at a Registration And Payment Counter"));
        }

        else if (eventPaymentSettings.Data!.OnSitePayingMode is OnSitePayingMode.PayAtPassGeneration)
        {
            if (counterDto.CounterType is CounterType.RegistrationOnly) return new();

            return new(new OperationFailedException("Counter Type", "Registration must be made/updated at a Registration Only Counter"));
        }

        return new();
    }

    public async Task<ResponseResult> CancelOnsiteVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<CancelVisitorAttendanceDtoValidator, CancelVisitorAttendanceDto>
                                                             (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var eventResponse = await _eventService.GetEventById(_loggedInUser.FacetsEventId, cancellationToken);

        if (eventResponse.Success is false) return new(eventResponse.Errors);

        if (eventResponse.Data!.Status == EventStatus.Inactive) return new(new OperationFailedException("Event Status", "Event is not active"));

        var cancellationResponse = await _visitorRegistrationService.CancelVisitorAttendance(registrationId, model, cancellationToken);

        if (cancellationResponse.Success is false) return cancellationResponse;

        await _uow.SaveChangesAsync(cancellationToken);

        return new();
    }

    public async Task<ResponseResult<RegisteredVisitorDto>> RegisterOnlineVisitorToEvent(VisitorOnlineEventRegistrationDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<VisitorOnlineEventRegistrationDtoValidator, VisitorOnlineEventRegistrationDto>
                                                             (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var registrationResponse = await _visitorRegistrationService.RegisterOnlineVisitorToEvent(model, cancellationToken);

        if (registrationResponse.Success is false) return registrationResponse;

        var pavilionSessionRegistrationResult = await RegisterToPavilionSessions();

        if (pavilionSessionRegistrationResult.Success is false) return new(pavilionSessionRegistrationResult.Errors);

        await _uow.SaveChangesAsync(cancellationToken);

        return registrationResponse;

        async Task<ResponseResult> RegisterToPavilionSessions()
        {
            if (model.PavilionSessionIDs.Count() is 0) return new();

            AddVisitorToPavilionSessionInternalDto addVisitorToPavilsionSession = new()
            {
                PavilionSessionIDs = model.PavilionSessionIDs,
                VisitorRegistrationId = registrationResponse.Data!.Id,
                EventDateIDs = model.EventDateIDs,
                PassCategoryId = model.PassCategoryId
            };

            var pavilionSessionRegistrationResult = await _visitorPavilionService.RegistorVisitorToPavilionSessions(addVisitorToPavilsionSession,
                                                                                                                    cancellationToken);

            return pavilionSessionRegistrationResult;
        }
    }

    public async Task<ResponseResult> UpdateOnlineVisitorRegistration(Guid visitorRegistrationId,
                                                                      UpdateVisitorOnlineEventRegistrationDto model,
                                                                      CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<UpdateVisitorOnlineEventRegistrationDtoValidator, UpdateVisitorOnlineEventRegistrationDto>
                                                           (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var registrationUpdateResponse = await _visitorRegistrationService.UpdateOnlineVisitorRegistration(visitorRegistrationId,
                                                                                                           model,
                                                                                                           cancellationToken);

        if (registrationUpdateResponse.Success is false) return registrationUpdateResponse;

        var pavilionSessionRegistrationResult = await UpdateVisitorPavilionSessions();

        if (pavilionSessionRegistrationResult.Success is false) return new(pavilionSessionRegistrationResult.Errors);

        await _uow.SaveChangesAsync(cancellationToken);

        return registrationUpdateResponse;

        async Task<ResponseResult> UpdateVisitorPavilionSessions()
        {
            if (model.PavilionSessionIDs.Count() is 0) return new();

            UpdateVisitorToPavilionSessionInternalDto addVisitorToPavilsionSession = new()
            {
                PavilionSessionIDs = model.PavilionSessionIDs,
                VisitorRegistrationId = visitorRegistrationId,
            };

            var pavilionSessionRegistrationResult = await _visitorPavilionService.UpdateVisitorPavilionSessions(addVisitorToPavilsionSession,
                                                                                                                cancellationToken);

            return pavilionSessionRegistrationResult;
        }
    }

    public async Task<ResponseResult> CancelOnlineVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<CancelVisitorAttendanceDtoValidator, CancelVisitorAttendanceDto>
                                                             (model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var eventResponse = await _eventService.GetEventById(_loggedInUser.FacetsEventId, cancellationToken);

        if (eventResponse.Success is false) return new(eventResponse.Errors);

        if (eventResponse.Data!.Status == EventStatus.Inactive) return new(new OperationFailedException("Event Status", "Event is not active"));

        var cancellationResponse = await _visitorRegistrationService.CancelVisitorAttendance(registrationId, model, cancellationToken);

        if (cancellationResponse.Success is false) return cancellationResponse;

        await _uow.SaveChangesAsync(cancellationToken);

        return new();
    }
}
