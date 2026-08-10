using Facets.Core.Common.Dtos;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Interfaces;

public interface IEventService
{
    Task<ResponseResult<FileDto>> AddEventLogo(Guid eventId, IFormFile file, CancellationToken token);
    Task<ResponseResult<EventDto>> CreateEvent(CreateEventDto model, CancellationToken cancellationToken);
    Task<ResponseResult> Delete(Guid id, CancellationToken token);
    Task<ResponseResult> DeleteEventLogo(Guid eventId, CancellationToken token);
    Task<ResponseResult<EventDetailDto>> GetEventById(Guid id, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<EventSummaryDto>>> GetEvents(Paginator paginator, EventFilter filter, CancellationToken token);
    Task<bool> IsEventActive(Guid facetsEventId, CancellationToken token);
    Task<ResponseResult> UpdateEvent(Guid eventId, UpdateEventDto model, CancellationToken token);
    Task<ResponseResult> UpdateEventStatus(Guid eventId, UpdateEventStatusDto model, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<PublicSiteEventSummaryDto>>> GetEventsForPublicSite(Paginator paginator, CancellationToken token);
    internal Task<ResponseResult<EventDateDetailDto>> GetEventDateByDate(Guid facetsEventId, DateTimeOffset currentDate, CancellationToken token);
    Task<ResponseResult<bool>> IsEventMarkedAsPayLaterForOnlineRegistration(Guid id, CancellationToken token);
    Task<ResponseResult<OnSitePayingMode>> GetEventOnSitePaymentModeSpec(Guid id, CancellationToken token);
}
