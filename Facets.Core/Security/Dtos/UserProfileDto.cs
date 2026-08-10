namespace Facets.Core.Security.Dtos;

public sealed class UserProfileDto
{
    public Guid Id { init; get; }
    public string Email { init; get; } = null!;
    public string UserName { init; get; } = null!;
    public string FirstName { init; get; } = null!;
    public string LastName { init; get; } = null!;
    public string TimeZone { init; get; } = null!;
    public string? ImageURL { get; init; }

    public IReadOnlyList<string> Roles { get; private set; } = new List<string>();

    public void SetRoles(IEnumerable<string> roles)
    {
        Roles = roles.ToList();
    }
}