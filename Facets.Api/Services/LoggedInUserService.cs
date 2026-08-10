using Facets.Core.Security.Claims;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using System.Security.Claims;

namespace Facets.Api.Services;

public sealed class LoggedInUserService : ILoggedInUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly string _emptyUserId = Guid.Empty.ToString();

    public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
    {

        UserEmail = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email)?.Value;

        _httpContextAccessor = httpContextAccessor;

        UserTimeZone = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(f => f.Type == CustomClaimTypes.UserTimeZone)?.Value;

        UserRole = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Role)?.Value;

        UserRole = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Role)?.Value;

        AdminApp = !httpContextAccessor.HttpContext?.Request
                                       .Path.StartsWithSegments($"/api/{APIConstants.APIGroup.Public}", StringComparison.OrdinalIgnoreCase) ?? false;

        UserId = AdminApp ? (httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? _emptyUserId) :
                            AppConstants.PublicSite.PublicSiteUserId;
    }

    public string UserId { get; }

    public string? UserEmail { get; }

    public string? UserTimeZone { get; }

    public string? UserRole { get; }

    public Guid FacetsEventId
    {
        get
        {
            var eventId = _httpContextAccessor.HttpContext?.Request?.Headers["x-facets-event-id"].ToString();

            bool isParsable = Guid.TryParse(eventId?.Trim(), out Guid parsedEventId);

            return isParsable ? parsedEventId : Guid.Empty;
        }

    }

    public bool AdminApp { get; }

    public bool IsAdminUser() => UserRole == AppConstants.Administrator.RoleName;
}
