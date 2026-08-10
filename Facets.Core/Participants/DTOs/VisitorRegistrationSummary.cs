using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.DTOs;

public sealed record VisitorRegistrationSummary(string Firstame,
                                                string Lastame,
                                                string? NICNumber,
                                                string? PassportNumber,
                                                string CountryName,
                                                VisitorStatus VisitorStatus,
                                                bool RegistrationCancelled,
                                                DateTimeOffset? RegistrationCancelledOn,
                                                VisitorIdentityType VisitorIdentityType,
                                                Guid EventId,
                                                string MobileNumber,
                                                Guid VisitorId,
                                                Guid Id);
