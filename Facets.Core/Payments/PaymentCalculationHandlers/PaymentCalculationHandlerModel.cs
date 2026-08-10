using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;

namespace Facets.Core.Payments.PaymentCalculationHandlers;

internal sealed class PaymentCalculationHandlerModel
{
    internal InternalCreateInvoiceDto InternalCreateInvoiceDto { get; }
    private readonly List<InvoiceLineItem> _invoiceLineItems = new();
    internal IReadOnlyList<InvoiceLineItem> InvoiceLineItems => _invoiceLineItems.AsReadOnly();
    internal ProcessedInvoiceData ProcessedInvoiceData { get; set; } = null!;

    public PaymentCalculationHandlerModel(InternalCreateInvoiceDto internalCreateInvoiceDto)
    {
        InternalCreateInvoiceDto = internalCreateInvoiceDto;
    }

    internal void AddLineItem(InvoiceLineItem invoiceLineItem)
    {
        _invoiceLineItems.Add(invoiceLineItem);
    }
}
