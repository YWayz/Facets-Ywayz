using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.Filters;

public sealed class CounterAssignmentFilter
{
    public IEnumerable<CounterType>? CounterTypes { get; init; }
}