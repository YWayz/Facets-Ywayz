using Facets.Core.Common.Entities;
using Facets.Core.Common.ValueObjects;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Entities;

public sealed class TeamMember : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public VisitorIdentityType IdentityType { get; private set; }

    public string? NICNumber { get; private set; }
    public string? PassportNumber { get; private set; }

    public Title Title { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string MobileNumber { get; private set; } = null!;
    public string? Email { get; private set; } = null!;
    public AddressValueObject? Address { get; private set; }
    public string? CompanyName { get; private set; } = null!;
    public Guid CountryId { get; private set; }
    public Country Country { get; private set; } = null!;

    public string? ImageURL { get; private set; }

    private readonly List<TeamMemberDocument> _attachments = new();
    public IReadOnlyCollection<TeamMemberDocument> Attachments => _attachments.AsReadOnly();

    public readonly List<TeamMemberEvent> _teamMemberEvents = new();
    public IReadOnlyCollection<TeamMemberEvent> TeamMemberEvents => _teamMemberEvents.AsReadOnly();

    public MemberBlacklistStatus blacklistStatus { get; private set; }


    private TeamMember() { }

    public TeamMember(Title title, string firstName, string lastName, string mobileNumber, string? email, AddressValueObject? address, Guid countryId, string? nic, string? passport, string? companyName, VisitorIdentityType identityType)
    {
        if (identityType is VisitorIdentityType.NIC)
        {
            PassportNumber = null;
            NICNumber = nic;
        }

        else if (identityType is VisitorIdentityType.Passport)
        {
            NICNumber = null;
            PassportNumber = passport;
        }

        Title = title;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        Email = email;
        Address = address;
        CountryId = countryId;
        NICNumber = nic;
        PassportNumber = passport;
        IdentityType = identityType;
        CompanyName = companyName;
    }

    internal void SetImageUrl(string uri) => ImageURL = uri;
    internal void RemoveProfileImage() => ImageURL = null;

    internal void AddDocument(string uri, AttachmentType nic, string uniqueName, string displayName)
    {
        _attachments.Add(new TeamMemberDocument(uri, nic, uniqueName, displayName));
    }

    public void AssignTeamMemberEvent(Guid eventId, Guid passCategoryId)
    {
        _teamMemberEvents.Add(new TeamMemberEvent(eventId, passCategoryId));       
    }
    
    public void UpdateTeamMember(string firstName, string lastName, Title title, string mobileNumber, string? email, AddressValueObject? address, Guid countryId, string? nic, string? passport, string? companyName, VisitorIdentityType identityType)
    {

        if (identityType is VisitorIdentityType.NIC)
        {
            PassportNumber = null;
            NICNumber = nic;
        }

        else if (identityType is VisitorIdentityType.Passport)
        {
            NICNumber = null;
            PassportNumber = passport;
        }

        Title = title;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        Email = email;
        Address = address;
        CountryId = countryId;
        NICNumber = nic;
        PassportNumber = passport;
        IdentityType = identityType;
        CompanyName = companyName;
    }

    internal void MarkAsBlacklisted()
    {
        blacklistStatus = MemberBlacklistStatus.BlackListed;
       
    }
    internal void RemoveBlacklisted()
    {
        blacklistStatus = MemberBlacklistStatus.None;

    }
}
