using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers;

[Route("api/health-check")]
[ApiController]
[AllowAnonymous]
public sealed class HealthCheckController : ControllerBase
{
    [HttpGet("api")]
    public ActionResult APIHealthCheck(CancellationToken token)
    {
        return Ok();
    }
}
