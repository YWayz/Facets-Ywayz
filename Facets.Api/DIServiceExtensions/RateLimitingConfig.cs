using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Facets.Api.DIServiceExtensions;

/// <summary>
/// Per-client-IP rate limits for the anonymous public-site endpoints, so they cannot be used to
/// flood OTP sending, enumerate identity numbers or spam registrations and uploads.
///
/// These are flood limits, not the main OTP defence: the OTP itself locks after
/// AppConstants.OTP.MaxFailedAttempts wrong codes and only MaxSendsPerWindow can be sent per identity.
/// Limits are generous because many visitors at an exhibition share one venue Wi-Fi IP address.
/// </summary>
public static class RateLimitingConfig
{
    /// <summary>OTP send and verify: 60 requests per 5 minutes per client IP.</summary>
    public const string OtpPolicy = "otp";

    /// <summary>Other anonymous public endpoints: 300 requests per minute per client IP.</summary>
    public const string AnonymousPublicPolicy = "anonymous-public";

    public static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"errors\":[{\"key\":\"TooManyRequests\",\"value\":[\"Too many requests. Please wait a moment and try again.\"]}]}",
                    cancellationToken);
            };

            options.AddPolicy(OtpPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(5),
                    QueueLimit = 0,
                }));

            options.AddPolicy(AnonymousPublicPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 300,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }));
        });

        return services;
    }

    // On Azure App Service the platform forwards the real client IP (Program.cs applies forwarded headers).
    private static string ClientKey(HttpContext httpContext) =>
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
