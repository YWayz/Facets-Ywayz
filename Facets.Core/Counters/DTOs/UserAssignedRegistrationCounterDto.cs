using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.DTOs;

public sealed record UserAssignedRegistrationCounterDto(Guid Id,
                                                        Guid EventId,
                                                        string CounterName,
                                                        bool IsLocked,
                                                        Guid RegistrationCounterId,
                                                        Guid AssignedUserId,
                                                        CounterType CounterType);
