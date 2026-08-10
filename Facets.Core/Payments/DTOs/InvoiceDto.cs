using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record InvoiceDto(Guid VisitorRegistrationId,
                                Guid VisitorId,
                                Guid EventId,
                                decimal TotalAmount,
                                bool InvoiceCancelled,
                                DateTimeOffset? InvoiceCancelledOn,
                                IReadOnlyList<InvoiceLineItemDto> InvoiceLineItems,
                                Guid PassCategoryId,
                                Guid PassCategorySettingId,
                                DiscountType AppliedDiscountType,
                                bool PassCategorySetToChargable,
                                PaymentStatus PaymentStatus);
