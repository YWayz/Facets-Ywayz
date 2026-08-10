using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Filters
{
    public class AttendancereportFilter
    {
        public required Guid EventId { get; init; }
        public required IEnumerable<Guid>? EventDateIds { get; init; }
        public IEnumerable<Guid>? VisitorCountryIds { get; init; }
        public IEnumerable<VisitorStatus>? VisitorStatuses { get; init; }
        public IEnumerable<Guid>? PassCategoryIds { get; init; }
        public DateTimeOffset? VisitorEventRegistrationSpecificDate { get; init; }
        public DateTimeOffset? VisitorEventRegistrationFromDate { get; init; }
        public DateTimeOffset? VisitorEventRegistrationToDate { get; init; }
    }
}
