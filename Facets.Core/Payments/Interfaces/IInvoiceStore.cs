using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Interfaces;

internal interface IInvoiceStore
{
    internal Task<ResponseResult<Invoice>> CreateInvoice(CreateInvoiceDto model, CancellationToken cancellationToken);
}

