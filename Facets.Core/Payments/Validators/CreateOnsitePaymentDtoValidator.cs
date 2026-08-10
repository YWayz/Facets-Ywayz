using Facets.Core.Payments.DTOs;
using Facets.SharedKernal;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Validators;

public sealed class CreateOnsitePaymentDtoValidator : AbstractValidator<CreateOnsitePaymentDto>
{
    public CreateOnsitePaymentDtoValidator()
    {
        RuleFor(r => r.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required");

        RuleFor(r => r.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Amountm must be zero or greater");

        RuleFor(p => p.PaymentMethod)
            .IsInEnum()
            .NotEqual(PaymentMethod.None)
            .WithMessage("Invalid PaymentMethod");

        When(r => r.PaymentMethod is PaymentMethod.Card,
            () =>
            {
                RuleFor(r => r.LastFourDigitsofCard)
                .Length(4)
                    .When(s => string.IsNullOrWhiteSpace(s.LastFourDigitsofCard) is false)
                .WithMessage("Length of last four digitd must be 4"); ;

                RuleFor(r => r.ReferenceNumber)
                .MaximumLength(AppConstants.StringLengths.IdentityNumber)
                .WithMessage($"Card payment refernce must be less than {AppConstants.StringLengths.IdentityNumber} characters");
            });

        RuleFor(r => r.VisitorId)
            .NotEmpty()
            .WithMessage("Visitor ID is required");
    }
}
