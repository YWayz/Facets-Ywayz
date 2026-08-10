using Facets.Core.Passes.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record PassCategoryDto(Guid Id,
                                     Guid EventId,
                                     string Name,
                                     string? Description,
                                     VisitorPassCategoryType VisitorPassCategoryType,
                                     string Color,
                                     bool isDefault = false,
                                     string passTypeName = null,
                                     IReadOnlyCollection<PassCategorySettingsDto> PassCategorySettings = null,
                                     IReadOnlyCollection<PassCategoryPavilionSettingsDto> PassCategoryPavilionSettings = null);
