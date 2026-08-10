using Facets.Core.Participants.Entities;
using Facets.Core.Payments.Entities;
using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.Core.Reports.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Reports;

internal sealed class VisitorReportRepository : IVisitorReportRepository
{
    private readonly AppDbContext _context;

    public VisitorReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorReportDto>>> VisitorReport(Paginator paginator, VisitorReportFilter filter, CancellationToken token)
    {
        var visitorAttendanceQuery = _context.Set<VisitorRegistration>().Where(p => p.EventId == filter.EventId && p.RegistrationCancelled == false);

        if (filter.EventDateIds is not null)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(p => p.VisitorAttendanceSchedules.Any(a => filter.EventDateIds!.Contains(a.EventDateId)));

        if (filter.VisitorEventRegistrationSpecificDate.HasValue)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => w.CreatedOn.Date == filter.VisitorEventRegistrationSpecificDate.Value.Date);

        if (filter.VisitorEventRegistrationFromDate.HasValue)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => w.CreatedOn >= filter.VisitorEventRegistrationFromDate.Value.Date);

        if (filter.VisitorEventRegistrationToDate.HasValue)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => w.CreatedOn <= filter.VisitorEventRegistrationToDate.Value.Date);

        if (filter.PassCategoryIds is not null)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => filter.PassCategoryIds.Contains(w.PassCategoryId));

        if (filter.VisitorCountryIds is not null)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => filter.VisitorCountryIds.Contains(w.Visitor.CountryId));

        if (filter.VisitorStatuses is not null)
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => filter.VisitorStatuses.Contains(w.Visitor.VisitorStatus));

        var totalRecordCount = await visitorAttendanceQuery.CountAsync(cancellationToken: token);

        var visitorResult = await visitorAttendanceQuery
                                        .Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                        .Take(paginator.PageSize)
                                        .Select(s => new VisitorReportDto
                                        {
                                            FirstName = s.Visitor.FirstName,
                                            LastName = s.Visitor.LastName,
                                            Identification = s.Visitor.VisitorIdentityType == VisitorIdentityType.NIC ? s.Visitor.NICNumber! :
                                                                                                                        s.Visitor.PassportNumber!,
                                            Company = s.Visitor.CompanyName,
                                            Country = s.Visitor.Country.Name,
                                            Email = s.Visitor.Email,
                                            MobileNo = s.Visitor.MobileNumber,
                                            VisitorStatus = s.Visitor.VisitorStatus,
                                            PassCategory = s.PassCategory.Name
                                        }).ToListAsync(token);

        return new ResponseResult<IReadOnlyList<VisitorReportDto>>(visitorResult, totalRecordCount);
    }

    public async Task<ResponseResult<IReadOnlyList<CollectionReportDto>>> VisitorCollectionReport(CollectionReportFilter filter, CancellationToken token)
    {

        var invoiceQuery = _context.Set<Invoice>()
                                   .Where(p => p.EventId == filter.EventId &&
                                               p.TotalAmount != 0M &&
                                               p.PaymentStatus == PaymentStatus.Paid &&
                                               p.InvoiceCancelled == false);
        var paymentQuery = _context.Set<Payment>()
                           .GroupBy(p => p.PaymentMethod)
                           .Select(g => new
                           {
                               PaymentMethod = g.Key,
                               PaymentCount = g.Count(),
                               TotalInvoiceAmount = g.Sum(p => p.InvoiceAmount)
                           })
                           .ToList();

        if (filter.PaidFromDate.HasValue)
            invoiceQuery = invoiceQuery.Where(w => w.CreatedOn.Date >= filter.PaidFromDate.Value.Date);

        if (filter.PaidToDate.HasValue)
            invoiceQuery = invoiceQuery.Where(w => w.CreatedOn.Date <= filter.PaidToDate.Value.Date);

        if (filter.SpecificDate.HasValue)
            invoiceQuery = invoiceQuery.Where(w => w.CreatedOn.Date == filter.SpecificDate.Value.Date);

        if (filter.CounterIds is not null)
            invoiceQuery = invoiceQuery.Where(p => filter.CounterIds.Contains(p.RegistrationCounterId));

        if (filter.PassCategoryIds is not null)
            invoiceQuery = invoiceQuery.Where(w => filter.PassCategoryIds.Contains(w.PassCategoryId));

        if (filter.RegistrationUserIds is not null)
            invoiceQuery = invoiceQuery.Where(w => filter.RegistrationUserIds.Contains(w.CreatedBy));

        //var visitorCollectionResult = await invoiceQuery.GroupBy(p => p.VisitorRegistrationCounter.Name)
        //                                                .Select(p => new CollectionReportDto()
        //                                                {
        //                                                    Description = p.Key ?? "Online",
        //                                                    RegisteredCount = p.Count(),
        //                                                    Amount = p.Sum(s => s.TotalAmount)
        //                                                }).ToListAsync(token);

        //return new ResponseResult<IReadOnlyList<CollectionReportDto>>(visitorCollectionResult);
        var visitorCollectionResult = await invoiceQuery.GroupBy(p => p.VisitorRegistrationCounter.Name)
                                                .Select(p => new CollectionReportDto()
                                                {
                                                    Description = p.Key ?? "Online",
                                                    RegisteredCount = p.Count(),
                                                    Amount = p.Sum(s => s.TotalAmount),
                                                    CardPayment = p.SelectMany(i => i.Payments)
                                                                   .Count(s => s.PaymentMethod == PaymentMethod.Card),
                                                    CardAmount = p.SelectMany(i => i.Payments)
                                                                  .Where(s => s.PaymentMethod == PaymentMethod.Card)
                                                                  .Sum(s => s.Amount),
                                                    CashPayment = p.SelectMany(i => i.Payments)
                                                                   .Count(s => s.PaymentMethod == PaymentMethod.Cash),
                                                    CashAmount = p.SelectMany(i => i.Payments)
                                                                  .Where(s => s.PaymentMethod == PaymentMethod.Cash)
                                                                  .Sum(s => s.Amount)
                                                }).ToListAsync(token);

        return new ResponseResult<IReadOnlyList<CollectionReportDto>>(visitorCollectionResult);




    }
}
