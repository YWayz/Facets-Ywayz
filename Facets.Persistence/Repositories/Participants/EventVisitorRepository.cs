using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Participants;

internal sealed class EventVisitorRepository : BaseRepository, IEventVisitorRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<VisitorAttendanceSchedule> _table;
    private readonly DbSet<VisitorPavilionSessionAttendanceSchedule> _tablePavilionSessionAttendance;
    private readonly DbSet<VisitorRegistration> _visitorRegistrationTable;

    public EventVisitorRepository(AppDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
        _table = dbContext.Set<VisitorAttendanceSchedule>();
        _tablePavilionSessionAttendance = dbContext.Set<VisitorPavilionSessionAttendanceSchedule>();
        _visitorRegistrationTable = dbContext.Set<VisitorRegistration>();
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorAttendanceSchedule, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<PassVerificationDto?> GetPassVerificationDataBySpec<PassVerificationDto>(ISpecification<VisitorRegistration, PassVerificationDto> specification, CancellationToken token)
    {
        return await _visitorRegistrationTable.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<PassTemplateVisitorDto?> GetPassTemplateVisitorBySpec<PassTemplateVisitorDto>(ISpecification<VisitorAttendanceSchedule, PassTemplateVisitorDto> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<VisitorAttendanceSchedule?> GetAttendanceToVerify(Guid eventId, Guid evenDateId, Guid attendanceSheduleId, CancellationToken token)
    {
        var query = _table.Where(w => w.Id == attendanceSheduleId &&
                                      w.VisitorRegistration.EventId == eventId &&
                                      w.EventDateId == evenDateId &&
                                      w.Cancelled == false &&
                                      w.VisitorRegistration.Visitor.VisitorStatus == VisitorStatus.Active &&
                                      w.CanAttend == true &&
                                      w.IsInvoiced == true);

        var attendance = await query.AsTracking().FirstOrDefaultAsync(token);

        return attendance;
    }

    public async Task<VisitorPavilionSessionAttendanceSchedule?> GetPavilionSessionAttendanceToVerify(Guid eventId, Guid visitorId, Guid pavilionId, Guid pavilionSessionId, Guid eventDateId, Guid attendanceScheduleId, CancellationToken token)
    {
        var query = _tablePavilionSessionAttendance.Where(w => w.Id == attendanceScheduleId &&
                                                               w.VisitorRegistration.EventId == eventId &&
                                                               w.PavilionSession.EventDateId == eventDateId &&
                                                               w.PavilionSession.PavilionId == pavilionId &&
                                                               w.PavilionSessionId == pavilionSessionId &&
                                                               w.Cancelled == false &&
                                                               w.VisitorRegistration.Visitor.VisitorStatus == VisitorStatus.Active &&
                                                               w.CanAttend == true &&
                                                               w.IsInvoiced == true);

        var visitorPavilionSessionAttendanceSchedule = await query.AsTracking().FirstOrDefaultAsync(token);

        return visitorPavilionSessionAttendanceSchedule;
    }
}
