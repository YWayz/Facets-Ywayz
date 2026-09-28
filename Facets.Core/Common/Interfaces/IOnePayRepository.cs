using Facets.Core.Payments.Entities;

namespace Facets.Core.Common.Interfaces;

public interface IOnePayRepository : IBaseRepository
{
    OnePayRequestedPaymentResponseLog AddOnePayRequestedPaymentResponseLog(OnePayRequestedPaymentResponseLog entity);

    /// <summary>
    /// The invoice with everything needed to record a payment (line items, registration schedules), tracked,
    /// including cancelled invoices so the caller can tell "cancelled" apart from "missing".
    /// </summary>
    Task<Invoice?> GetInvoiceForPaymentRecording(Guid invoiceId, CancellationToken token);

    /// <summary>OnePay transaction ids issued for this invoice when payment links were created, newest first.</summary>
    Task<IReadOnlyList<string>> GetRequestedTransactionIds(Guid invoiceId, CancellationToken token);

    Task<bool> IsOnlinePaymentRecorded(string transactionId, CancellationToken token);

    Payment AddPayment(Payment payment);

    PaymentGatewayNotification AddPaymentGatewayNotification(PaymentGatewayNotification notification);

    /// <summary>Unpaid, not cancelled invoices that had a payment link created since <paramref name="since"/>, with their newest transaction id.</summary>
    Task<IReadOnlyList<(Guid InvoiceId, string TransactionId)>> GetUnpaidInvoicesWithPaymentRequests(DateTimeOffset since, CancellationToken token);
}
