using Facets.Core.Payments.Entities;
using Facets.FunctionApp.CP.OnePayFunctions.Models;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.Persistence;
using Facets.SharedKernal;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using static Facets.SharedKernal.AppEnums;
using static System.Net.WebRequestMethods;

namespace Facets.FunctionApp.CP.OnePayFunctions;

/// <summary>
/// Receives OnePay payment notifications.
///
/// OnePay v3 callbacks are not signed, so the payload is never trusted on its own:
/// before an invoice is marked paid, the transaction must
///   1. be one we created for this exact invoice (matched against OnePayRequestedPaymentResponseLog), and
///   2. be confirmed as paid, in LKR, for at least the invoice amount by OnePay's /v3/transaction/status/ API.
///
/// Notifications that can never succeed (bad payload, unknown invoice, already paid) are logged and
/// acknowledged so OnePay stops retrying. Transient failures (OnePay status API or database down) throw,
/// so OnePay retries later.
/// </summary>
public sealed class PaymentStatusUpdateHTTPTrigger
{
    private readonly ILogger _logger;
    private readonly AppDbContext _dbContext;
    private readonly IOnePayService _onePayService;

    public PaymentStatusUpdateHTTPTrigger(ILoggerFactory loggerFactory, AppDbContext dbContext, IOnePayService onePayService)
    {
        _logger = loggerFactory.CreateLogger<PaymentStatusUpdateHTTPTrigger>();
        _dbContext = dbContext;
        _onePayService = onePayService;
    }

    [Function(nameof(OnePayPaymentStatusUpdate))]
    public async Task OnePayPaymentStatusUpdate([HttpTrigger(AuthorizationLevel.Function, Http.Post, Route = FuncAppConstants.OnePay.NotifyURLRoute)]
                                                 HttpRequestData req,
                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment notification webhook received");

        string stringBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        OnePayTransactionResult notification = ParseNotification(stringBody);

        // Always keep an audit record of what OnePay sent, even if we reject it below.
        _dbContext.Set<PaymentGatewayNotification>().Add(new PaymentGatewayNotification(transactionId: notification.TransactionId,
                                                                                         plRefNo: notification.PLRefNo,
                                                                                         status: notification.Status,
                                                                                         statusMessage: notification.StatusMessage,
                                                                                         additionalData: notification.AdditionalData,
                                                                                         dt: notification.DT,
                                                                                         GetInvoiceId(notification.AdditionalData)));
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (notification.Status is not FuncAppConstants.OnePay.SuccessCode)
        {
            _logger.LogInformation("Payment notification for transaction {TransactionId} is not a success (status {Status}); nothing to record",
                                   notification.TransactionId, notification.Status);
            return;
        }

        await RecordVerifiedPayment(notification, cancellationToken);
    }

    private async Task RecordVerifiedPayment(OnePayTransactionResult notification, CancellationToken cancellationToken)
    {
        string? transactionId = notification.TransactionId;

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            _logger.LogError("Success notification has no transaction id; ignored");
            return;
        }

        if (Guid.TryParse(GetInvoiceId(notification.AdditionalData), out Guid invoiceId) is false)
        {
            _logger.LogError("Success notification for transaction {TransactionId} has no valid invoice id; ignored", transactionId);
            return;
        }

        var invoice = await _dbContext.Set<Invoice>()
                                      .IgnoreQueryFilters()
                                      .AsTracking()
                                      .Include(i => i.InvoiceLineItems)
                                      .Include(i => i.VisitorRegistration.VisitorAttendanceSchedules)
                                      .Include(i => i.VisitorRegistration.VisitorPavilionSessionAttendanceSchedules)
                                      .AsSplitQuery()
                                      .FirstOrDefaultAsync(w => w.Id == invoiceId && w.InvoiceCancelled == false, cancellationToken);

        if (invoice is null)
        {
            // Ops must see this: money may have been taken for an invoice that was cancelled by a later Pay click.
            _logger.LogError("UNAPPLIED PAYMENT: invoice {InvoiceId} for transaction {TransactionId} was not found or is cancelled; the notification is stored in PaymentGatewayNotification for reconciliation", invoiceId, transactionId);
            return;
        }

        if (invoice.PaymentStatus is PaymentStatus.Paid)
        {
            // OnePay retries notifications; a repeat for an invoice we already recorded is expected.
            _logger.LogInformation("Invoice {InvoiceId} is already paid; duplicate notification for transaction {TransactionId} ignored", invoiceId, transactionId);
            return;
        }

        // Ask OnePay directly. A failure here is treated as transient: throw so OnePay retries.
        var statusResponse = await _onePayService.GetTransactionStatus(transactionId, cancellationToken);

        if (statusResponse.Success is false)
        {
            string errors = string.Join("; ", statusResponse.Errors.SelectMany(e => e.Value));
            throw new InvalidOperationException($"Could not verify transaction {transactionId} with OnePay: {errors}");
        }

        var status = statusResponse.Data!;

        if (status.Paid is false)
        {
            _logger.LogError("OnePay reports transaction {TransactionId} as NOT paid, but the notification claimed success; ignored", transactionId);
            return;
        }

        // The transaction must be one we requested for this invoice (ipg_transaction_id was logged when
        // the payment link was created). This stops a payment made for one invoice from being replayed
        // against another. Both the id in the notification and the id OnePay returns from the status
        // check are accepted, in case OnePay reports the same transaction under two ids.
        var requestedTransactionIds = await _dbContext.Set<OnePayRequestedPaymentResponseLog>()
                                                      .Where(l => l.InvoiceId == invoice.Id && l.IPGTransactionId != null)
                                                      .Select(l => l.IPGTransactionId!)
                                                      .ToListAsync(cancellationToken);

        bool transactionBelongsToInvoice = requestedTransactionIds.Contains(transactionId, StringComparer.OrdinalIgnoreCase)
                                           || (string.IsNullOrWhiteSpace(status.IPGTransactionId) is false
                                               && requestedTransactionIds.Contains(status.IPGTransactionId, StringComparer.OrdinalIgnoreCase));

        if (transactionBelongsToInvoice is false)
        {
            _logger.LogError("Transaction {TransactionId} (OnePay id {IpgTransactionId}) was not requested for invoice {InvoiceId}; possible forged notification, ignored",
                             transactionId, status.IPGTransactionId, invoiceId);
            return;
        }

        // Currency is checked only when OnePay returns it; amount must cover the invoice.
        bool wrongCurrency = string.IsNullOrWhiteSpace(status.Currency) is false
                             && string.Equals(status.Currency, AppConstants.OnePay.ApplicableCurrency, StringComparison.OrdinalIgnoreCase) is false;

        if (wrongCurrency || status.Amount < invoice.TotalAmount)
        {
            _logger.LogError("Transaction {TransactionId} paid {Amount} {Currency} but invoice {InvoiceId} is {InvoiceAmount} {ExpectedCurrency}; not recorded",
                             transactionId, status.Amount, status.Currency, invoiceId, invoice.TotalAmount, AppConstants.OnePay.ApplicableCurrency);
            return;
        }

        bool alreadyRecorded = await _dbContext.Set<Payment>()
                                               .AnyAsync(p => p.IsOnlinePayment && p.CardPaymentReferenceNumber == transactionId, cancellationToken);

        if (alreadyRecorded)
        {
            _logger.LogInformation("Transaction {TransactionId} is already recorded; ignored", transactionId);
            return;
        }

        _dbContext.Set<Payment>().Add(new Payment(PaymentMethod.Card,
                                                  "n/a",
                                                  transactionId,
                                                  invoice,
                                                  isOnlinePayment: true));

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicatePayment(ex))
        {
            // Two notifications for the same transaction raced; the unique index let only one through.
            _logger.LogInformation("Transaction {TransactionId} was recorded by a concurrent notification; ignored", transactionId);
            return;
        }

        _logger.LogInformation("Invoice {InvoiceId} marked as paid by verified transaction {TransactionId}", invoiceId, transactionId);
    }

    private OnePayTransactionResult ParseNotification(string body)
    {
        try
        {
            var notification = JsonSerializer.Deserialize<OnePayTransactionResult>(body);

            if (notification is not null) return notification;
        }
        catch (JsonException)
        {
            // Fall through: OnePay has been seen to post form-encoded bodies for some statuses.
        }

        var keyValues = HttpUtility.ParseQueryString(body);

        int.TryParse(keyValues["status"], out int statusCode);

        _logger.LogWarning("Payment notification body was not JSON; parsed as form data");

        return new OnePayTransactionResult
        {
            Status = statusCode,
            StatusMessage = keyValues["status_message"],
            TransactionId = keyValues["transaction_id"],
            AdditionalData = keyValues["additional_data"],
        };
    }

    /// <summary>
    /// additional_data is what we sent: "invoiceId:{guid};referenceNumber:{ref}".
    /// Returns null instead of throwing when it is missing or malformed.
    /// </summary>
    private static string? GetInvoiceId(string? additionalData)
    {
        if (string.IsNullOrWhiteSpace(additionalData)) return null;

        foreach (var part in additionalData.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var keyValue = part.Split(':', 2, StringSplitOptions.TrimEntries);

            if (keyValue.Length == 2 && keyValue[0].Equals("invoiceId", StringComparison.OrdinalIgnoreCase))
                return keyValue[1];
        }

        return null;
    }

    private static bool IsDuplicatePayment(DbUpdateException ex)
    {
        // EntityFramework.Exceptions (UseExceptionProcessor) surfaces unique index violations as UniqueConstraintException.
        return ex.GetType().Name == "UniqueConstraintException"
               || ex.InnerException?.Message.Contains("IX_Payment_CardPaymentReferenceNumber", StringComparison.OrdinalIgnoreCase) == true;
    }
}
