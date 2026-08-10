using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Filters;

public sealed record RegistrationFilter(string? SearchQuery,
                                        Guid? countryId,
                                        IEnumerable<VisitorStatus>? VisitorStatus,
                                        Guid? visitorId,
                                        IEnumerable<Guid>? EventDateIds,
                                        bool? RegistrationCancelled);
