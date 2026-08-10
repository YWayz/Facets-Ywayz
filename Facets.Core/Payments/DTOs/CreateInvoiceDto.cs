namespace Facets.Core.Payments.DTOs;

public sealed record CreateInvoiceDto(Guid VisitorRegistrationId,
                                      Guid VisitorId,
                                      Guid EventId,
                                      Guid? RegistrationCounterId,
                                      bool IsOnlinePayment,
                                      bool InvoicedOnSite);
