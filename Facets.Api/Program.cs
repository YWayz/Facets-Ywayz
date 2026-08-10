using Facets.Api.DIServiceExtensions;
using Facets.Api.Middleware;
using Facets.Api.Services;
using Facets.Core;
using Facets.Core.Common.Interfaces;
using Facets.Core.Security;
using Facets.Infrastructure;
using Facets.Infrastructure.NotificationServices;
using Facets.Persistence;
using Facets.SharedKernal.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddSerilogConfig();

    builder.Host.UseSerilog();

    var services = builder.Services;

    services.AddApplicationInsightsTelemetry();

    services.AddControllerConfig();

    services.AddSwaggerConfig();

    services.AddCorsConfig();

    services.AddApplicationServices();
    services.AddInfrastructureServices(builder.Configuration);
    services.AddPersistenceServices(builder.Configuration);

    services.AddHttpContextAccessor();
    services.AddScoped<ILoggedInUserService, LoggedInUserService>();
    services.AddScoped<IApplicationContext, ApplicationContext>();
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

app.UseCustomExceptionHandler();

// TODO: fix watch dog multiple auth issue
//app.UseWatchDogExceptionLogger();

if (app.Environment.IsDevelopment()) { }

else
{
    app.UseHsts();
}

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

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, max-age=21600, must-revalidate, private";
    }
});

app.UseCors();

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
