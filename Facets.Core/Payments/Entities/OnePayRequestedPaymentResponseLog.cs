using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.Payments.Entities;

public sealed class OnePayRequestedPaymentResponseLog : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public string? IPGTransactionId { get; private set; }

    public decimal GrossAmount { get; private set; }

    public decimal HandlingFee { get; private set; }

    public decimal NetAmount { get; private set; }

    public string? Currency { get; private set; }

    public Guid InvoiceId { get; private set; }
    public string InvoiceReferenceNumber { get; private set; }

    private OnePayRequestedPaymentResponseLog() { }

    public OnePayRequestedPaymentResponseLog(string? ipgTransactionId,
                                             decimal grossAmount,
                                             decimal handlingFee,
                                             decimal netAmount,
                                             string? currency,
                                             Guid invoiceId,
                                             string invoiceReferenceNumber)
    {
        IPGTransactionId = ipgTransactionId;
        GrossAmount = grossAmount;
        HandlingFee = handlingFee;
        NetAmount = netAmount;
        Currency = currency;
        InvoiceId = invoiceId;
        InvoiceReferenceNumber = invoiceReferenceNumber;
    }
}
