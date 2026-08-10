using Facets.Core.Events.Interfaces;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Interfaces;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Validators;

public sealed class UpdateVisitorOnlineEventRegistrationDtoValidator : AbstractValidator<UpdateVisitorOnlineEventRegistrationDto>
{
    public UpdateVisitorOnlineEventRegistrationDtoValidator(IEventService eventService, ILoggedInUserService loggedInUser, IVisitorRegistrationService visitorRegistrationService)
    {
        RuleFor(r => r.EventDateIDs)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Event Date IDs cannot be null")
            .Must(eventDateIds => eventDateIds.All(eventDateId => Guid.Empty != eventDateId))
            .WithMessage("Event Date IDs cannot contain empty guids")
            .Must((eventDateIds) =>
            {
                var anyDuplicate = eventDateIds.GroupBy(x => x).Any(g => g.Count() > 1);

                return !anyDuplicate;
            }).WithMessage("Event Date IDs cannot be duplicated");

        string eventErrorMsg = string.Empty;

        RuleFor(r => r)
            .MustAsync(async (model, cancellation) =>
            {
                var eventResponse = await eventService.GetEventById(loggedInUser.FacetsEventId, cancellation);

                var registeredVisitorResponse = await visitorRegistrationService.GetRegistrationById(model.id, cancellation);

                var currentDate = DateTimeOffset.UtcNow.Date;
                DateTimeOffset? eventDate = null;

                if (eventResponse.Success is false)

                {
                    eventErrorMsg = eventResponse.Errors.First().Value.First();
                    return false;
                }

                else if (eventResponse.Data!.Status is EventStatus.Inactive)
                {
                    eventErrorMsg = "Event is not active";
                    return false;
                }

                else if (model.EventDateIDs.All(a => eventResponse.Data.EventDates.Select(s => s.Key).Contains(a)) is false)
                {
                    eventErrorMsg = $"One or more event date ID(s) are invalid";
                    return false;
                }

                else if (registeredVisitorResponse!.Data!.RateType == RateType.PerDayRate && eventResponse.Data.EventDates.Where(w => model.EventDateIDs.Contains(w.Key))
                                                      .Select(s => s.Value.Date)
                                                      .Any(a => { eventDate = a; return a < currentDate; }))
                {
                    eventErrorMsg = $"Event date: {eventDate.ToApplicationDateFormat()} is a passed date";
                    return false;
                }

                else if (currentDate > eventResponse.Data.EventDates.Select(s => s.Value.Date).OrderByDescending(s => s).First())
                {
                    eventErrorMsg = $"Unable to register for the event, as it has already concluded";
                    return false;
                }


                return true;

            }).WithName("Event")
            .WithMessage(_ => eventErrorMsg);

        RuleFor(r => r.PavilionSessionIDs)
        .Cascade(CascadeMode.Stop)
        .NotNull().WithMessage("Pavilion Session IDs cannot be null")
        .Must(pavilionSessionIds => pavilionSessionIds.All(pavilionSessionId => Guid.Empty != pavilionSessionId))
        .WithMessage("Pavilion Session IDs cannot contain empty guids")
        .Must((pavilionSessionIds) =>
        {
            var anyDuplicate = pavilionSessionIds.GroupBy(x => x).Any(g => g.Count() > 1);

            return !anyDuplicate;
        }).WithMessage("Pavilion Session IDs cannot be duplicated");
    }
}
