using Facets.Core.Payments.Entities;
using Facets.FunctionApp.CP.OnePayFunctions.Models;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.Persistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using static System.Net.WebRequestMethods;

namespace Facets.FunctionApp.CP.OnePayFunctions;

/// <summary>
/// Receives OnePay payment notifications.
///
/// OnePay v3 callbacks are not signed, so the payload is only a hint: every notification that names a
/// transaction is verified with OnePay's status API and recorded through <see cref="IOnePayPaymentRecorder"/>,
/// the same code path the reconciliation timer and the payment result page use.
///
/// Outcomes that can never change (bad payload, unknown invoice, not paid) are logged and acknowledged so
/// OnePay stops retrying. Transient failures (OnePay or the database unreachable) throw, so OnePay retries.
/// </summary>
public sealed class PaymentStatusUpdateHTTPTrigger
{
    private readonly ILogger _logger;
    private readonly AppDbContext _dbContext;
    private readonly IOnePayPaymentRecorder _recorder;

    public PaymentStatusUpdateHTTPTrigger(ILoggerFactory loggerFactory, AppDbContext dbContext, IOnePayPaymentRecorder recorder)
    {
        _logger = loggerFactory.CreateLogger<PaymentStatusUpdateHTTPTrigger>();
        _dbContext = dbContext;
        _recorder = recorder;
    }

    [Function(nameof(OnePayPaymentStatusUpdate))]
    public async Task OnePayPaymentStatusUpdate([HttpTrigger(AuthorizationLevel.Function, Http.Post, Route = FuncAppConstants.OnePay.NotifyURLRoute)]
                                                 HttpRequestData req,
                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment notification webhook received");

        string stringBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        OnePayTransactionResult notification = ParseNotification(stringBody);

        string? invoiceIdText = GetInvoiceId(notification.AdditionalData);

        // Always keep an audit record of what OnePay sent, whatever happens next.
        _dbContext.Set<PaymentGatewayNotification>().Add(new PaymentGatewayNotification(transactionId: notification.TransactionId,
                                                                                         plRefNo: notification.PLRefNo,
                                                                                         status: notification.Status,
                                                                                         statusMessage: notification.StatusMessage,
                                                                                         additionalData: notification.AdditionalData,
                                                                                         dt: notification.DT,
                                                                                         invoiceIdText));
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (Guid.TryParse(invoiceIdText, out Guid invoiceId) is false)
        {
            _logger.LogError("Notification for transaction {TransactionId} has no valid invoice id; ignored", notification.TransactionId);
            return;
        }

        // Even a "failed" notification is worth a status check: it costs one API call and OnePay's own
        // answer, not the payload, decides.
        var result = await _recorder.RecordIfPaid(invoiceId, notification.TransactionId, cancellationToken);

        _logger.LogInformation("Notification for invoice {InvoiceId}, transaction {TransactionId}: {Result}", invoiceId, notification.TransactionId, result);
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
            // Fall through: some statuses have been seen as form-encoded bodies.
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

    /// <summary>additional_data is what we sent: "invoiceId:{guid};referenceNumber:{ref}". Null when missing or malformed.</summary>
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
}
