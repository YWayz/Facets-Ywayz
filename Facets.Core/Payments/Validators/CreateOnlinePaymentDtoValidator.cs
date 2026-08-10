using Facets.Core.Payments.DTOs;
using Facets.SharedKernal;
using FluentValidation;

namespace Facets.Core.Payments.Validators;

public sealed class CreateOnlinePaymentDtoValidator : AbstractValidator<CreateOnlinePaymentDto>
{
    public CreateOnlinePaymentDtoValidator()
    {
        RuleFor(r => r.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required");

        RuleFor(r => r.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Amountm must be zero or greater");

        RuleFor(r => r.VisitorId)
          .NotEmpty()
          .WithMessage("Visitor ID is required");

        RuleFor(r => r.LastFourDigitsOfCard)
               .Length(4)
                   .When(s => string.IsNullOrWhiteSpace(s.LastFourDigitsOfCard) is false)
               .WithMessage("Length of last four digitd must be 4");

        RuleFor(r => r.ReferenceNumber)
        .MaximumLength(AppConstants.StringLengths.IdentityNumber)
        .WithMessage($"Card payment refernce must be less than {AppConstants.StringLengths.IdentityNumber} characters");
    }
}
