using Facets.Core.Common.Dtos;
using Facets.Core.Common.Filters;
using Facets.Core.Events.DTOs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Common.Interfaces;

public interface ILookupService
{
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetEventList(Paginator paginator, EventLookupFilter filter, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetUserAssignedEventList(Paginator paginator, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetPassCategories(Guid eventId, Paginator paginator, PassCategoryLookupFilter filter, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetCountries(CancellationToken token);
    Task<ResponseResult<IReadOnlyList<RegistrationCounterLookUpDto>>> GetRegistrationCounters(Guid eventId, Paginator paginator, RegistrationCounterLookupFilter filter, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetAssignedUsersToEvent(Guid eventId, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetPavilions(Guid eventId, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token);
}
