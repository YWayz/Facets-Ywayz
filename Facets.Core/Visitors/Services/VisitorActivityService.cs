using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.TeamMembers.Specs;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Interfaces;
using Facets.Core.Visitors.Specs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Logging;

namespace Facets.Core.Visitors.Services;

public sealed class VisitorActivityService : IVisitorActivityService
{
    private readonly IVisitorActivityRepository _visitorActivityRepository;

    public VisitorActivityService(IVisitorActivityRepository visitorActivityRepository)
    {
        _visitorActivityRepository = visitorActivityRepository;
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorActivityDto>>> GetVisitorActivities(Paginator paginator, Guid visitorId, Guid eventId, CancellationToken token)
    {
        var (list, totalRecords) = await _visitorActivityRepository.GetProjectedListBySpec(paginator, new VisitorActivitySpec(visitorId, eventId), token);
        return new(list, totalRecords);
    }
}
