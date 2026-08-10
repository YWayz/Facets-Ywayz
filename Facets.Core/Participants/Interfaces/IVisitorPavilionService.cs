using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Interfaces;

public interface IVisitorPavilionService
{
    Task<ResponseResult<IReadOnlyList<VisitorPavilionSessionAttendanceScheduleDto>>> GetVisitorPavilionSessions(Guid visitorRegistrationId, CancellationToken token);
    internal Task<ResponseResult> RegistorVisitorToPavilionSessions(AddVisitorToPavilionSessionInternalDto model, CancellationToken token);
    internal Task<ResponseResult> UpdateVisitorPavilionSessions(UpdateVisitorToPavilionSessionInternalDto model, CancellationToken token);
    Task<ResponseResult<IReadOnlyCollection<PavilionSessionVisitorDto>>> GetPavilionSessionVisitors(Paginator paginator, PavilionSessionVisitorFilter filter, CancellationToken cancellationToken);
}