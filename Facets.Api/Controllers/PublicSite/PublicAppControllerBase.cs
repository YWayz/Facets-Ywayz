using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[ApiExplorerSettings(GroupName = APIConstants.APIGroup.Public)]
public abstract class PublicAppControllerBase : AppControllerBase { }