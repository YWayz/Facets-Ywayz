using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Infrastructure.OnePay.Services;

/// <summary>
/// The one place that turns "OnePay says this transaction is paid" into a recorded payment.
/// Used by the webhook, by the reconciliation timer, and by the payment result page, so every path
/// applies the same checks: the invoice is open, the transaction was issued for this invoice, OnePay's
/// status API confirms it, and the amount covers the invoice total.
/// </summary>
internal sealed class OnePayPaymentRecorder : IOnePayPaymentRecorder
{
    private readonly IOnePayRepository _repository;
    private readonly IOnePayAPIService _onePayApi;
    private readonly ILogger<OnePayPaymentRecorder> _logger;

    public OnePayPaymentRecorder(IOnePayRepository repository, IOnePayAPIService onePayApi, ILogger<OnePayPaymentRecorder> logger)
    {
        _repository = repository;
        _onePayApi = onePayApi;
        _logger = logger;
    }

    public async Task<PaymentRecordResult> RecordIfPaid(Guid invoiceId, string? transactionId, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetInvoiceForPaymentRecording(invoiceId, cancellationToken);

        if (invoice is null)
        {
            _logger.LogError("Payment recording: invoice {InvoiceId} does not exist (transaction {TransactionId})", invoiceId, transactionId);
            return PaymentRecordResult.InvoiceNotFound;
        }

        if (invoice.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Free) return PaymentRecordResult.AlreadyPaid;

        var requestedIds = await _repository.GetRequestedTransactionIds(invoiceId, cancellationToken);

        // No transaction id in the notification: fall back to the newest link we created for this invoice.
        transactionId = string.IsNullOrWhiteSpace(transactionId) ? requestedIds.FirstOrDefault() : transactionId.Trim();

        if (string.IsNullOrWhiteSpace(transactionId)) return PaymentRecordResult.NoTransaction;

        if (await _repository.IsOnlinePaymentRecorded(transactionId, cancellationToken)) return PaymentRecordResult.AlreadyPaid;

        // Ask OnePay. A failure here is transient: throw so the caller retries later.
        var statusResponse = await _onePayApi.GetTransactionStatus(transactionId, cancellationToken);

        if (statusResponse.Success is false)
        {
            string errors = string.Join("; ", statusResponse.Errors.SelectMany(e => e.Value));
            throw new InvalidOperationException($"Could not verify transaction {transactionId} with OnePay: {errors}");
        }

        var status = statusResponse.Data!;

        if (status.Paid is false) return PaymentRecordResult.NotPaid;

        bool belongsToInvoice = requestedIds.Contains(transactionId, StringComparer.OrdinalIgnoreCase)
                                || (string.IsNullOrWhiteSpace(status.IPGTransactionId) is false
                                    && requestedIds.Contains(status.IPGTransactionId, StringComparer.OrdinalIgnoreCase));

        if (belongsToInvoice is false)
        {
            _logger.LogError("Transaction {TransactionId} (OnePay id {IpgId}) was not requested for invoice {InvoiceId}; possible forged notification", transactionId, status.IPGTransactionId, invoiceId);
            return PaymentRecordResult.Rejected;
        }

        bool wrongCurrency = string.IsNullOrWhiteSpace(status.Currency) is false
                             && string.Equals(status.Currency, AppConstants.OnePay.ApplicableCurrency, StringComparison.OrdinalIgnoreCase) is false;

        if (wrongCurrency || status.Amount < invoice.TotalAmount)
        {
            _logger.LogError("Transaction {TransactionId} paid {Amount} {Currency} but invoice {InvoiceId} is {Total} LKR; not recorded", transactionId, status.Amount, status.Currency, invoiceId, invoice.TotalAmount);
            return PaymentRecordResult.Rejected;
        }

        if (invoice.InvoiceCancelled)
        {
            // Verified money for an invoice a later "Pay" click cancelled. Never drop it: keep it visible for finance.
            _logger.LogError("UNAPPLIED PAYMENT: transaction {TransactionId} ({Amount} LKR) is paid but invoice {InvoiceId} was cancelled; needs manual reconciliation", transactionId, status.Amount, invoiceId);
            return PaymentRecordResult.Unapplied;
        }

        _repository.AddPayment(new Payment(PaymentMethod.Card, "n/a", transactionId, invoice, isOnlinePayment: true));

        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateTransaction(ex))
        {
            _logger.LogInformation("Transaction {TransactionId} was recorded concurrently; treated as already paid", transactionId);
            return PaymentRecordResult.AlreadyPaid;
        }

        _logger.LogInformation("Invoice {InvoiceId} marked paid by verified transaction {TransactionId}", invoiceId, transactionId);

        return PaymentRecordResult.Recorded;
    }

    public async Task<int> ReconcileUnpaidInvoices(DateTimeOffset since, CancellationToken cancellationToken)
    {
        var candidates = await _repository.GetUnpaidInvoicesWithPaymentRequests(since, cancellationToken);

        int recorded = 0;

        foreach (var (invoiceId, transactionId) in candidates)
        {
            try
            {
                if (await RecordIfPaid(invoiceId, transactionId, cancellationToken) is PaymentRecordResult.Recorded) recorded++;
            }
            catch (Exception ex)
            {
                // One failure must not stop the sweep; the invoice is picked up again next run.
                _logger.LogWarning(ex, "Reconciliation skipped invoice {InvoiceId}", invoiceId);
            }
        }

        _logger.LogInformation("Payment reconciliation checked {Count} unpaid invoices and recorded {Recorded} payments", candidates.Count, recorded);

        return recorded;
    }

    private static bool IsDuplicateTransaction(DbUpdateException ex)
    {
        return ex.GetType().Name == "UniqueConstraintException"
               && ex.GetBaseException().Message.Contains("IX_Payment_CardPaymentReferenceNumber", StringComparison.OrdinalIgnoreCase);
    }
}
