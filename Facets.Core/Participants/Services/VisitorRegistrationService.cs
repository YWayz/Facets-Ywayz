using Facets.Core.Common.Interfaces;
using Facets.Core.Common;
using Facets.Core.Common.Validators;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Specs;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;
using Facets.Core.Events.Interfaces;
using Facets.SharedKernal.Extensions;
using Facets.Core.Common.Dtos;
using Facets.SharedKernal.Helpers;

namespace Facets.Core.Participants.Services;

internal sealed class VisitorRegistrationService : IVisitorRegistrationService
{
    private readonly IVisitorRegistrationRepository _visitorRegistrationRepository;
    private readonly ILoggedInUserService _loggedInUser;
    private readonly IModelValidator _validator;
    private readonly IVisitorService _visitorService;
    private readonly IEventService _eventService;
    private readonly ISMSService _smsService;
    private readonly IEmailService _emailService;

    public VisitorRegistrationService(IVisitorRegistrationRepository visitorRegistrationRepository, ILoggedInUserService loggedInUser, IModelValidator validator, IVisitorService visitorService, IEventService eventService, ISMSService smsService,
        IEmailService emailService)
    {
        _visitorRegistrationRepository = visitorRegistrationRepository;
        _loggedInUser = loggedInUser;
        _validator = validator;
        _visitorService = visitorService;
        _eventService = eventService;
        _smsService = smsService;
        _emailService = emailService;
    }

    public async Task<ResponseResult<RegisteredVisitorDto>> RegisterOnsiteVisitorToEvent(VisitorOnsiteEventRegistrationDto model, Guid registrationCounterId, CancellationToken cancellationToken)
    {
        var visitorResponse = await _visitorService.GetVisitorById(model.VisitorId, cancellationToken);

        if (visitorResponse.Success is false) return new(visitorResponse.Errors);

        if (visitorResponse.Data!.VisitorStatus is VisitorStatus.BlackListed)
            return new(new OperationFailedException("Visitor", "Visitor is Blacklisted"));

        VisitorRegistration entity = new(_loggedInUser.FacetsEventId,
                                         model.VisitorId,
                                         registeredToEventOnsite: true,
                                         model.PassCategoryId,
                                         registrationCounterId,
                                         model.EventDateIDs);

        _visitorRegistrationRepository.Add(entity);

        return new(new RegisteredVisitorDto(entity.Id,
                                            entity.VisitorId,
                                            entity.PassCategoryId,
                                            model.EventDateIDs.ToList().AsReadOnly(),
                                            registeredToEventOnsite: entity.RegisteredToEventOnsite));
    }

    public async Task<ResponseResult<RegisteredVisitorDetailDto>> GetRegistrationById(Guid id, CancellationToken token)
    {
        var visitorRegistration = await _visitorRegistrationRepository.GetProjectedRegistrationBySpec(new RegistraionDetailByIdSpec(id), token);

        if (visitorRegistration is null) return new(new NotFoundException(nameof(id), "Registration", id));

        return new(visitorRegistration);
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorRegistrationSummary>>> GetRegistrations(Paginator paginator, RegistrationFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _visitorRegistrationRepository.GetProjectedListBySpec(paginator,
                                                                                               new VisitorRegistrationListSpec
                                                                                                    (_loggedInUser.FacetsEventId,
                                                                                                     filter),
                                                                                               token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<RegisteredVisitorDetailDto>> GetVisitorRegistration(Guid visitorId, CancellationToken token)
    {
        var visitorRegistration = await _visitorRegistrationRepository.GetProjectedRegistrationBySpec(new RegistraionByVisitorIdSpec
                                                                                                            (_loggedInUser.FacetsEventId,
                                                                                                             visitorId),
                                                                                                      token);

        if (visitorRegistration is null) return new(new NotFoundException(nameof(visitorId), "Visitor Registration", visitorId));

        return new(visitorRegistration);
    }

    public async Task<ResponseResult<VisitorAttendanceScheduleDto>> GetVisitorRegistrationByCurrentDate(Guid visitorId, DateTimeOffset currentDate, CancellationToken token)
    {
        var visitorAttendanceScheduleDto = await _visitorRegistrationRepository.GetProjectedRegistrationBySpec(new RegistrationByVisitorIdAndDateSpec
                                                                                                            (_loggedInUser.FacetsEventId,
                                                                                                             visitorId,
                                                                                                             currentDate),
                                                                                                      token);

        if (visitorAttendanceScheduleDto is null) return new(new NotFoundException(nameof(visitorId), "Visitor Attendance Schedule", visitorId));

        return new(visitorAttendanceScheduleDto);
    }

    public async Task<ResponseResult<VisitorPavilionSessionAttendanceScheduleDto>> GetPassVisitorRegistrationByCurrentDate(Guid visitorId, Guid pavilionId, Guid pavilionSessionId, DateTimeOffset currentDate, CancellationToken token)
    {
        var visitorPavilionSessionAttendanceScheduleDto = await _visitorRegistrationRepository.GetProjectedRegistrationBySpec(new PassRegistrationByVisitorIdAndDateSpec
                                                                                                            (_loggedInUser.FacetsEventId,
                                                                                                             visitorId,
                                                                                                             pavilionId,
                                                                                                             pavilionSessionId,
                                                                                                             currentDate),
                                                                                                      token);

        if (visitorPavilionSessionAttendanceScheduleDto is null) return new(new NotFoundException(nameof(visitorId), "Visitor Pavilion Session Attendance Schedule", visitorId));

        return new(visitorPavilionSessionAttendanceScheduleDto);
    }

    public async Task<ResponseResult> UpdateOnsiteVisitorRegistration(Guid registrationId,
                                                                      UpdateVisitorOnsiteEventRegistrationDto model,
                                                                      Guid registrationCounterId, CancellationToken cancellationToken)
    {
        var registration = await _visitorRegistrationRepository.GetRegistrationBySpec(new UpdateVisitorRegistrationSpec(registrationId,
                                                                                      _loggedInUser.FacetsEventId),
                                                                                      cancellationToken,
                                                                                      asTracking: true);

        if (registration is null) return new(new NotFoundException(nameof(registrationId), "Visitor Registration", registrationId));

        var updateResponse = registration.Update(model.EventDateIDs, attendanceScheduledOnsite: true, registrationCounterId);

        if (updateResponse.Success is false) return updateResponse;

        return new();

    }

    public async Task<ResponseResult> UpdateOnlineVisitorRegistration(Guid registrationId,
                                                                      UpdateVisitorOnlineEventRegistrationDto model,
                                                                      CancellationToken cancellationToken)
    {
        var registration = await _visitorRegistrationRepository.GetRegistrationBySpec(new UpdateVisitorRegistrationSpec(registrationId,
                                                                                _loggedInUser.FacetsEventId),
                                                                                cancellationToken,
                                                                                asTracking: true);

        if (registration is null) return new(new NotFoundException(nameof(registrationId), "Visitor Registration", registrationId));

        var updateResponse = registration.Update(model.EventDateIDs, attendanceScheduledOnsite: false, visitorRegistrationCounterId: null);

        if (updateResponse.Success is false) return updateResponse;

        return new();
    }

    public async Task<ResponseResult> CancelVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken)
    {

        List<VisitorAttendanceSchedule> _visitorAttendanceSchedules = new();
        var registration = await _visitorRegistrationRepository.GetRegistrationBySpec(new CancelRegistrationSpec(registrationId),
                                                                                      cancellationToken,
                                                                                      asTracking: true);
        _visitorAttendanceSchedules = registration.VisitorAttendanceSchedules.ToList();
        if (registration is null) return new(new NotFoundException(nameof(registrationId), "Visitor Registration", registrationId));


        var cancellationResponse = registration.CancelRegistration(model.AttendanceScheduleIds);


        if (_visitorAttendanceSchedules.Where(a => model.AttendanceScheduleIds.Contains(a.Id)).Any(s => s.VisitorAttended is true))
            return new(new OperationFailedException("Registration Cancellation", "One or more event(s) has been attended and cannot be cancelled"));

        var attendanceShedulesToCancel = _visitorAttendanceSchedules.Where(w => model.AttendanceScheduleIds.Contains(w.Id)).ToList();


        if (cancellationResponse.Success is false) return cancellationResponse;

        await SendCancellationEmail(registration, attendanceShedulesToCancel, cancellationToken);

        return new();
    }

    public async Task<ResponseResult<RegisteredVisitorDto>> RegisterOnlineVisitorToEvent(VisitorOnlineEventRegistrationDto model,
                                                                                         CancellationToken cancellationToken)
    {
        var visitorResponse = await _visitorService.GetVisitorById(model.VisitorId, cancellationToken);

        if (visitorResponse.Success is false) return new(visitorResponse.Errors);

        var visitor = visitorResponse.Data!;

        if (visitor.VisitorStatus is VisitorStatus.BlackListed)
            return new(new OperationFailedException("Visitor", "Visitor is Blacklisted"));

        else if (visitor.OTPVerificationRequired && visitor.OTPVerified is false)
            return new(new OperationFailedException("Visitor", "OTP verification requried"));

        VisitorRegistration entity = new(_loggedInUser.FacetsEventId,
                                         model.VisitorId,
                                         registeredToEventOnsite: false,
                                         model.PassCategoryId,
                                         visitorRegistrationCounterId: null,
                                         model.EventDateIDs);

        _visitorRegistrationRepository.Add(entity);

        return new(new RegisteredVisitorDto(entity.Id,
                                            entity.VisitorId,
                                            entity.PassCategoryId,
                                            model.EventDateIDs.ToList().AsReadOnly(),
                                            registeredToEventOnsite: entity.RegisteredToEventOnsite));
    }

    public async Task<ResponseResult<VisitorPassRegistrationDto>> GetVisitorPassRegistration(Guid visitorId, Guid visitorRegistrationId, RegistrationFilter filter, CancellationToken token)
    {
        var visitorPassRegistration = await _visitorRegistrationRepository.GetProjectedRegistrationBySpec(new GetVisitorPassRegistrationByVisitorIdSpec
                                                                                                            (visitorId,
                                                                                                            visitorRegistrationId,
                                                                                                            _loggedInUser.FacetsEventId,
                                                                                                            filter),
                                                                                                      token);

        if (visitorPassRegistration is null) return new(new NotFoundException(nameof(visitorId), "Visitor Pass Registration", visitorId));

        return new(visitorPassRegistration);
    }

    public async Task<ResponseResult> MarkPassAsPrinted(Guid visitorId, Guid attendanceScheduleId, CancellationToken cancellationToken)
    {
        var registration = await _visitorRegistrationRepository.GetRegistrationBySpec(new UpdateVisitorRegistrationPassPrintSpec(_loggedInUser.FacetsEventId,
                                                                                                                       visitorId,
                                                                                                                       attendanceScheduleId),
                                                                                                                       cancellationToken, asTracking: true);

        if (registration!.VisitorAttendanceSchedules.Count is 0) return new(new NotFoundException(nameof(attendanceScheduleId), "Visitor attendance schedule", attendanceScheduleId));

        var updateResponse = registration.UpdatePassPrinted();

        if (updateResponse.Success is false) return updateResponse;

        await _visitorRegistrationRepository.SaveChangesAsync(cancellationToken);

        return new();
    }


    // Add a new method to send the cancellation email
    private async Task SendCancellationEmail(VisitorRegistration visitorRegistration, List<VisitorAttendanceSchedule> attendanceShedulesToCancel, CancellationToken cancellationToken)
    {
        var eventRespone = await _eventService.GetEventById(visitorRegistration.EventId, cancellationToken);

        var assignedEventDateIds = attendanceShedulesToCancel.Select(s => s.EventDateId);

        var eventDates = eventRespone.Data!.EventDates
                                           .Where(w => assignedEventDateIds.Contains(w.Key))
                                           .Select(s => s.Value)
                                           .OrderBy(o => o);

        var localDates = eventDates.Select(s => s.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat()).ToList();

        if (localDates.Count is 0) return;

        string commaDelimeteredDates = string.Join(", ", localDates);

        var visitorResponse = await _visitorService.GetVisitorById(visitorRegistration.VisitorId, cancellationToken);
        var visitor = visitorResponse.Data!;

        //string template = await _emailService.GetEmailTemplate("VisitorRegistrationCancelEmailTemplate.html");

        //await _emailService.SendEmailByQueue(EmailBuilder.BuildCancellationEmailMessage(toEmailAddress: visitor.Email, eventName: eventRespone.Data.Name, visitor.FirstName, visitor.LastName, visitor.VisitorReference
        //    , visitor.VisitorIdentityType, visitor.NICNumber, commaDelimeteredDates, template));


        bool isSriLankanNumber = MobileNumberHelper.IsSriLankaNumber(visitor.MobileNumber);

        string sendTo = isSriLankanNumber ? visitor.MobileNumber! : visitor.Email!;


        if (isSriLankanNumber is false)
        {
            string template = await _emailService.GetEmailTemplate("VisitorRegistrationCancelEmailTemplate.html");

            await _emailService.SendEmailByQueue(EmailBuilder.BuildCancellationEmailMessage(
                                                                              sendTo,
                                                                              eventName: eventRespone.Data.Name,
                                                                              visitorFirstName: visitor.FirstName,
                                                                              visitorLastName: visitor.LastName,
                                                                              visitorReference: visitor.VisitorReference,
                                                                              visitorIdentityType: visitor.VisitorIdentityType,
                                                                              identificationNumber: visitor.NICNumber ?? visitor.PassportNumber!,
                                                                              commaDelimeteredDates: commaDelimeteredDates,
            template));
        }

        else
        {
            await _smsService
            .SendSMSByQueue(SMSMessage.BuildEventRegistrationCancelCompletedMessage(mobileNumber: visitor.MobileNumber,
                                                                              eventName: eventRespone.Data.Name,
                                                                              visitorFirstName: visitor.FirstName,
                                                                              visitorLastName: visitor.LastName,
                                                                              visitorReference: visitor.VisitorReference,
                                                                              visitorIdentityType: visitor.VisitorIdentityType,
                                                                              identificationNumber: visitor.NICNumber ?? visitor.PassportNumber!,
                                                                              commaDelimeteredDates: commaDelimeteredDates));
        }



    }
}
