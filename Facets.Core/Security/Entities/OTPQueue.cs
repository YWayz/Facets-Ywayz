using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Entities;

public sealed class OTPQueue : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }


    public string IdentityNumber { get; private set; } = null!;

    public string SentTo { get; private set; } = null!;

    public OTPType Type { get; private set; }

    public string Code { get; private set; } = null!;

    public DateTimeOffset ValidUntil { get; private set; }

    public bool Verified { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }

    public int FailedAttempts { get; private set; }

    public bool IsLocked => FailedAttempts >= AppConstants.OTP.MaxFailedAttempts;

    private OTPQueue() { }

    public OTPQueue(string code, OTPType type, string identityNumber, string sendTo)
    {
        Code = code;
        Type = type;
        IdentityNumber = identityNumber;

        ValidUntil = DateTimeOffset.UtcNow.AddMinutes(AppConstants.OTP.ValidMinutes);
        SentTo = sendTo;
    }

    internal void RegisterFailedAttempt()
    {
        FailedAttempts++;
    }

    internal void MarkAsVerified()
    {
        Verified = true;
        VerifiedAt = DateTimeOffset.UtcNow;
    }
}
