using FluentValidation;
using Facets.Core.Security.Dtos;

namespace Facets.Core.Security.Validators;

public sealed class UpdateRoleClaimsDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleClaimsDtoValidator()
    {
        RuleFor(r => r.Permissions)
                .AppPermissionValueValidation();
    }
}
