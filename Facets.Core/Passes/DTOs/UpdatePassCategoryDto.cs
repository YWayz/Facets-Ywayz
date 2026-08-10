namespace Facets.Core.Passes.DTOs;

public sealed record UpdatePassCategoryDto(string Name,
                                           string? Description, 
                                           string Color);
