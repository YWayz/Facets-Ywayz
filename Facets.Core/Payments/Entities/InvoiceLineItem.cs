using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Entities;

public sealed class InvoiceLineItem : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;

    public Guid ItemId { get; private set; }

    public decimal Amount { get; private set; }

    public InvoiceLineItemType LineItemType { get; private set; }

    private InvoiceLineItem() { }

    public InvoiceLineItem(Guid itemId, decimal amount, InvoiceLineItemType lineItemType)
    {
        ItemId = itemId;
        Amount = amount;
        LineItemType = lineItemType;
    }

    internal void Deleted()
    {
        IsDeleted = true;
    }
}
