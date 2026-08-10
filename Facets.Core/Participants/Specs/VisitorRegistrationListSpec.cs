using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Filters;
using Microsoft.EntityFrameworkCore;

namespace Facets.Core.Participants.Specs;

internal sealed class VisitorRegistrationListSpec : Specification<VisitorRegistration, VisitorRegistrationSummary>
{
    public VisitorRegistrationListSpec(Guid eventId, RegistrationFilter filter)
    {
        Query.Where(w => w.EventId == eventId);

        if (string.IsNullOrWhiteSpace(filter.SearchQuery?.Trim()) is false)
        {
            string? searchTerm = filter.SearchQuery?.Trim();

            Query.Where(w => EF.Functions.Like(w.Visitor.FirstName, searchTerm + "%") ||
                             EF.Functions.Like(w.Visitor.LastName, searchTerm + "%") ||
                             EF.Functions.Like(w.Visitor.NICNumber!, searchTerm + "%") ||
                             EF.Functions.Like(w.Visitor.PassportNumber!, searchTerm + "%") ||
                             EF.Functions.Like(w.Visitor.MobileNumber, searchTerm + "%") ||
                             EF.Functions.Like(w.Visitor.VisitorReference, searchTerm + "%"));
        }

        if (filter.RegistrationCancelled.HasValue) Query.Where(w => w.RegistrationCancelled == filter.RegistrationCancelled.Value);

        if (filter.countryId.HasValue) Query.Where(w => w.Visitor.CountryId == filter.countryId.Value);

        if (filter.VisitorStatus is not null) Query.Where(w => filter.VisitorStatus.Contains(w.Visitor.VisitorStatus));

        if (filter.visitorId.HasValue) Query.Where(w => w.VisitorId == filter.visitorId.Value);

        Query.Select(s => new VisitorRegistrationSummary(s.Visitor.FirstName,
                                                         s.Visitor.LastName,
                                                         s.Visitor.NICNumber,
                                                         s.Visitor.PassportNumber,
                                                         s.Visitor.Country.Name,
                                                         s.Visitor.VisitorStatus,
                                                         s.RegistrationCancelled,
                                                         s.RegistrationCancelledOn,
                                                         s.Visitor.VisitorIdentityType,
                                                         s.EventId,
                                                         s.Visitor.MobileNumber,
                                                         s.VisitorId,
                                                         s.Id));
    }
}
