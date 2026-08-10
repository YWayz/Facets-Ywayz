using Facets.Core.Passes.DTOs;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using FluentValidation;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Validators;

public sealed class UpdatePassTemplateDtoValidator : AbstractValidator<UpdatePassTemplateDto>
{
    public UpdatePassTemplateDtoValidator(ILoggedInUserService loggedInUser)
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
                    .WithMessage("height is required");

        RuleFor(r => r.Width)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("width is required");

        RuleFor(r => r.SizeType)
                      .Cascade(CascadeMode.Stop)
                      .NotEmpty()
                      .WithMessage("Size is required")
                      .IsInEnum()
                      .WithMessage("Invalid Size")
                      .NotEqual(TemplateSizeType.None)
                      .WithMessage("Size type cannot be None");

        RuleFor(r => r.PassType)
                     .Cascade(CascadeMode.Stop)
                     .IsInEnum()
                     .WithMessage("Invalid Pass type")
                     .NotEqual(AppEnums.PassType.None)
                     .WithMessage("Pass type cannot be None")
                     .NotEmpty()
                     .WithMessage("Pass type is required");
    }
}
