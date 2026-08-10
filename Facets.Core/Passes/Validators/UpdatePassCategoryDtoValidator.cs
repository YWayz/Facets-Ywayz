using Facets.Core.Common.Validators;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using FluentValidation;

namespace Facets.Core.Passes.Validators;

public sealed class UpdatePassCategoryDtoValidator : AbstractValidator<UpdatePassCategoryDto>
{
    public UpdatePassCategoryDtoValidator(IPassCategoryRepository passCategoryRepository, ILoggedInUserService loggedInUser)
    {
        RuleFor(r => r.Name)
          .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Pass category name is required")
                .MaximumLength(AppConstants.StringLengths.FirstName)
                .WithMessage($"Pass category name must be less than {AppConstants.StringLengths.FirstName} characters");

        RuleFor(r => r.Description)
              .MaximumLength(AppConstants.StringLengths.Description)
              .WithMessage($"Pass category description must be less than {AppConstants.StringLengths.Description} characters");

    }
}
