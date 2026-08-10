using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Validators;

public sealed class CreatePassCategoryDtoValidator : AbstractValidator<CreatePassCategoryDto>
{
    public CreatePassCategoryDtoValidator(IPassCategoryRepository passCategoryRepository, ILoggedInUserService loggedInUser)
    {
        RuleFor(r => r.Name)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Pass category name is required")
                    .MaximumLength(AppConstants.StringLengths.Description)
                    .WithMessage($"Pass category name must be less than {AppConstants.StringLengths.Description} characters");

        RuleFor(r => r.Description)
                    .MaximumLength(AppConstants.StringLengths.Description)
                    .WithMessage($"Pass category description must be less than {AppConstants.StringLengths.Description} characters");

        RuleFor(r => r.PassCategoryType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Invalid Pass category type")
            .IsInEnum()
            .WithMessage("Invalid Pass category type")
            .NotEqual(PassCategoryType.None)
            .WithMessage("Pass Category Type cannot be None")
            .Must((model, passCategoryType) =>
            {
                return passCategoryType is PassCategoryType.TeamMember or PassCategoryType.Custom_Visitor;
            })
             .WithMessage("Pass Category Type must be  Custom_Visitor or TeamMember");
    }
}