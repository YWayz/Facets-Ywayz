using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

internal sealed record ProcessedInvoiceData(Guid PassCategorySettingId,
                                            Guid PassCategoryId,
                                            bool IsChargeable,
                                            DiscountType DiscountType,
                                            decimal AmountPerLineItem);
