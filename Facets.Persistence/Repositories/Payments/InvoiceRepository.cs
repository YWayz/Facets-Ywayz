using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Payments;

internal sealed class InvoiceRepository : BaseRepository, IInvoiceRepository
{
    private readonly DbSet<Invoice> _table;
    public InvoiceRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<Invoice>();
    }

    public Invoice Add(Invoice newInvoie)
    {
        _table.Add(newInvoie);

        return newInvoie;
    }

    public async Task<Invoice?> GetInvoiceBySpec(ISpecification<Invoice> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var invoice = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return invoice;
    }

    public async Task<IReadOnlyList<Invoice>> GetInvoicesBySpec(ISpecification<Invoice> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var invoices = await query.WithSpecification(specification).ToListAsync(cancellationToken: token);

        return invoices.AsReadOnly();
    }

    public async Task<TResult?> GetProjectedInvoiceBySpec<TResult>(ISpecification<Invoice, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Invoice, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }
}
