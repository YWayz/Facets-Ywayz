namespace Facets.Core.Passes.DTOs;

public sealed record PassCategoryPavilionSettingsDto(Guid Id,
                                                    decimal PavilionRate,
                                                    Guid PassCategoryId,
                                                    Guid PavilionId,
                                                    string PassCategoryName,
                                                    string PassType
                                                    );