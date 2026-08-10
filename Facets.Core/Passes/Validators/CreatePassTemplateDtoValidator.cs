using Facets.Core.Passes.DTOs;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Validators;

public sealed class CreatePassTemplateDtoValidator : AbstractValidator<CreatePassTemplateDto>
{
    public CreatePassTemplateDtoValidator(ILoggedInUserService loggedInUser)
    {
        RuleFor(r => r.TemplateText)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Pass template is required");

        RuleFor(r => r.PreviewTemplateText)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Pass template preview is required");

        RuleFor(r => r.Height)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Height is required");

        RuleFor(r => r.Width)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("Width is required");

        RuleFor(r => r.PassType)
                      .Cascade(CascadeMode.Stop)
                      .IsInEnum()
                      .WithMessage("Invalid Pass type")
                      .NotEqual(AppEnums.PassType.None)
                      .WithMessage("Pass type cannot be None")
                      .NotEmpty()
                      .WithMessage("Pass type is required");

        RuleFor(r => r.SizeType)
                      .Cascade(CascadeMode.Stop)
                      .NotEmpty()
                      .WithMessage("Size is required")
                      .IsInEnum()
                      .WithMessage("Invalid Size")
                      .NotEqual(TemplateSizeType.None)
                      .WithMessage("Size type cannot be None");
    }
}
