using Azure.Identity;
using Facets.Core;
using Facets.Core.Common.Interfaces;
using Facets.FunctionApp.CP;
using Facets.FunctionApp.CP.EFCore.AuditSetup;
using Facets.Infrastructure;
using Facets.Infrastructure.NotificationServices;
using Facets.Persistence;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
     .ConfigureAppConfiguration(c =>
     {
         c.AddEnvironmentVariables();
     })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((appBuilder, services) =>
    {
        var configuration = appBuilder.Configuration;

        services.TryAddScoped<ITableStorageService, TableStorageService>();


        services.AddAzureClients(builder =>
        {
            var azureWebJobsStorage = configuration.GetValue<string>("AzureWebJobsStorage");

            builder.AddTableServiceClient(azureWebJobsStorage);

            builder.UseCredential(new DefaultAzureCredential());
        });

        services.AddHttpClient();

        services.TryAddScoped<ILoggedInUserService, AzureFunctionLoggedInService>();
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddApplicationServices();
        services.AddPersistenceServices(configuration);
        services.AddInfrastructureServices(configuration);

        services.TryAddScoped<IQueueService, QueueService>();

    }).Build();

// The webhook and reconciliation need the OnePay settings; the notification functions need SMTP.
// Missing values used to surface only as failures deep inside a function. Warn once at startup instead.
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    var startupLogger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    string[] requiredKeys = { "OnePaySettings:BaseURL", "OnePaySettings:AppID", "OnePaySettings:AppToken", "OnePaySettings:HashSalt", "ConnectionStrings:MSSQLDbConnection", "AzureWebJobsStorage", "Smtp_From", "Smtp_Password", "ArchiveAppAuditLogCron" };

    var missing = requiredKeys.Where(k => string.IsNullOrWhiteSpace(configuration[k])).ToList();

    if (missing.Count > 0)
        startupLogger.LogError("Function App settings missing: {Settings}. See DEPLOYMENT.md", string.Join(", ", missing.Select(k => k.Replace(":", "__"))));
}

host.Run();