using Facets.Core.Events.DTOs;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Validators;

public sealed class UpdatePaymentSettingsDtoValidator : AbstractValidator<UpdatePaymentSettingsDto>
{
    public UpdatePaymentSettingsDtoValidator()
    {
        RuleFor(r => r.OnSitePayingMode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Invalid on site paying mode status")
            .IsInEnum()
            .WithMessage("Invalid on site paying mode status")
            .NotEqual(OnSitePayingMode.None)
            .WithMessage("On site paying mode cannot be None");
    }
}
