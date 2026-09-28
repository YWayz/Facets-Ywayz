using Facets.Core.Common.Dtos;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Visitors.Filters;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Api.Controllers.V1.Visitors;

[Route("api/visitors/{visitorId}/documents")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
[Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.View)] // NIC scans and photos: event access alone is not enough
public sealed class VisitorsDocumentsController : AdminAppControllerBase
{
    private readonly IVisitorService _visitorService;

    public VisitorsDocumentsController(IVisitorService visitorService)
    {
        _visitorService = visitorService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<FileDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddDocumentsToExistingVisitor([FromRoute] Guid visitorId, [FromForm] List<KeyValuePair<AttachmentType, IFormFile>> files)
    {
        var response = await _visitorService.AddDocumentsToExistingVisitor(visitorId, files, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetDocuments), new { visitorId = visitorId }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet(Name = nameof(GetDocuments))]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<FileDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDocuments([FromRoute] Guid visitorId, [FromQuery] VisitorDocumentFilter filter, CancellationToken token)
    {
        var response = await _visitorService.GetDocuments(visitorId, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDocument([FromRoute] Guid visitorId, [FromRoute] Guid id)
    {
        var response = await _visitorService.DeleteDocument(visitorId, id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
