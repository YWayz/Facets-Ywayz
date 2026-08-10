using Ardalis.Specification;
using Facets.Core.Events.Entities;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.SharedKernal.Models;

namespace Facets.Core.Participants.Interfaces;

public interface IVisitorPavilionSessionRepository
{
    Task<IReadOnlyList<KeyValuePair<Guid, int>>> GetVisitorCountByPavilionSessions(IEnumerable<Guid> distinctPavilionSessionIds, CancellationToken token);

    Task<IReadOnlyList<PavilionSession>> GetListBySpec(ISpecification<PavilionSession> specification,
                                                       CancellationToken token,
                                                       bool asTracking = false);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>( ISpecification<PavilionSession, TResult> specification, CancellationToken token);

    Task<(IReadOnlyCollection<PavilionSessionVisitorDto> list, int totalRecordCount)> GetPavilionSessionVisitors(Paginator paginator, Guid eventId, PavilionSessionVisitorFilter filter, CancellationToken cancellationToken);
}
