using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Interfaces;

public interface IVisitorRegistrationService
{
    Task<ResponseResult<RegisteredVisitorDetailDto>> GetRegistrationById(Guid id, CancellationToken token);

    Task<ResponseResult<IReadOnlyList<VisitorRegistrationSummary>>> GetRegistrations(Paginator paginator, RegistrationFilter filter, CancellationToken token);

    Task<ResponseResult<RegisteredVisitorDetailDto>> GetVisitorRegistration(Guid visitorId, CancellationToken token);

    internal Task<ResponseResult> UpdateOnsiteVisitorRegistration(Guid id, UpdateVisitorOnsiteEventRegistrationDto model, Guid registrationCounterId, CancellationToken cancellationToken);

    internal Task<ResponseResult<RegisteredVisitorDto>> RegisterOnsiteVisitorToEvent(VisitorOnsiteEventRegistrationDto model, Guid registrationCounterId, CancellationToken cancellationToken);

    internal Task<ResponseResult> CancelVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken);

    internal Task<ResponseResult<RegisteredVisitorDto>> RegisterOnlineVisitorToEvent(VisitorOnlineEventRegistrationDto model, CancellationToken cancellationToken);

    Task<ResponseResult<VisitorPassRegistrationDto>> GetVisitorPassRegistration(Guid visitorId, Guid visitorRegistrationId, RegistrationFilter filter, CancellationToken token);

    internal Task<ResponseResult> UpdateOnlineVisitorRegistration(Guid registrationId, UpdateVisitorOnlineEventRegistrationDto model, CancellationToken cancellationToken);

    Task<ResponseResult<VisitorAttendanceScheduleDto>> GetVisitorRegistrationByCurrentDate(Guid visitorId, DateTimeOffset currentDate, CancellationToken token);
    Task<ResponseResult<VisitorPavilionSessionAttendanceScheduleDto>> GetPassVisitorRegistrationByCurrentDate(Guid visitorId, Guid pavilionId, Guid pavilionSessionId, DateTimeOffset currentDate, CancellationToken token);

    Task<ResponseResult> MarkPassAsPrinted(Guid visitorId, Guid attendanceScheduleId, CancellationToken cancellationToken);
}
