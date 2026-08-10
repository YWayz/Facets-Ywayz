using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

internal sealed record InternalCreateInvoiceDto(Guid VisitorRegistrationId,
                                                Guid VisitorId,
                                                Guid EventId,
                                                Dictionary<InvoiceLineItemType, IEnumerable<Guid>> InvoiceLineItemSets,
                                                IEnumerable<Guid> SelectedEventDateIds,
                                                IEnumerable<Guid> EventDateIds,
                                                Guid? RegistrationCounterId,
                                                bool IsOnlinePayment,
                                                bool InvoicedOnSite,
                                                InvoicePaymentInfo InvoicePaymentInfo);
