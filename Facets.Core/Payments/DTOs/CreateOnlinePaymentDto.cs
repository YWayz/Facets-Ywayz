using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record CreateOnlinePaymentDto(Guid RegistrationId,
                                            decimal Amount,
                                            Guid VisitorId,
                                            string? LastFourDigitsOfCard,
                                            string? ReferenceNumber);
