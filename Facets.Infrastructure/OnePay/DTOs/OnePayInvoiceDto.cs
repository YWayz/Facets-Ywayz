namespace Facets.Infrastructure.OnePay.DTOs;

internal sealed record OnePayInvoiceDto(Guid InvoiceId,
                                        decimal TotalAmount,
                                        string ReferenceNumber,
                                        string FirstName,
                                        string LastName,
                                        string MobileNumber,
                                        string Email);
