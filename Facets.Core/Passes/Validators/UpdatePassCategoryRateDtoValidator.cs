using Facets.Core.Passes.DTOs;
using FluentValidation;

namespace Facets.Core.Passes.Validators;

public sealed class UpdatePassCategoryRateDtoValidator : AbstractValidator<UpdatePassCategoryRateDto>
{
    public UpdatePassCategoryRateDtoValidator()
    {
        When(r => r.IsChargeable is true,
            () =>
            {
                RuleFor(p => p.Rate)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("The rate should be greater than or equal to 0");

                RuleFor(p => p.DiscountedRate)
                    .LessThanOrEqualTo(c => c.Rate)
                    .WithMessage("The discount rate cannot be equal to or greater than the rate");

                RuleFor(p => p.RateType)
                    .IsInEnum()
                    .NotEqual(SharedKernal.AppEnums.RateType.None)
                    .WithMessage("Invalid Rate Type");
            });

        When(r => r.IsChargeable is false,
            () =>
            {
                RuleFor(p => p.Rate)
                    .Equal(0)
                    .WithMessage("The rate should be 0");

                RuleFor(p => p.DiscountedRate)
                    .Equal(0)
                    .WithMessage("The discount rate should be 0");
            });

        When(r => r.ApplyEarlyRegistrationDiscountedRate || r.ApplyOnlineRegistrationDiscountedRate || r.ApplyEntireEventDiscountedRate,
            () =>
            {
                RuleFor(p => p.DiscountedRate)
                    .LessThanOrEqualTo(c => c.Rate)
                    .WithMessage("The discount rate cannot be equal to or greater than the rate");
            });

        When(r => r.ApplyEarlyRegistrationDiscountedRate is true,
            () =>
            {
                RuleFor(p => p.EarlyRegistrationDiscountedRateValidUntil)
                   .NotEmpty()
                   .WithMessage("Early registration discount date is required");
            });
    }
}
