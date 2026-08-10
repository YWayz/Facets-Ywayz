using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs
{
    public class AttendenceReportDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Identification { get; init; }
        public required string Country { get; init; }
        public required string PassCategory { get; init; }
        public required string MobileNo { get; init; }
        public required string? Email { get; init; }
        public required string? Company { get; init; }
        public VisitorStatus VisitorStatus { get; init; }
        public required int TotalCount { get; init; }

        public DateTimeOffset? AttendanceDates { get; set; }

        public bool IsAttendance { get; set; }
    }
}
