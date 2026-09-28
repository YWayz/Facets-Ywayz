using Facets.Infrastructure.OnePay.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Facets.FunctionApp.CP.OnePayFunctions;

/// <summary>
/// Safety net for lost webhooks. Every 15 minutes, each unpaid invoice that had a OnePay link created in
/// the last 7 days is checked against OnePay's status API and recorded if it was paid. Without this, a
/// webhook lost to a cold start, a key rotation or a OnePay outage meant a paid visitor with no pass.
/// </summary>
public sealed class ReconcilePaymentsTimerTrigger
{
    private static readonly TimeSpan LookBack = TimeSpan.FromDays(7);

    private readonly IOnePayPaymentRecorder _recorder;
    private readonly ILogger _logger;

    public ReconcilePaymentsTimerTrigger(IOnePayPaymentRecorder recorder, ILoggerFactory loggerFactory)
    {
        _recorder = recorder;
        _logger = loggerFactory.CreateLogger<ReconcilePaymentsTimerTrigger>();
    }

    [Function(nameof(ReconcilePaymentsTimerTrigger))]
    public async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment reconciliation started");

        await _recorder.ReconcileUnpaidInvoices(DateTimeOffset.UtcNow - LookBack, cancellationToken);
    }
}
