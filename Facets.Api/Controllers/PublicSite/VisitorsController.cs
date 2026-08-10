using Facets.Core.Common.Dtos;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/visitors")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class VisitorsController : PublicAppControllerBase
{
    private readonly IVisitorStore _visitorStore;
    private readonly IVisitorService _visitorService;

    public VisitorsController(IVisitorStore visitorStoreService, IVisitorService visitorService)
    {
        _visitorStore = visitorStoreService;
        _visitorService = visitorService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ResponseResult<VisitorSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SearchVisitor([FromQuery] string? searchValue, CancellationToken token)
    {
        var response = await _visitorStore.PublicSearchVisitor(searchValue, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<VisitorCreatedDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> RegisterOnline([FromBody] RegisterVisitorDto model)
    {
        var response = await _visitorStore.Register(model, registerOnline: true, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateVisitorOnline([FromRoute] Guid id, [FromBody] UpdateVisitorDto model)
    {
        var response = await _visitorService.UpdateVisitor(id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("{identificationNumber}/availability-check")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<VisitorAvailabilityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AvailabilityCheck([FromRoute] string identificationNumber, CancellationToken token)
    {
        var response = await _visitorStore.AvailabilityCheck(identificationNumber, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetPublicVisitorById))]
    [ProducesResponseType(typeof(ResponseResult<VisitorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPublicVisitorById([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _visitorService.GetVisitorById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost("{visitorId}/documents")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<FileDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddDocumentsToExistingVisitor([FromRoute] Guid visitorId, [FromForm] List<KeyValuePair<AttachmentType, IFormFile>> files)
    {
        var response = await _visitorService.AddDocumentsToExistingVisitor(visitorId, files, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetPublicVisitorDocuments), new { visitorId = visitorId }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{visitorId}/documents", Name = nameof(GetPublicVisitorDocuments))]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<FileDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPublicVisitorDocuments([FromRoute] Guid visitorId, [FromQuery] VisitorDocumentFilter filter, CancellationToken token)
    {
        var response = await _visitorService.GetDocuments(visitorId, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
