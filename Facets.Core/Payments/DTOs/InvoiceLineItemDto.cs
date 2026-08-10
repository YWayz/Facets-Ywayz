using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record InvoiceLineItemDto(DateTimeOffset CreatedOn,
                                        Guid InvoiceId,
                                        Guid ItemId,
                                        decimal Amount,
                                        Guid EventId,
                                        Guid VisitorId,
                                        InvoiceLineItemType LineItemType);
