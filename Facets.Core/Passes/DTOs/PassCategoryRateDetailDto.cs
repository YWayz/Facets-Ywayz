using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.DTOs;

public sealed record PassCategoryRateDetailDto(Guid Id,
                                               Guid passCategoryId,
                                               string PassCategoryName,
                                               bool IsChargeable,
                                               decimal Rate,
                                               decimal DiscountedRate,
                                               bool ApplyEarlyRegistrationDiscountedRate,
                                               bool ApplyOnlineRegistrationDiscountedRate,
                                               bool ApplyEntireEventDiscountedRate,
                                               DateTimeOffset? EarlyRegistrationDiscountedRateValidUntil,
                                               Guid EventId,
                                               DateTimeOffset VisitorRegistrationStartsOn,
                                               DateTimeOffset VisitorRegistrationEndsOn,
                                               RateType RateType);