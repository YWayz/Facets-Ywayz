namespace Facets.Infrastructure.OnePay.Interfaces;

public enum PaymentRecordResult
{
    /// <summary>A verified payment was saved and the invoice is now paid.</summary>
    Recorded,
    /// <summary>The invoice was already paid (or free), or this transaction was already recorded.</summary>
    AlreadyPaid,
    /// <summary>OnePay reports the transaction as not paid.</summary>
    NotPaid,
    /// <summary>No payment link was ever created for this invoice.</summary>
    NoTransaction,
    /// <summary>The transaction does not belong to this invoice, or the amount or currency is wrong.</summary>
    Rejected,
    /// <summary>OnePay confirms payment but the invoice was cancelled; needs manual reconciliation.</summary>
    Unapplied,
    InvoiceNotFound,
}

public interface IOnePayPaymentRecorder
{
    /// <summary>
    /// Verifies the transaction with OnePay and records the payment if it is genuine and covers the invoice.
    /// Returns a result for outcomes that will never change; throws for transient failures so the caller can retry.
    /// </summary>
    Task<PaymentRecordResult> RecordIfPaid(Guid invoiceId, string? transactionId, CancellationToken cancellationToken);

    /// <summary>Re-checks every unpaid invoice that had a payment link created since <paramref name="since"/>. Returns how many were recorded.</summary>
    Task<int> ReconcileUnpaidInvoices(DateTimeOffset since, CancellationToken cancellationToken);
}
