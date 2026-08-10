using Facets.Core.Events.DTOs;
using Facets.SharedKernal;
using FluentValidation;

namespace Facets.Core.Events.Validators;

internal sealed class UpdatePavilionDtoValidator : AbstractValidator<UpdatePavilionDto>
{
    public UpdatePavilionDtoValidator()
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Pavilion name is required")
            .MaximumLength(AppConstants.StringLengths.FirstName)
            .WithMessage($"Pavilion name must be less than {AppConstants.StringLengths.FirstName} characters");
    }
}
