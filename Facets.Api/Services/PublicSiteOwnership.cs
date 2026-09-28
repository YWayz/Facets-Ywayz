using Facets.Core.Participants.Entities;
using Facets.Core.Payments.Entities;
using Facets.Core.Visitors.Entities;
using Facets.Persistence;
using Facets.SharedKernal;
using Facets.SharedKernal.Helpers;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Facets.Api.Services;

/// <summary>
/// Answers "does the signed-in public-site user own this record?".
///
/// A public-site token is issued after OTP verification of one identity number (NIC or passport),
/// which the token carries as its name claim. A public user may only see or change the visitor
/// with that identity number, and that visitor's registrations, payments and documents.
/// Before this check existed, any signed-in public user could read or edit any visitor by id.
/// </summary>
public interface IPublicSiteOwnership
{
    /// <summary>The NIC or passport number the current token was issued for, or null.</summary>
    string? IdentityNumber { get; }

    /// <summary>True when the caller holds a public-site (visitor) token rather than a staff token.</summary>
    bool IsPublicSiteUser { get; }

    /// <summary>Id of the visitor the current token belongs to, or null.</summary>
    Task<Guid?> GetOwnVisitorId(CancellationToken cancellationToken);

    Task<bool> OwnsVisitor(Guid visitorId, CancellationToken cancellationToken);

    Task<bool> OwnsRegistration(Guid registrationId, CancellationToken cancellationToken);

    Task<bool> OwnsInvoice(Guid invoiceId, CancellationToken cancellationToken);

    /// <summary>
    /// True for a visitor who has just registered online and has not verified their OTP yet.
    /// The public site uploads the profile photo right after the (anonymous) registration and
    /// before OTP verification, so that one step has to work without a token.
    /// </summary>
    Task<bool> IsNewUnverifiedOnlineVisitor(Guid visitorId, CancellationToken cancellationToken);
}

public sealed class PublicSiteOwnership : IPublicSiteOwnership
{
    private static readonly TimeSpan NewVisitorUploadWindow = TimeSpan.FromHours(1);

    private readonly AppDbContext _dbContext;

    private Guid? _ownVisitorId;
    private bool _ownVisitorLoaded;

    public PublicSiteOwnership(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
        _dbContext = dbContext;

        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated is true)
        {
            string? subject = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                              ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IsPublicSiteUser = subject == AppConstants.PublicSite.PublicSiteUserId;

            // Same claim PublicSiteUserAccessRequirementHandler validates the token with.
            IdentityNumber = IdentityNumberHelper.Normalize(user.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                                                            ?? user.FindFirst(ClaimTypes.Name)?.Value);
        }
    }

    public string? IdentityNumber { get; }

    public bool IsPublicSiteUser { get; }

    public async Task<Guid?> GetOwnVisitorId(CancellationToken cancellationToken)
    {
        if (_ownVisitorLoaded) return _ownVisitorId;

        _ownVisitorLoaded = true;

        if (string.IsNullOrWhiteSpace(IdentityNumber)) return null;

        _ownVisitorId = await _dbContext.Set<Visitor>()
                                        .IgnoreQueryFilters()
                                        .Where(v => v.IsDeleted == false
                                                    && (v.NICNumber == IdentityNumber || v.PassportNumber == IdentityNumber))
                                        .OrderByDescending(v => v.CreatedOn)
                                        .Select(v => (Guid?)v.Id)
                                        .FirstOrDefaultAsync(cancellationToken);

        return _ownVisitorId;
    }

    public async Task<bool> OwnsVisitor(Guid visitorId, CancellationToken cancellationToken)
    {
        var ownVisitorId = await GetOwnVisitorId(cancellationToken);

        return ownVisitorId is not null && ownVisitorId == visitorId;
    }

    public async Task<bool> OwnsRegistration(Guid registrationId, CancellationToken cancellationToken)
    {
        var ownVisitorId = await GetOwnVisitorId(cancellationToken);

        if (ownVisitorId is null) return false;

        return await _dbContext.Set<VisitorRegistration>()
                               .IgnoreQueryFilters()
                               .AnyAsync(r => r.Id == registrationId && r.VisitorId == ownVisitorId, cancellationToken);
    }

    public async Task<bool> OwnsInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        var ownVisitorId = await GetOwnVisitorId(cancellationToken);

        if (ownVisitorId is null) return false;

        return await _dbContext.Set<Invoice>()
                               .IgnoreQueryFilters()
                               .AnyAsync(i => i.Id == invoiceId && i.VisitorId == ownVisitorId, cancellationToken);
    }

    public async Task<bool> IsNewUnverifiedOnlineVisitor(Guid visitorId, CancellationToken cancellationToken)
    {
        var createdAfter = DateTimeOffset.UtcNow - NewVisitorUploadWindow;

        return await _dbContext.Set<Visitor>()
                               .IgnoreQueryFilters()
                               .AnyAsync(v => v.Id == visitorId
                                              && v.IsDeleted == false
                                              && v.RegisteredOnline
                                              && v.OTPVerificationRequired
                                              && v.OTPVerified == false
                                              && v.CreatedOn >= createdAfter, cancellationToken);
    }
}
