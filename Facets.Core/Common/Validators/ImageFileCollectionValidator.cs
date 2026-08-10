using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Facets.Core.Common.Validators;

public sealed class ImageFileCollectionValidator : AbstractValidator<IEnumerable<IFormFile>>
{
    public ImageFileCollectionValidator()
    {
        RuleForEach(r => r)
            .Cascade(CascadeMode.Stop)
            .ValidateImageFile();
    }
}
