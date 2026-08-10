using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Entities;

public sealed class Payment : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? LastFourDigitsOfCreditCard { get; private set; }

    public string? CardPaymentReferenceNumber { get; private set; }

    public bool IsOnlinePayment { get; private set; }

    public Invoice Invoice { get; private set; } = null!;
    public Guid InvoiceId { get; private set; }

    public decimal InvoiceAmount { get; set; }

    private Payment() { }

    public Payment(PaymentMethod paymentMethod,
                   string? lastFourDigitsOfCreditCard,
                   string? cardPaymentReferenceNumber,
                   Invoice invoice,
                   bool isOnlinePayment)
    {
        Amount = invoice.TotalAmount;
        IsOnlinePayment = isOnlinePayment;

        InvoiceId = invoice.Id;
        InvoiceAmount = invoice.TotalAmount;

        PaymentMethod = InvoiceAmount is not 0 ? paymentMethod : PaymentMethod.NoPaymentNeeded;

        if (PaymentMethod is PaymentMethod.Card)
        {
            LastFourDigitsOfCreditCard = lastFourDigitsOfCreditCard;
            CardPaymentReferenceNumber = cardPaymentReferenceNumber;
        }

        invoice.MarkAsPaid();
    }
}
