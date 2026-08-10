using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Passes.DTOs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Interfaces;

public interface IEventVisitorService
{
    Task<ResponseResult<IReadOnlyList<Attendee>>> GetEventVisitors(Guid eventDateId, Paginator paginator, AttendeesFilter filter, CancellationToken token);
    Task<ResponseResult<VisitorPassVerificationDto>> GetVisitorVerificationDetails(Guid eventId, Guid visitorId, Guid eventDateId, CancellationToken token);
    Task<ResponseResult<VisitorPassTemplateDto>> GetVisitorPassGenerationTemplate(Guid eventId, Guid visitorId, Guid eventDateId, CancellationToken token);
    Task<ResponseResult<QRVerifiedVisitorDto>> VerifyPass(VisitorPassVerificationDto model, CancellationToken token);
    Task<ResponseResult<QRVerifiedVisitorDto>> VerifyPavilionPass(VisitorPavilionPassVerificationDto model, CancellationToken token);
}
