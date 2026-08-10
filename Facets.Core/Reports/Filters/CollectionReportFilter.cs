using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Reports.Filters
{
    public class CollectionReportFilter
    {
        public required Guid EventId { get; init; }
        public DateTimeOffset? PaidFromDate { get; init; }
        public DateTimeOffset? PaidToDate { get; init; }
        public DateTimeOffset? SpecificDate { get; init; }
        public IEnumerable<Guid>? PassCategoryIds { get; init; }
        public IEnumerable<string>? RegistrationUserIds { get; init; }
        public IEnumerable<Guid?>? CounterIds { get; init; }
    }
}
