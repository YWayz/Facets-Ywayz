using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

internal sealed record InvoicePaymentInfo(Guid PassCategorySettingId,
                                          Guid PassCategoryId,
                                          bool IsChargeable,
                                          decimal Rate,
                                          decimal DiscountedRate,
                                          bool ApplyEarlyRegistrationDiscountedRate,
                                          bool ApplyOnlineRegistrationDiscountedRate,
                                          bool ApplyEntireEventDiscountedRate,
                                          DateTimeOffset? EarlyRegistrationDiscountedRateValidUntil,
                                          RateType RateType);
