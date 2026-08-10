using Facets.Core.Reports.Filters;
using Facets.Core.Reports.Interfaces;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Visitors
{
    [Route("api/attendancereport")]
    

    public class VisitersReportController : AdminAppControllerBase
    {
        private readonly IVisitorService _visitorService;

        public VisitersReportController(IVisitorService visitorService)
        {
                _visitorService = visitorService;
        }

        [HttpGet("visitor")]
        //[Authorize(policy: ApplicationAuthPolicy.ReportPolicy.VisitorReport)]
        //[ProducesResponseType(typeof(ResponseResult<IReadOnlyList<AttendenceReportDto>>), StatusCodes.Status200OK)]

        public async Task<ActionResult> AttendancReport([FromQuery] Paginator paginator, [FromQuery] AttendancereportFilter filter, CancellationToken token)
        {
            var response = await _visitorService.AttendanceReport(paginator, filter, token);
            return response.Success ? Ok(response) : UnsuccessfullResponse(response);
        }

    }
}
