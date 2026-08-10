using Facets.Core.Security.Dtos;
using FluentValidation;

namespace Facets.Core.Security.Validators;

public sealed class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
{
    public ForgotPasswordModelValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email/Username is required");
    }
}
