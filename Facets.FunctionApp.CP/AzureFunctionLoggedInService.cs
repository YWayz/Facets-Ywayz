using Facets.SharedKernal.Interfaces;

namespace Facets.FunctionApp.CP;

internal class AzureFunctionLoggedInService : ILoggedInUserService
{
    public string UserId { get; } = "Azure Function";

    public string? UserEmail { get; } = "Azure Function";

    public string? UserTimeZone { get; } = "Azure Function";

    public string? UserRole { get; } = "Azure Function";

    public Guid FacetsEventId => Guid.Empty;

    public bool AdminApp { get; } = false;

    public bool IsAdminUser() => false;
}
