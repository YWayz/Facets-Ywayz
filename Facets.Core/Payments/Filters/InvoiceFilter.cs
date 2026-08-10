using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Filters;

public sealed record InvoiceFilter(IEnumerable<Guid>? EventDateIds, IEnumerable<PaymentStatus>? PaymentStatuses);