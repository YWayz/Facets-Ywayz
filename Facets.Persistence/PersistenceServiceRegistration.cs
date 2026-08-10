using EntityFramework.Exceptions.SqlServer;
using Facets.Core.Common.Interfaces;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Events.Interfaces;
using Facets.Core.Lookups.Interfaces;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Reports.Interfaces;
using Facets.Core.Security.Interfaces;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.Visitors.Interfaces;
using Facets.Persistence.Repositories;
using Facets.Persistence.Repositories.Assocify;
using Facets.Persistence.Repositories.Counters;
using Facets.Persistence.Repositories.Events;
using Facets.Persistence.Repositories.Lookups;
using Facets.Persistence.Repositories.Participants;
using Facets.Persistence.Repositories.Passes;
using Facets.Persistence.Repositories.Payments;
using Facets.Persistence.Repositories.Reports;
using Facets.Persistence.Repositories.Security;
using Facets.Persistence.Repositories.TeamMemberActivities;
using Facets.Persistence.Repositories.TeamMemberEvents;
using Facets.Persistence.Repositories.TeamMembers;
using Facets.Persistence.Repositories.Visitors;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Facets.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString(AppConstants.Database.APIDbConnectionName))
                  .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                  .UseExceptionProcessor());

        services.TryAddScoped<IUserSecurityRespository, UserSecurityRespository>();
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();

        services.TryAddScoped<IEventRepository, EventRepository>();
        services.TryAddScoped<IPassCategoryRepository, PassCategoryRepository>();
        services.TryAddScoped<IRegistrationCounterRepository, RegistrationCounterRepository>();
        services.TryAddScoped<ITeamMemberRepository, TeamMemberRepository>();

        services.AddHttpClient<IAssocifyMemberRepository, AssocifyMemberRepository>((serviceProvider, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(configuration["Assocify:BaseURL"] ?? string.Empty);
        });

        services.TryAddScoped<IVisitorRepository, VisitorRepository>();
        services.TryAddScoped<ILookupRepository, LookupRepository>();
        services.TryAddScoped<IVisitorRegistrationRepository, VisitorRegistrationRepository>();
        services.TryAddScoped<IInvoiceRepository, InvoiceRepository>();
        services.TryAddScoped<ITeamMemberEventRepository, TeamMemberEventRepository>();
        services.TryAddScoped<IPaymentRepository, PaymentRepository>();
        services.TryAddScoped<ITeamMemberActivityRepository, TeamMemberActivityRepository>();
        services.TryAddScoped<IPassTemplateRepository, PassTemplateRepository>();
        services.TryAddScoped<IVisitorReportRepository, VisitorReportRepository>();
        services.TryAddScoped<IOTPRepository, OTPRepository>();
        services.TryAddScoped<IVisitorActivityRepository, VisitorActivityRepository>();
        services.TryAddScoped<IEventVisitorRepository, EventVisitorRepository>();
        services.TryAddScoped<IPavilionRepository, PavilionRepository>();
        services.TryAddScoped<IVisitorPavilionSessionRepository, VisitorPavilionSessionRepository>();

        services.TryAddScoped<IOnePayRepository, OnePayRepository>();

        return services;
    }
}
