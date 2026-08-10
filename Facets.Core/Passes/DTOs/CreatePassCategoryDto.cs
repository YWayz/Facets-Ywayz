using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record CreatePassCategoryDto(string Name,
                                           string? Description,
                                           PassCategoryType PassCategoryType, 
                                           string Color);
