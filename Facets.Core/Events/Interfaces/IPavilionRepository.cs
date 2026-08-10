using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Events.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Events.Interfaces;

public interface IPavilionRepository : IBaseRepository
{
    Pavilion AddPavilion(Pavilion pavilion);
    Task<bool> IsPavilionNameTaken(Guid eventId, string name, CancellationToken cancellationToken, Guid? id = null);
    Task<TResult?> GetProjectedPavilionBySpec<TResult>(ISpecification<Pavilion, TResult> specification, CancellationToken token);
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Pavilion, TResult> specification, CancellationToken token);
    Task<Pavilion?> GetPavilionBySpec(ISpecification<Pavilion> specification, CancellationToken token, bool asTracking = false);
    Task<bool> CheckOverlappingSession(Guid eventId, Guid pavilionId, Guid eventDateId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken, Guid? pavilionSessionId = null);
    Task<bool> CheckPavilionSessionsExist(Guid eventId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Pavilion>> GetPavilions(Guid eventId, CancellationToken cancellationToken);
    Task<bool> CanDeletePavilion(Guid eventId, Guid pavilionId, CancellationToken cancellationToken);
    Task<bool> CanDeletePavilionSession(Guid eventId, Guid pavilionId, Guid pavilionSessionId, CancellationToken cancellationToken);
    Task<PavilionSession?> GetPavilionSessionBySpec(ISpecification<PavilionSession> specification, CancellationToken token, bool asTracking = false);
}
