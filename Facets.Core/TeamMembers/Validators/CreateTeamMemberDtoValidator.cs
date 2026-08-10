using Facets.Core.Common.Validators;
using Facets.Core.TeamMembers.DTOs;
using Facets.SharedKernal;
using FluentValidation;

namespace Facets.Core.TeamMembers.Validators;

public sealed class CreateTeamMemberDtoValidator : AbstractValidator<CreateTeamMemberDto>
{
    public CreateTeamMemberDtoValidator()
    {   
        RuleFor(r => r.CountryId)
            .NotEmpty()
            .WithMessage("Country is required");

        RuleFor(r => r.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(AppConstants.StringLengths.FirstName)
            .WithMessage($"First name must be less than {AppConstants.StringLengths.FirstName}");

        RuleFor(r => r.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(AppConstants.StringLengths.LastName)
            .WithMessage($"Last name must be less than {AppConstants.StringLengths.LastName}");

        RuleFor(r => r.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required")
            .MaximumLength(AppConstants.StringLengths.PhoneNumber);

        RuleFor(r => r.Email)
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MaximumLength(AppConstants.StringLengths.Email)
            .WithMessage($"Email address must be less than {AppConstants.StringLengths.Email}");

        RuleFor(s => s.Address)
           .SetValidator(new AddressValidator());

        RuleFor(s => s.CompanyName)
            .MaximumLength(AppConstants.StringLengths.Description);
    }
}
