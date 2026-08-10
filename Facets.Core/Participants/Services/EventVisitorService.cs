using Facets.Core.Common.Interfaces;
using Facets.Core.Events.Interfaces;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Specs;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using System.Text;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Services;

internal sealed class EventVisitorService : IEventVisitorService
{
    private readonly IEventVisitorRepository _eventVisitorRepository;
    private readonly IPassTemplateService _passTemplateService;
    private readonly IVisitorRegistrationRepository _visitorRegistrationRepository;
    private readonly IEventService _eventService;
    private readonly IFileRespository _fileRespository;
    private readonly IVisitorRegistrationService _visitorRegistrationService;
    private readonly IEventRepository _eventRepository;
    private readonly IApplicationContext _applicationContext;
    private readonly IVisitorService _visitorService;
    private readonly ILoggedInUserService _loggedInUser;

    public EventVisitorService(IEventVisitorRepository eventVisitorRepository,
                               ILoggedInUserService loggedInUser,
                               IPassTemplateService passTemplateService,
                               IVisitorRegistrationRepository visitorRegistrationRepository,
                               IEventService eventService,
                               IVisitorService visitorService,
                               IApplicationContext applicationContext,
                               IFileRespository fileRespository,
                               IVisitorRegistrationService visitorRegistrationService,
                               IEventRepository eventRepository)
    {
        _eventVisitorRepository = eventVisitorRepository;
        _passTemplateService = passTemplateService;
        _visitorRegistrationRepository = visitorRegistrationRepository;
        _eventService = eventService;
        _visitorService = visitorService;
        _fileRespository = fileRespository;
        _visitorRegistrationService = visitorRegistrationService;
        _eventRepository = eventRepository;
        _applicationContext = applicationContext;
        _loggedInUser = loggedInUser;
    }

    public async Task<ResponseResult<IReadOnlyList<Attendee>>> GetEventVisitors(Guid eventDateId, Paginator paginator, AttendeesFilter filter, CancellationToken token)
    {
        var eventPayLaterResponse = await _eventService.IsEventMarkedAsPayLaterForOnlineRegistration(_loggedInUser.FacetsEventId, token);

        var onSitePayingModeResponse = await _eventService.GetEventOnSitePaymentModeSpec(_loggedInUser.FacetsEventId, token);

        var (list, totalRecords) = await _eventVisitorRepository
                                         .GetProjectedListBySpec(paginator, new EventVisitorFilterSpec(eventDateId,
                                                                                                       _loggedInUser.FacetsEventId,
                                                                                                       filter,
                                                                                                       eventPayLaterResponse.Data!,
                                                                                                       onSitePayingModeResponse.Data!),
                                                                 token);
        return new(list, totalRecords);
    }

    public async Task<ResponseResult<VisitorPassVerificationDto>> GetVisitorVerificationDetails(Guid eventId, Guid visitorId, Guid eventDateId, CancellationToken token)
    {
        var res = await _eventVisitorRepository
                                         .GetPassVerificationDataBySpec(new GetVisitorVerificationDetailsSpec(eventId, visitorId, eventDateId), token);


        return new ResponseResult<VisitorPassVerificationDto>(res);
    }

    public async Task<ResponseResult<VisitorPassTemplateDto>> GetVisitorPassGenerationTemplate(Guid eventId, Guid visitorId, Guid eventDateId, CancellationToken token)
    {
        var template = await _passTemplateService.GetPassTemplates(new Paginator(), eventId, new PassTemplateFilter(PassType.Visitor), token);

        if (template.TotalRecordCount is 0)
            return new(new OperationFailedException("Visitor Pass Template", "No Pass template available for current event"));

        var passVisitor = await _eventVisitorRepository
                                .GetPassTemplateVisitorBySpec(new PassTemplateVisitorByVisitorIdSpec(visitorId, eventDateId, eventId),
                                                              token);

        var imageResponse = await _fileRespository.DownloadAsStream(AppConstants.BlobStorage.ContainerName.VisitorDocuments, passVisitor.ProfileImage, token);

        if (imageResponse.Success is false) return new(imageResponse.Errors);

        var templateData = template.Data!.First();

        StringBuilder templateBuilder = new(templateData.TemplateText);
        var imageData = ImageHelper.GetThumbnailAsBase64(imageResponse.Data!);

        string templateText = templateBuilder.Replace("Event Name", passVisitor.EventName.ToUpper())
                                             .Replace("First Name", passVisitor.FirstName.ToUpper())
                                             .Replace("Last Name", passVisitor.LastName.ToUpper())
                                             .Replace("NIC Number/ Passport Number", passVisitor.VisitorIdentityType ==
                                                                                     VisitorIdentityType.NIC ?
                                                                                     passVisitor.NICNumber!.ToUpper() :
                                                                                     passVisitor.PassportNumber!.ToUpper())
                                             .Replace("Mobile Number", passVisitor.MobileNumber.ToUpper())
                                             .Replace("Pass Generated Date & Time", passVisitor.PassGeneratedDateTime)
                                             .Replace("Pass Date", passVisitor.PassDate.GetLocalTime
                                                                   (AppConstants.SriLankaTimeZone).ToApplicationDateFormat())
                                             .Replace("Pass Rate", passVisitor.PassRate.ToString("F"))
                                             .Replace("Country", passVisitor.CountryName.ToUpper())
                                             .Replace("Company", passVisitor.CompanyName?.ToUpper())
                                             .Replace($"https://exhibition.facetssrilanka.com/assets/images/user-profile.png", imageData)
                                             .ToString();

        var fullName = passVisitor.FirstName.ToUpper() + " " + passVisitor.LastName.ToUpper();

        await AddVisitorActivity(eventId, visitorId, passVisitor);

        return new ResponseResult<VisitorPassTemplateDto>(new VisitorPassTemplateDto(templateData.Width, templateData.Height, templateText, passVisitor.PassCategoryColor, passVisitor.PassCategoryName.ToUpper(), passVisitor.ProfileImage, fullName));

        async Task AddVisitorActivity(Guid eventId, Guid visitorId, PassTemplateVisitorDto passVisitor)
        {
            string description = $"{passVisitor.PassGeneratedDateTime} - Generated pass for {passVisitor.PassDate.ToApplicationDateFormat()}";

            VisitorActivity activity = new(eventId, visitorId, description, VisitorActivityType.PassGenerated);

            _visitorService.AddVisitorActivity(activity);

            await _eventVisitorRepository.SaveChangesAsync(token);
        }
    }

    public async Task<ResponseResult<QRVerifiedVisitorDto>> VerifyPass(VisitorPassVerificationDto model, CancellationToken token)
    {
        var visitorRegistrationResponse = await _visitorRegistrationService.GetVisitorRegistration(model.VisitorId, token);

        if (visitorRegistrationResponse.Success is false) return new(visitorRegistrationResponse.Errors);

        var visitorPassCategoryType = visitorRegistrationResponse.Data!.VisitorPassCategoryType;

        var currentDate = DateTimeOffset.UtcNow;

        var visitorAttendanceScheduleResponse = await _visitorRegistrationService.GetVisitorRegistrationByCurrentDate(model.VisitorId,
                                                                                                                      currentDate, token);

        if (visitorAttendanceScheduleResponse.Success is false) return new(visitorAttendanceScheduleResponse.Errors);

        var attendenceScheduleId = visitorPassCategoryType == VisitorPassCategoryType.PerDayPass ? model.AttendanceSheduleId : visitorAttendanceScheduleResponse.Data!.VisitorAttendanceScheduleId;

        var visitorAttendance = await _eventVisitorRepository.GetAttendanceToVerify(_loggedInUser.FacetsEventId,
                                                                                    visitorAttendanceScheduleResponse.Data!.EventDateId,
                                                                                    attendenceScheduleId,
                                                                                    token);

        if (visitorAttendance is null)
            return new(new OperationFailedException("Visitor Regsitration", "Visitor has no registration for current date"));

        var attendanceResponse = visitorAttendance.UpdateAttendance(Guid.Parse(_loggedInUser.UserId));

        if (attendanceResponse.Success is false) return new(attendanceResponse.Errors);

        var visitor = await _visitorRegistrationRepository
                                .GetProjectedRegistrationBySpec(new QRVerifiedVisitorSpec(_loggedInUser.FacetsEventId,
                                                                                          visitorAttendance.VisitorRegistrationId),
                                                                token);

        await _eventVisitorRepository.SaveChangesAsync(token);

        return new(visitor);
    }

    public async Task<ResponseResult<QRVerifiedVisitorDto>> VerifyPavilionPass(VisitorPavilionPassVerificationDto model, CancellationToken token)
    {
        var currentDate = DateTimeOffset.UtcNow;

        var visitorPavilionSessionAttendanceScheduleResponse = await _visitorRegistrationService.GetPassVisitorRegistrationByCurrentDate(model.VisitorId,
                                                                                                                          model.PavilionId,
                                                                                                                          model.PavilionSessionId,
                                                                                                                          currentDate, token);

        if (visitorPavilionSessionAttendanceScheduleResponse.Success is false) return new(visitorPavilionSessionAttendanceScheduleResponse.Errors);

        var visitorPavilionSessionAttendanceSchedule = await _eventVisitorRepository.GetPavilionSessionAttendanceToVerify(_loggedInUser.FacetsEventId,
                                                                                                                          model.VisitorId,
                                                                                                                          model.PavilionId,
                                                                                                                          model.PavilionSessionId,
                                                                                                                          visitorPavilionSessionAttendanceScheduleResponse.Data!.EventDateId,
visitorPavilionSessionAttendanceScheduleResponse.Data!.VisitorPavilionAttendanceScheduleId,
                                                                                                                          token
                                                                                                                         );

        if (visitorPavilionSessionAttendanceSchedule is null)
            return new(new OperationFailedException("Visitor Regsitration", "Visitor has no registration for current date"));

        var attendanceResponse = visitorPavilionSessionAttendanceSchedule.UpdateAttendance();

        if (attendanceResponse.Success is false) return new(attendanceResponse.Errors);

        var visitor = await _visitorRegistrationRepository
                                .GetProjectedRegistrationBySpec(new QRVerifiedVisitorSpec(_loggedInUser.FacetsEventId,
                                                                                          visitorPavilionSessionAttendanceSchedule.VisitorRegistrationId),
                                                                token);

        await _eventVisitorRepository.SaveChangesAsync(token);

        return new(visitor);
    }
}
