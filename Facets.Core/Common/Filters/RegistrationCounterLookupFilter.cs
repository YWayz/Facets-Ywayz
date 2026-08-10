using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Common.Filters;

public sealed record RegistrationCounterLookupFilter
{
    public bool? IsLocked { get; init; }
    public IEnumerable<CounterType>? CounterTypes { get; init; }
}
