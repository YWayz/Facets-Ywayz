using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Filters;
using Facets.Core.Counters.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Counters;

[Route("api/registration-counters")]
public sealed class RegistrationCounterAssignmentController : AdminAppControllerBase
{
    private readonly IRegistrationCounterService _registrationCounterService;

    public RegistrationCounterAssignmentController(IRegistrationCounterService registrationCounterService)
    {
        _registrationCounterService = registrationCounterService;
    }

    [HttpPost("{counterId}/counter-assignment")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<UserAssignedRegistrationCounterDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AssignRegistrationCounter([FromRoute] Guid counterId, [FromBody] AssignRegistrationCounterDto model)
    {
        var response = await _registrationCounterService.AssignRegistrationCounter(counterId, model, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }


    [HttpGet("check-assignment")]
    [ProducesResponseType(typeof(ResponseResult<CounterAssignmentStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCounterAssignmentForCurrentUser([FromQuery] CounterAssignmentFilter filter, CancellationToken token)
    {
        var response = await _registrationCounterService.GetCounterAssignmentForCurrentUser(filter, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
