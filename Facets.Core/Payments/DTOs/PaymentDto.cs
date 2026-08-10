using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.DTOs;

public sealed record PaymentDto(PaymentMethod PaymentMethod, 
                                bool IsOnlinePayment, 
                                Guid InvoiceId, 
                                decimal TotalAmount, 
                                PaymentStatus InvoicePaymentStatus);