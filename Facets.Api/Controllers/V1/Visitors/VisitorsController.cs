using Facets.Core.Security.AuthPolicies;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Visitors;

[Route("api/visitors")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class VisitorsController : AdminAppControllerBase
{
    private readonly IVisitorStore _visitorStore;
    private readonly IVisitorService _visitorService;

    public VisitorsController(IVisitorStore visitorStore, IVisitorService visitorService)
    {
        _visitorStore = visitorStore;
        _visitorService = visitorService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.OnsiteRegistration)]
    [ProducesResponseType(typeof(ResponseResult<VisitorCreatedDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> RegisterOnsite([FromBody] RegisterVisitorDto model)
    {
        var response = await _visitorStore.Register(model, registerOnline: false, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetVisitorById), new { id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ResponseResult<VisitorSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SearchMember([FromQuery] string? searchValue, CancellationToken token)
    {
        var response = await _visitorStore.SearchVisitor(searchValue, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.OnsiteUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateVisitorOnsite([FromRoute] Guid id, [FromBody] UpdateVisitorDto model)
    {
        var response = await _visitorService.UpdateVisitor(id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetVisitorById))]
    [ProducesResponseType(typeof(ResponseResult<VisitorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVisitorById([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _visitorService.GetVisitorById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitors([FromQuery] Paginator paginator, [FromQuery] VisitorFilter filter, CancellationToken token)
    {
        var response = await _visitorService.GetVisitors(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/mark-as-blacklisted")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.Blacklist)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkAsBlackListed([FromRoute] Guid id, [FromBody] MarkAsBlackListedDto model)
    {
        var response = await _visitorService.MarkAsBlackListed(id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/remove-from-blacklist")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.Blacklist)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveFromBlacklist([FromRoute] Guid id)
    {
        var response = await _visitorService.RemoveFromBlackList(id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("{visitorId}/activities")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorActivityDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMembersByEvent([FromRoute] Guid visitorId, [FromQuery] Paginator paginator, CancellationToken token)
    {
        var response = await _visitorService.GetVisitorActivities(paginator, visitorId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
