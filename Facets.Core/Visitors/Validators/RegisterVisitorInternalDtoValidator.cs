using Facets.Core.Common.Validators;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Validators;

public sealed class RegisterVisitorInternalDtoValidator : AbstractValidator<RegisterVisitorInternalDto>
{
    public RegisterVisitorInternalDtoValidator(IVisitorRepository visitorRepository)
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
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("NIC number is required")
                .MaximumLength(AppConstants.StringLengths.IdentityNumber)
                .WithMessage($"NIC number must be less than {AppConstants.StringLengths.IdentityNumber} characters");

            });

        When(r => r.VisitorIdentityType is VisitorIdentityType.Passport,
            () =>
            {
                RuleFor(r => r.PassportNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Passport number is required")
                .MaximumLength(AppConstants.StringLengths.IdentityNumber)
                .WithMessage($"Passport number must be less than {AppConstants.StringLengths.IdentityNumber} characters");

            });

        RuleFor(_ => _)
           .Cascade(CascadeMode.Stop)
           .MustAsync(async (model, cancellation) =>
           {
               string identificationNumber = (model.NICNumber ?? model.PassportNumber)!;

               bool exists = await visitorRepository.VisitorRegistered(identificationNumber, cancellationToken: cancellation);

               return !exists;
           })
           .WithName("Identification Number")
           .WithMessage("Visitor is already registered");

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
            .Cascade(CascadeMode.Stop)
            .NotEmpty().When(w => w.RegisterOnline is true).WithMessage("Email is required")
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
