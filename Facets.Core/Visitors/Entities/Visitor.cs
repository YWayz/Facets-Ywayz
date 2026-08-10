using Facets.Core.Common.Entities;
using Facets.Core.Common.ValueObjects;
using Facets.Core.Visitors.Events;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Entities;

public sealed class Visitor : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public bool IsAssocifyMember { get; private set; } = false;

    public VisitorIdentityType VisitorIdentityType { get; private set; }

    public string? NICNumber { get; private set; }
    public string? PassportNumber { get; private set; }

    public Guid CountryId { get; private set; }
    public Country Country { get; private set; } = null!;

    public bool RegisteredOnline { get; private set; }
    public bool OTPVerified { get; private set; }
    public bool OTPVerificationRequired { get; private set; }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string MobileNumber { get; private set; } = null!;
    public string? CompanyName { get; private set; } = null!;
    public string? Email { get; private set; } = null!;
    public string? VisitorReference { get; private set; } = null!;

    public AddressValueObject? Address { get; private set; }

    private readonly List<VisitorDocument> _documents = new();

    public IReadOnlyCollection<VisitorDocument> Documents => _documents.AsReadOnly();

    private readonly List<VisitorBlackListHistory> _visitorBlackListHistories = new();

    public IReadOnlyCollection<VisitorBlackListHistory> VisitorBlackListHistories => _visitorBlackListHistories.AsReadOnly();

    public VisitorStatus VisitorStatus { get; private set; }

    public DateTimeOffset? BlackListedUntil { get; private set; }

    private Visitor() { }

    public Visitor(VisitorIdentityType visitorIdentityType,
                   string? nicNumber,
                   string? passportNumber,
                   Guid countryId,
                   string firstName,
                   string lastName,
                   string mobileNumber,
                   string? email,
                   string? companyName,
                   AddressValueObject? address,
                   bool isAssocifyMember,
                   bool registeredOnline)
    {
        SetIdentityType(visitorIdentityType, nicNumber, passportNumber); 
        SetRegistrationMode(registeredOnline);
        SetReferenceNumber();
        IsAssocifyMember = isAssocifyMember;
        VisitorIdentityType = visitorIdentityType;
        CountryId = countryId;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        CompanyName = companyName;
        Email = email;
        Address = address;
        VisitorStatus = VisitorStatus.Active;
        OTPVerified = false;

        RegisterDomainEvent(new VisitorCreatingEvent(this));

        void SetIdentityType(VisitorIdentityType visitorIdentityType, string? nicNumber, string? passportNumber)
        {
            if (visitorIdentityType is VisitorIdentityType.NIC)
            {
                PassportNumber = null;
                NICNumber = nicNumber;
            }

            else if (visitorIdentityType is VisitorIdentityType.Passport)
            {
                NICNumber = null;
                PassportNumber = passportNumber;
            }
        }

        void SetRegistrationMode(bool registeredOnline)
        {
            RegisteredOnline = registeredOnline;
            OTPVerificationRequired = RegisteredOnline is true ? true : false;
        }
    }

    internal void AddDocument(string attachmentURL, AttachmentType attachmentType, string uniqueName, string displayName)
    {
        _documents.Add(new VisitorDocument(attachmentURL, attachmentType, uniqueName, displayName));
    }

    internal void UpdateInfo(VisitorIdentityType visitorIdentityType,
                             string? nicNumber,
                             string? passportNumber,
                             Guid countryId,
                             string firstName,
                             string lastName,
                             string mobileNumber,
                             string? email,
                             string? companyName,
                             AddressValueObject? address)
    {
        if (visitorIdentityType is VisitorIdentityType.NIC)
        {
            PassportNumber = null;
            NICNumber = nicNumber;
        }

        else if (visitorIdentityType is VisitorIdentityType.Passport)
        {
            NICNumber = null;
            PassportNumber = passportNumber;
        }
        SetReferenceNumber();
        VisitorIdentityType = visitorIdentityType;
        CountryId = countryId;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = mobileNumber;
        CompanyName = companyName;
        Email = email;
        Address = address;
    }

    internal void DeleteDocument(Guid documentId)
    {
        _documents.FirstOrDefault(f => f.Id == documentId)?.Delete();
    }

    internal void MarkAsBlacklisted(DateTimeOffset? blacklistUntil, string reason)
    {
        VisitorStatus = VisitorStatus.BlackListed;
        BlackListedUntil = blacklistUntil;

        _visitorBlackListHistories.Add(new(VisitorStatus, reason, BlackListedUntil));
    }

    internal void RemoveFromBlacklist()
    {
        VisitorStatus = VisitorStatus.Active;
        BlackListedUntil = null;
        _visitorBlackListHistories.Add(new(VisitorStatus, "N/A", BlackListedUntil));
    }

    internal ResponseResult UpdateOTPVerificationStatus()
    {
        if (VisitorStatus is VisitorStatus.BlackListed)
            return new(new OperationFailedException("Visitor OTP verfication", "Visitor has been blacklisted"));

        else if (OTPVerificationRequired is false)
            return new(new OperationFailedException("Visitor OTP verfication", "OTP verification is not required"));

        else if (OTPVerified is true)
            return new(new OperationFailedException("Visitor OTP verfication", "OTP is already verified"));

        OTPVerified = true;

        return new();
    }

    internal void SetReferenceNumber()
    {
        // Get the current date and time in the desired format
        string dateTimeString = DateTime.Now.ToString("yyMMddHHmmss");

        // Generate a random 6-digit number
        Random random = new Random();
        int randomNumber = random.Next(100000, 1000000);

        // Combine the date, time, and random number into the reference number format
        string referenceNumber = $"{dateTimeString}-{randomNumber.ToString("D6")}";

        VisitorReference= referenceNumber;
    }
}
