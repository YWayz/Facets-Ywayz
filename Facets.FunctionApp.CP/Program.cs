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

host.Run();