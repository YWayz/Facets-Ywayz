using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.Payments.Entities;

public sealed class PaymentGatewayNotification : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public string? TransactionId { get; private set; }

    public string? PLRefNo { get; private set; }

    public int Status { get; private set; }

    public string? StatusMessage { get; private set; }

    public string? AdditionalData { get; private set; }

    public string? DT { get; private set; }
    public string? InvoiceId { get; private set; }

    private PaymentGatewayNotification() { }

    public PaymentGatewayNotification(string? transactionId,
                                      string? plRefNo,
                                      int status,
                                      string? statusMessage,
                                      string? additionalData,
                                      string? dt,
                                      string? invoiceId)
    {
        TransactionId = transactionId;
        PLRefNo = plRefNo;
        Status = status;
        StatusMessage = statusMessage;
        AdditionalData = additionalData;
        DT = dt;
        InvoiceId = invoiceId;
    }
}
