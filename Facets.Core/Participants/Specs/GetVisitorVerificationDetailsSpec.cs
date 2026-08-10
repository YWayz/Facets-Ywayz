using Ardalis.Specification;
using Facets.Core.Participants.Entities;
using Facets.Core.Passes.DTOs;

namespace Facets.Core.Participants.Specs;

internal sealed class GetVisitorVerificationDetailsSpec : Specification<VisitorRegistration, VisitorPassVerificationDto>
{
    public GetVisitorVerificationDetailsSpec(Guid eventId, Guid visitorId, Guid eventDateId)
    {
        Query.Where(w => w.VisitorId == visitorId && w.EventId == eventId);

        Query.Select(s => s.VisitorAttendanceSchedules.Where(w => w.EventDateId == eventDateId)
                                                      .Select(e => new VisitorPassVerificationDto(e.Id, visitorId)).FirstOrDefault());
    }
}
