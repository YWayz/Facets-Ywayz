using Facets.Core.Events.DTOs;
using Facets.Core.Events.Filters;
using Facets.Core.Events.Specs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Events.Interfaces;

public interface IPavilionService
{
    Task<ResponseResult<PavilionDto>> CreatePavilion(Guid eventId, CreatePavilionDto model, CancellationToken cancellationToken);
    Task<ResponseResult<PavilionDto>> GetPavilionById(Guid eventId, Guid id, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<PavilionSummaryDto>>> GetPavilions(Guid eventId, Paginator paginator, PavilionFilter filter, CancellationToken token);
    Task<ResponseResult> UpdatePavilion(Guid eventId, Guid pavilionId, UpdatePavilionDto model, CancellationToken token);
    Task<ResponseResult> DeletePavilion(Guid eventId, Guid id, CancellationToken token);

    Task<ResponseResult<PavilionSessionDto>> CreatePavilionSession(Guid eventId, Guid pavilionId, CreateOrUpdatePavilionSessionItemDto model, CancellationToken cancellationToken);
    Task<ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token);
    Task<ResponseResult> UpdatePavilionSession(Guid eventId, Guid pavilionId, Guid pavilionSessionId, CreateOrUpdatePavilionSessionItemDto model, CancellationToken token);

    Task<ResponseResult> UpdatePavilionStatus(Guid eventId, Guid pavilionId, UpdatePavilionStatusDto model, CancellationToken token);
    Task<ResponseResult<bool>> CheckPavilionSessionsExist(Guid eventId, CancellationToken token);
    Task<ResponseResult> DeletePavilionSession(Guid eventId, Guid pavilionSessionId, Guid pavilionId, CancellationToken token);
}
