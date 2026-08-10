using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record InvoicePaymentDto(decimal InvoiceAmount, PaymentMethod PaymentMethod, RateType RateType, PaymentStatus PaymentStatus);