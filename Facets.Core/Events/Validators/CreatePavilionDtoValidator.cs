using Facets.Core.Events.DTOs;
using Facets.Core.Events.Interfaces;
using Facets.SharedKernal;
using FluentValidation;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Core.Events.Validators;

public sealed class CreatePavilionDtoValidator : AbstractValidator<CreatePavilionDto>
{
    public CreatePavilionDtoValidator(IPavilionRepository pavilionRepository)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(StringLengths.FirstName)
            .WithMessage($"Name must be less than {AppConstants.StringLengths.FirstName} characters");
    }
}
