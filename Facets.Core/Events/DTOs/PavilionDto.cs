using Facets.Core.Passes.DTOs;

namespace Facets.Core.Events.DTOs;

public sealed record PavilionDto(Guid Id,
                                 string Name,
                                 IReadOnlyCollection<PassCategoryPavilionSettingsDto> PassCategoryPavilionSettings = null!);
