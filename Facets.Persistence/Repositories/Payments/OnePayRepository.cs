using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

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

    public async Task<Invoice?> GetInvoiceForPaymentRecording(Guid invoiceId, CancellationToken token)
    {
        return await _dbContext.Set<Invoice>()
                               .IgnoreQueryFilters()
                               .AsTracking()
                               .Include(i => i.InvoiceLineItems)
                               .Include(i => i.VisitorRegistration).ThenInclude(r => r.VisitorAttendanceSchedules)
                               .Include(i => i.VisitorRegistration).ThenInclude(r => r.VisitorPavilionSessionAttendanceSchedules.Where(p => p.IsDeleted == false))
                               .AsSplitQuery()
                               .FirstOrDefaultAsync(i => i.Id == invoiceId, token);
    }

    public async Task<IReadOnlyList<string>> GetRequestedTransactionIds(Guid invoiceId, CancellationToken token)
    {
        return await _dbContext.Set<OnePayRequestedPaymentResponseLog>()
                               .Where(l => l.InvoiceId == invoiceId && l.IPGTransactionId != null)
                               .OrderByDescending(l => l.CreatedOn)
                               .Select(l => l.IPGTransactionId!)
                               .ToListAsync(token);
    }

    public async Task<bool> IsOnlinePaymentRecorded(string transactionId, CancellationToken token)
    {
        return await _dbContext.Set<Payment>().AnyAsync(p => p.IsOnlinePayment && p.CardPaymentReferenceNumber == transactionId, token);
    }

    public Payment AddPayment(Payment payment)
    {
        _dbContext.Set<Payment>().Add(payment);
        return payment;
    }

    public PaymentGatewayNotification AddPaymentGatewayNotification(PaymentGatewayNotification notification)
    {
        _dbContext.Set<PaymentGatewayNotification>().Add(notification);
        return notification;
    }

    public async Task<IReadOnlyList<(Guid InvoiceId, string TransactionId)>> GetUnpaidInvoicesWithPaymentRequests(DateTimeOffset since, CancellationToken token)
    {
        var rows = await _dbContext.Set<OnePayRequestedPaymentResponseLog>()
                                   .Where(l => l.CreatedOn >= since && l.IPGTransactionId != null)
                                   .Join(_dbContext.Set<Invoice>().IgnoreQueryFilters()
                                                                  .Where(i => i.PaymentStatus == PaymentStatus.Unpaid && i.InvoiceCancelled == false),
                                         l => l.InvoiceId,
                                         i => i.Id,
                                         (l, i) => new { l.InvoiceId, l.IPGTransactionId, l.CreatedOn })
                                   .OrderByDescending(x => x.CreatedOn)
                                   .ToListAsync(token);

        return rows.GroupBy(r => r.InvoiceId)
                   .Select(g => (g.Key, g.First().IPGTransactionId!))
                   .ToList();
    }
}
