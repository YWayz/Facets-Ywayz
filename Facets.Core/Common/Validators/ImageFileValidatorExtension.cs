using Facets.SharedKernal;
using Facets.SharedKernal.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Facets.Core.Common.Validators;

internal static class ImageFileValidatorExtension
{
    internal static IRuleBuilderOptions<T, IFormFile?> ValidateImageFile<T>(this IRuleBuilder<T, IFormFile?> rule)
    {
        return rule.ChildRules(file =>
                    {
                        file.RuleFor(f => f!.FileName).NotEmpty().WithMessage("Image name is required");
                        file.RuleFor(c => c!.FileName).Must((file, fileName) =>
                        {
                            var fileExtension = FileHelper.GetFileExtension(fileName);
                            var isValidExtension = AppConstants.FileExtension.ValidImageFileExtensions.Any(ext => ext == fileExtension.ToLower());
                            return isValidExtension;
                        }).WithMessage("Unsupported image type");

                        // Extension alone is easy to fake; also check the declared content type and cap the size.
                        file.RuleFor(c => c!.ContentType)
                            .Must(ct => ct is not null && AppConstants.FileExtension.ValidImageContentTypes.Contains(ct.ToLower()))
                            .WithMessage("Unsupported image type");

                        file.RuleFor(c => c!.Length)
                            .GreaterThan(0).WithMessage("Image is empty")
                            .LessThanOrEqualTo(AppConstants.FileExtension.MaxImageFileSizeBytes)
                            .WithMessage($"Image must be {AppConstants.FileExtension.MaxImageFileSizeBytes / (1024 * 1024)} MB or smaller");
                    });
    }
}
