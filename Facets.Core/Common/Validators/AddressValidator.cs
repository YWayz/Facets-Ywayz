using Facets.Core.Common.ValueObjects;
using Facets.SharedKernal;
using FluentValidation;

namespace Facets.Core.Common.Validators;

public sealed class AddressValidator : AbstractValidator<AddressValueObject?>
{
    public AddressValidator()
    {
        RuleFor(r => r.Address)
            .MaximumLength(AppConstants.StringLengths.Address).WithMessage($"Address must be less than {AppConstants.StringLengths.Address} characters");
    }
}
