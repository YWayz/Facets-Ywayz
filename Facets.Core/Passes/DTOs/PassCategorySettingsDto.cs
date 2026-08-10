using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record PassCategorySettingsDto(Guid Id, 
                                             Guid PassCategoryId, 
                                             bool IsChargeable, 
                                             decimal Rate, 
                                             decimal DiscountedRate, 
                                             bool ApplyEarlyRegistrationDiscountedRate, 
                                             bool ApplyOnlineRegistrationDiscountedRate, 
                                             bool ApplyEntireEventDiscountedRate, 
                                             DateTimeOffset? EarlyRegistrationDiscountedRateValidUntil, 
                                             RateType RateType);