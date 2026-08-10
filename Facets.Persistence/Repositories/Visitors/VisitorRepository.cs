using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Participants.Entities;
using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Facets.Persistence.Repositories.Visitors;

internal sealed class VisitorRepository : BaseRepository, IVisitorRepository
{
    private readonly DbSet<Visitor> _table;
    private readonly AppDbContext _context;

    public VisitorRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<Visitor>();
        _context = dbContext;
    }

    public Visitor Add(Visitor visitor)
    {
        _table.Add(visitor);

        return visitor;
    }

    public async Task<TResult?> GetProjectedVisitorBySpec<TResult>(ISpecification<Visitor, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<Visitor?> GetVisitorBySpec(ISpecification<Visitor> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var teamMember = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return teamMember;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Visitor, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public void AddVisitorActivity(VisitorActivity visitorActivity)
    {
        _dbContext.Set<VisitorActivity>().Add(visitorActivity);
    }

    public async Task<bool> VisitorRegistered(string identificationNumber, CancellationToken cancellationToken)
    {
        var visitorRegistered = await _table.AnyAsync(f => f.NICNumber == identificationNumber ||
                                                           f.PassportNumber == identificationNumber,
                                                      cancellationToken);

        return visitorRegistered;
    }


    public async Task<ResponseResult<IReadOnlyList<AttendenceReportDto>>> AttendanceCollectionReport(Paginator paginator, AttendancereportFilter filter, CancellationToken token)
    {
        var visitorAttendanceQuery = _context.Set<VisitorRegistration>()
       .Where(p => p.EventId == filter.EventId && p.RegistrationCancelled == false);

        if (filter.EventDateIds != null && filter.EventDateIds.Any())
        {
            visitorAttendanceQuery = visitorAttendanceQuery.Where(p =>
                p.VisitorAttendanceSchedules.Any(a => filter.EventDateIds.Contains(a.EventDateId)));
        }

        if (filter.VisitorEventRegistrationSpecificDate.HasValue)
        {
            var specificDate = filter.VisitorEventRegistrationSpecificDate.Value.Date;
            visitorAttendanceQuery = visitorAttendanceQuery.Where(w => w.CreatedOn.Date == specificDate);
        }

        var visitorResult = await visitorAttendanceQuery
            .Where(s => s.VisitorAttendanceSchedules.Any(vas => vas.VisitorAttended))
            .Skip((paginator.PageNumber - 1) * paginator.PageSize)
            .Take(paginator.PageSize)
            .Select(s => new AttendenceReportDto
            {
                FirstName = s.Visitor.FirstName,
                LastName = s.Visitor.LastName,
                Identification = s.Visitor.VisitorIdentityType == VisitorIdentityType.NIC
                    ? s.Visitor.NICNumber!
                    : s.Visitor.PassportNumber!,
                Country = s.Visitor.Country.Name,
                PassCategory = s.PassCategory.Name,
                MobileNo = s.Visitor.MobileNumber,
                Email = s.Visitor.Email,
                Company = s.Visitor.CompanyName,
                VisitorStatus = s.Visitor.VisitorStatus,
                TotalCount = s.VisitorAttendanceSchedules.Count(vas => vas.VisitorAttended),
            })
            .ToListAsync(token);

        var totalRecordCount = await visitorAttendanceQuery.CountAsync(cancellationToken: token);
        return new ResponseResult<IReadOnlyList<AttendenceReportDto>>(visitorResult, totalRecordCount);
    }
  }
