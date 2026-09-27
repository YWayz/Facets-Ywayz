using Facets.Api.DIServiceExtensions;
using Facets.Api.Middleware;
using Facets.Api.Services;
using Facets.Core;
using Facets.Core.Common.Interfaces;
using Facets.Core.Security;
using Facets.Infrastructure;
using Facets.Infrastructure.FileStorage;
using Facets.Infrastructure.NotificationServices;
using Facets.Persistence;
using Facets.SharedKernal.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddSerilogConfig();

    builder.Host.UseSerilog();

    // Fail fast with a clear list of missing settings instead of failing on every request later.
    builder.ValidateRequiredConfiguration();

    var services = builder.Services;

    services.AddApplicationInsightsTelemetry();

    services.AddControllerConfig();

    services.AddSwaggerConfig();

    services.AddCorsConfig(builder.Configuration, builder.Environment);

    services.AddRateLimitingConfig();

    // Azure App Service terminates TLS in front of the app; trust its X-Forwarded-* headers so
    // the real client IP (used by rate limiting) and scheme are seen. ForwardLimit stays at its
    // default of 1, so only the right-most address (the one App Service's front end adds) is used
    // and a client cannot spoof its IP by sending its own X-Forwarded-For.
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.AddApplicationServices();
    services.AddInfrastructureServices(builder.Configuration);
    services.AddPersistenceServices(builder.Configuration);

    services.AddHttpContextAccessor();
    services.AddScoped<ILoggedInUserService, LoggedInUserService>();
    services.AddScoped<IApplicationContext, ApplicationContext>();
    services.AddScoped<IPublicSiteOwnership, PublicSiteOwnership>();

    // Every file URL leaving the API becomes a short-lived signed link (blob containers are private).
    services.AddOptions<Microsoft.AspNetCore.Mvc.JsonOptions>()
            .Configure<IFileUrlSigner>((options, signer) => options.JsonSerializerOptions.Converters.Add(new SignedFileUrlJsonConverter(signer)));
    services.TryAddScoped<IQueueService, QueueService>();

    services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));

    services.AddIdentityConfig(builder);

    services.AddMemoryCache();

    // TODO: fix watch dog multiple auth issue
    //services.AddWatchDogConfig(builder);

    services.AddOutputCacheConfig();
    services.AddMemoryCache();
}

var app = builder.Build();

app.UseForwardedHeaders();

app.UseCustomExceptionHandler();

// TODO: fix watch dog multiple auth issue
//app.UseWatchDogExceptionLogger();

if (app.Environment.IsDevelopment()) { }

else
{
    app.UseHsts();
}

// Swagger lists every endpoint and model. Only in Development, or where Swagger__Enabled=true is set on purpose.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/Admin/swagger.json", "Admin APIs");
        c.SwaggerEndpoint("/swagger/Public/swagger.json", "Public APIs");

        c.RoutePrefix = app.Environment.IsDevelopment() ? string.Empty : c.RoutePrefix;
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

        if (app.Environment.IsDevelopment())
        {
            c.EnablePersistAuthorization();
        }
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, max-age=21600, must-revalidate, private";
    }
});

app.UseCors();

app.UseRateLimiter();

app.UseOutputCache();

app.MapControllers().RequireAuthorization();

// TODO: output cache does not work if watchdog is used
//app.UseWatchDog(opt =>
//{
//    opt.WatchPageUsername = builder.Configuration["WatchDog:Username"];
//    opt.WatchPagePassword = builder.Configuration["WatchDog:Password"];
//});

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, max-age=21600, must-revalidate, private";
    }
});


app.Run();
