using Facets.Api.DIServiceExtensions;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Filters;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Facets.Infrastructure.OnePay.Interfaces;

namespace Facets.Api.Controllers.V1.Payments;

[Route("api/invoices")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class InvoicesController : AdminAppControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IOnePayPaymentRecorder _paymentRecorder;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService invoiceService, IOnePayPaymentRecorder paymentRecorder, ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _paymentRecorder = paymentRecorder;
        _logger = logger;
    }

    [HttpGet("{id}", Name = nameof(GetInvoiceById))]
    [ProducesResponseType(typeof(ResponseResult<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetInvoiceById([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _invoiceService.GetInvoiceById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("~/api/registrations/{registrationId}/invoices")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<InvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitorRegistrationRegistration([FromQuery] Paginator paginator, [FromQuery] RegistrationInvoiceFilter filter, [FromRoute] Guid registrationId, CancellationToken token)
    {
        var response = await _invoiceService.GetInvoicesByRegistration(paginator, filter, registrationId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("~/api/visitors/{visitorId}/invoice")]
    [ProducesResponseType(typeof(ResponseResult<InvoicePaymentDto?>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetInvoicePayment([FromRoute] Guid visitorId, [FromQuery] InvoiceFilter filter, CancellationToken token)
    {
        var response = await _invoiceService.InvoicePaymentByRegistration(visitorId, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{invoiceId}/payment-status")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingConfig.AnonymousPublicPolicy)]
    [ProducesResponseType(typeof(ResponseResult<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckPaymentStatus([FromRoute] Guid invoiceId, CancellationToken token)
    {
        var response = await _invoiceService.CheckPaymentStatus(invoiceId, token);

        // The visitor often lands here before OnePay's notification arrives. If the invoice is still unpaid,
        // ask OnePay directly and record the payment now instead of showing "failed" and inviting a second charge.
        if (response.Success && response.Data is false)
        {
            try
            {
                var result = await _paymentRecorder.RecordIfPaid(invoiceId, transactionId: null, token);

                if (result is PaymentRecordResult.Recorded or PaymentRecordResult.AlreadyPaid)
                    response = await _invoiceService.CheckPaymentStatus(invoiceId, token);
            }
            catch (Exception ex)
            {
                // OnePay unreachable: report what the database says; the reconciliation timer will retry.
                _logger.LogWarning(ex, "Payment status check could not reach OnePay for invoice {InvoiceId}", invoiceId);
            }
        }

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
