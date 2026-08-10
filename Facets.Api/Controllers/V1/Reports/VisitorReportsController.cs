using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.Core.Reports.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Reports;

[Route("api/reports")]
public sealed class VisitorReportsController : AdminAppControllerBase
{
    private readonly IReportService _reportService;

    public VisitorReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }


    [HttpGet("visitor")]
    [Authorize(policy: ApplicationAuthPolicy.ReportPolicy.VisitorReport)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> VisitorsReport([FromQuery] Paginator paginator, [FromQuery] VisitorReportFilter filter, CancellationToken token)
    {
        var response = await _reportService.VisitorReport(paginator, filter, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("collection")]
    [Authorize(policy: ApplicationAuthPolicy.ReportPolicy.CollectionReport)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<CollectionReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> CollectionReport([FromQuery] CollectionReportFilter filter, CancellationToken token)
    {
        var response = await _reportService.CollectionReport(filter, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
