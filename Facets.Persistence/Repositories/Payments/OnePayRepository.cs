using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;

namespace Facets.Persistence.Repositories.Payments;
internal sealed class OnePayRepository : BaseRepository, IOnePayRepository
{
    public OnePayRepository(AppDbContext dbContext) : base(dbContext) { }

    public OnePayRequestedPaymentResponseLog AddOnePayRequestedPaymentResponseLog(OnePayRequestedPaymentResponseLog entity)
    {
        var table = _dbContext.Set<OnePayRequestedPaymentResponseLog>();

        table.Add(entity);

        return entity;
    }
}
