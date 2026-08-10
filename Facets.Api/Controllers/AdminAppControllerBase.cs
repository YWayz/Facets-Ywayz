using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Facets.Api.Controllers;

[ApiExplorerSettings(GroupName = APIConstants.APIGroup.Admin)]
public abstract class AdminAppControllerBase : AppControllerBase { }
