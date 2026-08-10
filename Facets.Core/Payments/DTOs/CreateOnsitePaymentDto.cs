using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record CreateOnsitePaymentDto(Guid RegistrationId,
                                            decimal Amount,
                                            Guid VisitorId,
                                            PaymentMethod PaymentMethod,
                                            string? LastFourDigitsofCard,
                                            string? ReferenceNumber);
