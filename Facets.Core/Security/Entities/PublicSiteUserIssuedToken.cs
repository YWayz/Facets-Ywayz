namespace Facets.Core.Security.Entities;

public sealed class PublicSiteUserIssuedToken
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public string UserIdentificationNumber { get; private set; } = null!;
    public string JTI { get; private set; } = null!;

    private PublicSiteUserIssuedToken() { }

    public PublicSiteUserIssuedToken(string userIdentificationNumber, string jti)
    {
        UserIdentificationNumber = userIdentificationNumber;
        JTI = jti;
        CreatedOn = DateTimeOffset.Now;
    }
}
