using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Payments.Interfaces;

public interface IInvoiceRepository : IBaseRepository
{
    Invoice Add(Invoice newInvoie);

    Task<TResult?> GetProjectedInvoiceBySpec<TResult>(ISpecification<Invoice, TResult> specification, CancellationToken token);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Invoice, TResult> specification, CancellationToken token);

    Task<Invoice?> GetInvoiceBySpec(ISpecification<Invoice> specification, CancellationToken token, bool asTracking = false);

    Task<IReadOnlyList<Invoice>> GetInvoicesBySpec(ISpecification<Invoice> specification, CancellationToken token, bool asTracking = false);


}
