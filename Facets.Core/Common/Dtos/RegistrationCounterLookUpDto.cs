using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Common.Dtos;

public sealed record RegistrationCounterLookUpDto(Guid Id, string Name, string? Description, bool IsLocked, Guid EventId, CounterType CounterType);
