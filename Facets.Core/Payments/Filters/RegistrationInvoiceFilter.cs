using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Filters;

public sealed record RegistrationInvoiceFilter(IEnumerable<PaymentStatus>? PaymentStatuses, IEnumerable<Guid>? InvoiceIds);
