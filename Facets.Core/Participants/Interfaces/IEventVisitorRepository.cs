using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Participants.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Participants.Interfaces;

public interface IEventVisitorRepository : IBaseRepository
{
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorAttendanceSchedule, TResult> specification, CancellationToken token);
    Task<PassVerificationDto?> GetPassVerificationDataBySpec<PassVerificationDto>(ISpecification<VisitorRegistration, PassVerificationDto> specification, CancellationToken token);
    Task<PassTemplateVisitorDto?> GetPassTemplateVisitorBySpec<PassTemplateVisitorDto>(ISpecification<VisitorAttendanceSchedule, PassTemplateVisitorDto> specification, CancellationToken token);


    Task<VisitorAttendanceSchedule?> GetAttendanceToVerify(Guid eventId, Guid evenDateId, Guid attendanceSheduleId, CancellationToken token);
    Task<VisitorPavilionSessionAttendanceSchedule?> GetPavilionSessionAttendanceToVerify(Guid eventId, Guid visitorId, Guid pavilionId, Guid pavilionSessionId, Guid eventDateId, Guid attendanceScheduleId, CancellationToken token);
}
