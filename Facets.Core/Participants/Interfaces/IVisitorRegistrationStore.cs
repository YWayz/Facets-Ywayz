using Facets.Core.Participants.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Interfaces;

public interface IVisitorRegistrationStore
{
    Task<ResponseResult> CancelOnsiteVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken);
    Task<ResponseResult> CancelOnlineVisitorAttendance(Guid registrationId, CancelVisitorAttendanceDto model, CancellationToken cancellationToken);
    Task<ResponseResult<RegisteredVisitorDto>> RegisterOnsiteVisitorToEvent(VisitorOnsiteEventRegistrationDto model, CancellationToken cancellationToken);
    Task<ResponseResult> UpdateOnsiteVisitorRegistration(Guid id, UpdateVisitorOnsiteEventRegistrationDto model, CancellationToken cancellationToken);

    Task<ResponseResult<RegisteredVisitorDto>> RegisterOnlineVisitorToEvent(VisitorOnlineEventRegistrationDto model, CancellationToken cancellationToken);

    Task<ResponseResult> UpdateOnlineVisitorRegistration(Guid id, UpdateVisitorOnlineEventRegistrationDto model, CancellationToken cancellationToken);
}
