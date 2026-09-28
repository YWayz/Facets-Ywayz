using Facets.Core.Security.Entities;
using Facets.Persistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Facets.FunctionApp.CP.ArchiveAppAuditLogs;

/// <summary>
/// Data minimisation. OTP codes (with the phone number or email they went to) and issued visitor token
/// records used to be kept forever. Codes are useless after 5 minutes and tokens after a day, so both are
/// deleted nightly once they are comfortably past any use.
/// </summary>
public sealed class PurgeSecurityRowsTimerTrigger
{
    private static readonly TimeSpan OtpRetention = TimeSpan.FromDays(7);
    private static readonly TimeSpan IssuedTokenRetention = TimeSpan.FromDays(2);

    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;

    public PurgeSecurityRowsTimerTrigger(AppDbContext dbContext, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = loggerFactory.CreateLogger<PurgeSecurityRowsTimerTrigger>();
    }

    [Function(nameof(PurgeSecurityRowsTimerTrigger))]
    public async Task Run([TimerTrigger("0 30 2 * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        int otps = await _dbContext.Set<OTPQueue>()
                                   .Where(o => o.CreatedOn < now - OtpRetention)
                                   .ExecuteDeleteAsync(cancellationToken);

        int tokens = await _dbContext.Set<PublicSiteUserIssuedToken>()
                                     .Where(t => t.CreatedOn < now - IssuedTokenRetention)
                                     .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("Purged {Otps} expired OTP rows and {Tokens} expired visitor token rows", otps, tokens);
    }
}
