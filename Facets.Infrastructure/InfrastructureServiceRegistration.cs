using Azure.Identity;
using Facets.Core.Common.Interfaces;
using Facets.Infrastructure.FileStorage;
using Facets.Infrastructure.NotificationServices;
using Facets.Infrastructure.OnePay;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.Infrastructure.OnePay.Services;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Reflection;

namespace Facets.Infrastructure;

public static class InfrastructureServiceRegistration
{
    private static readonly MediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue = new MediaTypeWithQualityHeaderValue("application/json");
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAzureClients(builder =>
        {
            // Add a KeyVault client
            //builder.AddSecretClient(Configuration.GetSection("KeyVault"));

            // Add a storage account client
            string? azstoragrCon = configuration.GetConnectionString("AzureStorage");

            builder.AddBlobServiceClient(azstoragrCon);
            builder.AddQueueServiceClient(azstoragrCon);

            // Use DefaultAzureCredential by default
            builder.UseCredential(new DefaultAzureCredential());

            // Set up any default settings
            //builder.ConfigureDefaults(Configuration.GetSection("AzureDefaults"));
        });

        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileRespository, FileRespository>();
        services.AddSingleton<IFileUrlSigner, BlobFileUrlSigner>();
        services.AddScoped<ISMSService, SMSService>();
        services.TryAddScoped<IOnePayService, OnePayService>();

        var OnePaySettingsConfig = configuration.GetSection(nameof(OnePaySettings));

        services.Configure<OnePaySettings>(OnePaySettingsConfig);

        OnePaySettings onepaySettings = new();

        OnePaySettingsConfig.Bind(onepaySettings);

        services.AddHttpClient<IOnePayAPIService, OnePayAPIService>((serviceProvider, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(onepaySettings?.BaseURL ?? string.Empty);
            httpClient.DefaultRequestHeaders.Accept.Add(mediaTypeWithQualityHeaderValue);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, onepaySettings?.AppToken ?? string.Empty);
        });

        return services;
    }
}
