using Facets.Core.Common.Entities;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;

namespace Facets.Core.Lookups.Interfaces;

public interface ILookupRepository
{
    Task<IReadOnlyList<KeyValuePair<Guid, string>>> GetAssignedUsersToEvent(Guid eventId, CancellationToken token);
    Task<IReadOnlyList<Country>> GetCountries(CancellationToken token);
    Task<IReadOnlyList<KeyValuePair<Guid, string>>> GetPavilions(Guid eventId, CancellationToken token);
    Task<IReadOnlyList<PavilionSessionSummaryDto>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token);
}
