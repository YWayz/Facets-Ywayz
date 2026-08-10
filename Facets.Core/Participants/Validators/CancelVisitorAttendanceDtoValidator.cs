using Facets.Core.Participants.DTOs;
using FluentValidation;

namespace Facets.Core.Participants.Validators;

public sealed class CancelVisitorAttendanceDtoValidator : AbstractValidator<CancelVisitorAttendanceDto>
{
    public CancelVisitorAttendanceDtoValidator()
    {
        RuleFor(r => r.AttendanceScheduleIds)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Attendance Schedule IDs cannot be null")
            .NotEmpty().WithMessage("Attendance Schedule cannot be an empty list")
            .Must(attendanceSchdeuledIds => attendanceSchdeuledIds.All(attendanceScheduledId => Guid.Empty != attendanceScheduledId))
            .WithMessage("Attendance Schedule cannot contain empty guids")
            .Must((attendanceSchdeuledIds) =>
            {
                var anyDuplicate = attendanceSchdeuledIds.GroupBy(x => x).Any(g => g.Count() > 1);

                return !anyDuplicate;
            }).WithMessage("Attendance Schedule IDs cannot be duplicated");
    }
}
