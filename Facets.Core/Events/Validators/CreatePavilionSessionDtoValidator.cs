using Facets.Core.Events.DTOs;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using FluentValidation;

namespace Facets.Core.Events.Validators;

internal sealed class CreatePavilionSessionDtoValidator : AbstractValidator<CreateOrUpdatePavilionSessionItemDto>
{
    public CreatePavilionSessionDtoValidator()
    {
        RuleFor(r => r.EventDateId)
                       .Cascade(CascadeMode.Stop)
                       .NotNull().WithMessage("Event Date IDs cannot be null")
                       .NotEmpty().WithMessage("Event Date IDs cannot be an empty list")
                       .Must(eventDateId => Guid.Empty != eventDateId)
                       .WithMessage("Event Date IDs cannot contain empty guids");

        RuleFor(r => r.AllowedVisitorCount)
             .Cascade(CascadeMode.Stop)
             .GreaterThan(0)
             .WithMessage("Allowed visitor count should be greater than 0");

        RuleFor(r => r.StartTime)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage("Start time is required");

        RuleFor(r => r.EndTime)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage("Visitor registration end date is required");

        RuleFor(r => r)
            .Must(model =>
            {

                return model.StartTime.TimeOfDay != model.EndTime.TimeOfDay;
            })
            .WithMessage("Start time and End time cannot be the same");

        RuleFor(r => r)
            .Must(model =>
            {
                return model.StartTime.GetLocalTime(AppConstants.SriLankaTimeZone).Date == model.EndTime.GetLocalTime(AppConstants.SriLankaTimeZone).Date;
            })
            .WithMessage("End time should be passed Start time");
    }
}