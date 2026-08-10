using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Filters;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Counters;

[Route("api/events/{eventId}/registration-counter")]
public sealed class RegistrationCounterController : AdminAppControllerBase
{
    private readonly IRegistrationCounterService _registrationCounterService;

    public RegistrationCounterController(IRegistrationCounterService registrationCounterService)
    {
        _registrationCounterService = registrationCounterService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.Create)]
    [ProducesResponseType(typeof(ResponseResult<RegistrationCounterDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateRegistrationCounter([FromRoute] Guid eventId, [FromBody] CreateRegistrationCounterDto model)
    {
        var response = await _registrationCounterService.CreateRegistrationCounter(eventId, model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetRegistrationCounterById), new { eventId = response.Data!.EventId, id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetRegistrationCounterById))]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<RegistrationCounterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRegistrationCounterById([FromRoute] Guid eventId, [FromRoute] Guid id, CancellationToken token)
    {
        ResponseResult<RegistrationCounterDto> response = await _registrationCounterService.GetRegistrationCounterById(eventId, id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<RegistrationCounterDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRegistrationCounters([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] RegistrationCounterFilter filter, CancellationToken token)
    {
        var response = await _registrationCounterService.GetRegistrationCounters(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateRegistrationCounter([FromRoute] Guid eventId, [FromRoute] Guid id, [FromBody] UpdateRegistrationCounterDto model)
    {
        var response = await _registrationCounterService.UpdateRegistrationCounter(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRegistrationCounter([FromRoute] Guid eventId, [FromRoute] Guid id)
    {
        var response = await _registrationCounterService.Delete(eventId, id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/unlock")]
    [Authorize(policy: ApplicationAuthPolicy.RegistrationCounterPolicy.Unlock)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnlockCounter([FromRoute] Guid eventId, [FromRoute] Guid id)
    {
        var response = await _registrationCounterService.UnlockCounter(eventId, id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}