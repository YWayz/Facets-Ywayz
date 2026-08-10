using Facets.Core.Visitors.DTOs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Visitors.Interfaces;

public interface IVisitorActivityService
{
    Task<ResponseResult<IReadOnlyList<VisitorActivityDto>>> GetVisitorActivities(Paginator paginator, Guid visitorId, Guid eventId, CancellationToken token);
}
