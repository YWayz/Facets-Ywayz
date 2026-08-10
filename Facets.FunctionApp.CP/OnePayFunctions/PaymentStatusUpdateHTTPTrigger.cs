using Facets.Core.Payments.Entities;
using Facets.FunctionApp.CP.OnePayFunctions.Models;
using Facets.Persistence;
using Facets.SharedKernal.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using static Facets.SharedKernal.AppEnums;
using static System.Net.WebRequestMethods;

namespace Facets.FunctionApp.CP.OnePayFunctions;

public sealed class PaymentStatusUpdateHTTPTrigger
{
    private readonly ILogger _logger;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private Invoice? _invoice;

    public PaymentStatusUpdateHTTPTrigger(ILoggerFactory loggerFactory, AppDbContext dbContext, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<PaymentStatusUpdateHTTPTrigger>();
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [Function(nameof(OnePayPaymentStatusUpdate))]
    public async Task OnePayPaymentStatusUpdate([HttpTrigger(AuthorizationLevel.Function, Http.Post,Route = FuncAppConstants.OnePay.NotifyURLRoute)]
                                                 HttpRequestData req)
    {
        string? stringBody = string.Empty;
        try
        {
            _logger.LogInformation("Payment notification webhook received");

            stringBody = await new StreamReader(req.Body).ReadToEndAsync();

            var notification = JsonSerializer.Deserialize<OnePayTransactionResult>(stringBody);

            if (notification is null)
            {
                _logger.LogError($"Unable to deseialize the payment update webhook payload");
                throw new OperationFailedException("Payment update payload", "Unable to deserialize the payment update webhook payload");
            }

            var paymentNotification = LogIncomingPayHereNotification(notification);

            await ValidateInvoice(notification);

            AddPaymentInfo(paymentNotification);

            await _dbContext.SaveChangesAsync();
        }
        catch (JsonException ex)
        {
            var keyValues = HttpUtility.ParseQueryString(stringBody);

            int.TryParse(keyValues["status"], out int statusCode);

            OnePayTransactionResult error = new()
            {
                Status = statusCode,
                AdditionalData = null,
                DT = null,
                PLRefNo = null,
                StatusMessage = keyValues["status_message"],
                TransactionId = keyValues["transaction_id"],
            };
            var paymentNotification = LogIncomingPayHereNotification(error);
            await _dbContext.SaveChangesAsync();
        }

        catch (Exception e)
        {
            _logger.LogError($"Error Msg: {e.InnerException?.Message ?? e.Message}");
            throw;
        }
    }

    private void AddPaymentInfo(PaymentGatewayNotification paymentNotification)
    {
        if (paymentNotification.Status is not FuncAppConstants.OnePay.SuccessCode) return;

        Payment payment = new(PaymentMethod.Card,
                              "n/a",
                              paymentNotification.TransactionId,
                              _invoice!,
                              isOnlinePayment: true);

        _dbContext.Set<Payment>().Add(payment);
    }

    private PaymentGatewayNotification LogIncomingPayHereNotification(OnePayTransactionResult notification)
    {
        string? invoiceIdAsString = GetInvoiceId(notification);

        PaymentGatewayNotification paymentNotification = new(transactionId: notification.TransactionId,
                                                             plRefNo: notification.PLRefNo,
                                                             status: notification.Status,
                                                             statusMessage: notification.StatusMessage,
                                                             additionalData: notification.AdditionalData,
                                                             dt: notification.DT,
                                                             invoiceIdAsString);

        _dbContext.Set<PaymentGatewayNotification>().Add(paymentNotification);

        return paymentNotification;
    }

    private async Task ValidateInvoice(OnePayTransactionResult notification)
    {
        string? invoiceIdAsString = GetInvoiceId(notification);

        if (Guid.TryParse(invoiceIdAsString, out Guid invoiceId) is false)
        {
            _logger.LogError($"Invalid invoice id: {invoiceIdAsString}");
            throw new BadRequestException(nameof(invoiceId), "Invalid invoice id");
        }

        var invoice = await _dbContext.Set<Invoice>()
                                      .IgnoreQueryFilters()
                                      .AsTracking()
                                      .Include(i => i.VisitorRegistration.VisitorAttendanceSchedules)
                                      .Include(i => i.VisitorRegistration.VisitorPavilionSessionAttendanceSchedules)
                                      .AsSplitQuery()
                                      .FirstOrDefaultAsync(w => w.Id == invoiceId && w.InvoiceCancelled == false);

        if (invoice is null)
        {
            _logger.LogError($"Invoice id ({invoiceId}) was not found");
            throw new NotFoundException(nameof(invoice.Id), "Invoice", invoiceId);
        }

        if (invoice.PaymentStatus is PaymentStatus.Paid)
        {
            _logger.LogError($"Invoice id ({invoiceId}) is already paid");
            throw new OperationFailedException("Invoice", $"Invoice id ({invoiceId}) is already paid");
        }

        _invoice = invoice;

        _logger.LogInformation($"Valid invoice id: {invoice.Id}");
    }

    private static string? GetInvoiceId(OnePayTransactionResult notification)
    {
        return notification.AdditionalData?.Split(";")[0].Split(":")[1] ?? null;
    }
}
