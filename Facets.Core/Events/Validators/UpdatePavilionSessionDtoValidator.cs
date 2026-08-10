using Facets.Core.Events.DTOs;
using FluentValidation;

namespace Facets.Core.Events.Validators;

internal sealed class UpdatePavilionSessionDtoValidator : AbstractValidator<CreateOrUpdatePavilionSessionItemDto>
{
    public UpdatePavilionSessionDtoValidator()
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
    }
}
