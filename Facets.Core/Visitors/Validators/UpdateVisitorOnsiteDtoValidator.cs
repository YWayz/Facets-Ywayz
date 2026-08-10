using Facets.Core.Common.Validators;
using Facets.Core.Visitors.DTOs;
using Facets.SharedKernal;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Validators;

public sealed class UpdateVisitorOnsiteDtoValidator : AbstractValidator<UpdateVisitorDto>
{
    public UpdateVisitorOnsiteDtoValidator()
    {
        RuleFor(r => r.VisitorIdentityType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Invalid identity type")
            .IsInEnum()
            .WithMessage("Invalid identity type")
            .NotEqual(VisitorIdentityType.None)
            .WithMessage("Identity type cannot be None");

        When(r => r.VisitorIdentityType is VisitorIdentityType.NIC,
            () =>
            {
                RuleFor(r => r.NICNumber)
                .NotEmpty()
                .WithMessage("NIC number is required")
                .MaximumLength(AppConstants.StringLengths.IdentityNumber)
                .WithMessage($"NIC number must be less than {AppConstants.StringLengths.IdentityNumber} characters");

            });

        When(r => r.VisitorIdentityType is VisitorIdentityType.Passport,
            () =>
            {
                RuleFor(r => r.PassportNumber)
                .NotEmpty()
                .WithMessage("Passport number is required")
                .MaximumLength(AppConstants.StringLengths.IdentityNumber)
                .WithMessage($"Passport number must be less than {AppConstants.StringLengths.IdentityNumber} characters");

            });

        RuleFor(r => r.CountryId)
            .NotEmpty()
            .WithMessage("Country is required");

        RuleFor(r => r.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(r => r.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");

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
