using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.DTOs;

public sealed record CreateRegistrationCounterDto(string Name, string? Description, CounterType CounterType);