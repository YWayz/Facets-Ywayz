namespace Facets.Core.Passes.DTOs;

public sealed record UpdatePassCategoryPavilionRateItemDto(Guid PassCategoryPavilionSettingsId,
                                                           decimal PavilionRate,
                                                           Guid PassCategoryId);