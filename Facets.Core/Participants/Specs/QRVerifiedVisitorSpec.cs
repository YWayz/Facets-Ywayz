using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Specs;

internal sealed class QRVerifiedVisitorSpec : Specification<VisitorRegistration, QRVerifiedVisitorDto>
{
    public QRVerifiedVisitorSpec(Guid eventId, Guid registrationId)
    {
        Query.Where(w => w.EventId == eventId && w.Id == registrationId);

        Query.Select(s => new QRVerifiedVisitorDto(s.Visitor.FirstName,
                                                   s.Visitor.LastName,
                                                   s.Visitor.NICNumber! ?? s.Visitor.PassportNumber!,
                                                   s.PassCategory.Name,
                                                   s.Visitor.Documents
                                                            .Where(w => w.IsDeleted == false)
                                                            .OrderByDescending(w => w.CreatedOn)
                                                            .FirstOrDefault(w => w.AttachmentType == AttachmentType.ProfileImage)!.AttachmentURL))
            ;
    }
}
