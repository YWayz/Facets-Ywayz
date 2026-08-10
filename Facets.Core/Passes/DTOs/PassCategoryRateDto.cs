namespace Facets.Core.Passes.DTOs;

public sealed record PassCategoryRateDto(Guid Id,
                                          Guid passCategoryId,
                                          string PassCategoryName, 
                                          bool IsChargeable, 
                                          decimal Rate, 
                                          decimal DiscountedRate, 
                                          Guid EventId,
                                          DateTimeOffset VisitorRegistrationStartsOn);