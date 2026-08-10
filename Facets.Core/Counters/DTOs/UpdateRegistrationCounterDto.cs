using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.DTOs;

public sealed record UpdateRegistrationCounterDto(string Name, string? Description, CounterType CounterType);