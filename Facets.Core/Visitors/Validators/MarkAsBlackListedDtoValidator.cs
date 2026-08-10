using Facets.Core.Visitors.DTOs;
using Facets.SharedKernal;
using FluentValidation;

namespace Facets.Core.Visitors.Validators;

public sealed class MarkAsBlackListedDtoValidator : AbstractValidator<MarkAsBlackListedDto>
{
    public MarkAsBlackListedDtoValidator()
    {
        RuleFor(r => r.Reason)
            .NotEmpty()
            .WithMessage("Provide a reason to Blacklist")
            .MaximumLength(AppConstants.StringLengths.Description);


        When(r => r.BlackListUntil.HasValue,
            () =>
            {
                RuleFor(r => r.BlackListUntil)
                .GreaterThan(DateTimeOffset.UtcNow);
            });
    }
}
