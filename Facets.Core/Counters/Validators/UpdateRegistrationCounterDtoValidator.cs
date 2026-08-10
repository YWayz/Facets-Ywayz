using Facets.Core.Common.Validators;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.Validators;

public sealed class UpdateRegistrationCounterDtoValidator : AbstractValidator<UpdateRegistrationCounterDto>
{
    public UpdateRegistrationCounterDtoValidator(IRegistrationCounterRepository registrationCounterRepository, ILoggedInUserService loggedInUser)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(AppConstants.StringLengths.FirstName)
            .WithMessage($"Name must be less than {AppConstants.StringLengths.FirstName} characters");

        RuleFor(r => r.Description)
            .MaximumLength(AppConstants.StringLengths.Description)
            .WithMessage($"Description must be less than {AppConstants.StringLengths.Description} characters");

        RuleFor(r => r.CounterType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Invalid Counter Type")
            .IsInEnum()
            .WithMessage("Invalid Counter Type")
            .NotEqual(CounterType.None)
            .WithMessage("Counter Type cannot be None");
    }
}
