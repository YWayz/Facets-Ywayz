using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Payments;

internal sealed class PaymentRepository : BaseRepository, IPaymentRepository
{
    private readonly DbSet<Payment> _table;
    public PaymentRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<Payment>();
    }

    public Payment Add(Payment payment)
    {
        _table.Add(payment);

        return payment;
    }
}
