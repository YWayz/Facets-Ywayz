using Facets.Core.Passes.DTOs;
using FluentValidation;

namespace Facets.Core.Passes.Validators;

public sealed class UpdatePassCategoryPavilionRateDtoValidator : AbstractValidator<UpdatePassCategoryPavilionRateDto>
{
    public UpdatePassCategoryPavilionRateDtoValidator()
    {
        RuleForEach(fe => fe.PavilionRates)
            .ChildRules(c =>
        {
            c.RuleFor(p => p.PavilionRate)
                 .NotNull()
                 .WithMessage("Pavilion rate is required")
                 .GreaterThanOrEqualTo(0)
                 .WithMessage("The rate should be greater than or to 0");

            c.RuleFor(r => r.PassCategoryId)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Pass category id cannot be null")
                 .NotEmpty().WithMessage("Pass category id cannot be an empty list")
                 .Must(passCategoryId => Guid.Empty != passCategoryId)
                 .WithMessage("Pass category id cannot contain empty guid");

            c.RuleFor(r => r.PassCategoryPavilionSettingsId)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("Pass category pavilion settings id cannot be null")
                 .NotEmpty().WithMessage("Pass category pavilion settings id cannot be an empty list")
                 .Must(passSettingsId => Guid.Empty != passSettingsId)
                 .WithMessage("Pass category pavilion settings id cannot contain empty guid");
        });
    }
}
