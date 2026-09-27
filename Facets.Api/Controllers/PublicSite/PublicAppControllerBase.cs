using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[ApiExplorerSettings(GroupName = APIConstants.APIGroup.Public)]
public abstract class PublicAppControllerBase : AppControllerBase
{
    /// <summary>
    /// Response for a record that exists but belongs to someone else. Returns 404 rather than 403
    /// so the public API does not reveal whether another person's record exists.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    protected ObjectResult NotOwnedResponse(string entityName, object key)
    {
        return UnsuccessfullResponse(new ResponseResult(new NotFoundException(entityName, entityName, key)));
    }
}
