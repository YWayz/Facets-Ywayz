using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Facets.Api.Policies;

/// <summary>
/// The admin app names the event it is working on in the <c>x-facets-event-id</c> header, and the
/// <c>HasAccessToEvent</c> policy checks that header. Many routes also carry an <c>{eventId}</c>. Before this
/// filter the two were never compared, so a staff user with access to event A could send A in the header and
/// edit event B's pass categories, pavilions or settings through the route. Now a mismatch is refused.
/// </summary>
public sealed class EventScopeConsistencyFilter : IAsyncActionFilter
{
    public const string HeaderName = "x-facets-event-id";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.RouteData.Values.TryGetValue("eventId", out var routeValue)
            && Guid.TryParse(routeValue?.ToString(), out Guid routeEventId))
        {
            string header = context.HttpContext.Request.Headers[HeaderName].ToString();

            if (Guid.TryParse(header, out Guid headerEventId) && headerEventId != routeEventId)
            {
                ErrorResponse error = new()
                {
                    Errors = new List<KeyValuePair<string, IEnumerable<string>>>
                    {
                        new("eventId", new[] { "The event in the request does not match the selected event." })
                    }
                };

                context.Result = new BadRequestObjectResult(error);
                return;
            }
        }

        await next();
    }
}
