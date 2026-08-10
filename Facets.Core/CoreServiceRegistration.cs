using Facets.Core.Common.Interfaces;
using Facets.Core.Common.Services;
using Facets.Core.Common.Validators;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Counters.Services;
using Facets.Core.Events.Interfaces;
using Facets.Core.Events.Services;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Services;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Passes.Services;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Payments.Interfaces.LineItemHadlers;
using Facets.Core.Payments.Services;
using Facets.Core.Payments.Services.LineItemHandlers;
using Facets.Core.Reports.Interfaces;
using Facets.Core.Reports.Services;
using Facets.Core.Security.Interfaces;
using Facets.Core.Security.Services;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.TeamMembers.Services;
using Facets.Core.Visitors.Interfaces;
using Facets.Core.Visitors.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Facets.Core;

public static class CoreServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.TryAddScoped<IModelValidator, ModelValidator>();

        services.TryAddScoped<ISecurityService, SecurityService>();
        services.TryAddScoped<ITokenBuilder, TokenBuilder>();
        services.TryAddScoped<IPermissionService, PermissionService>();
        services.TryAddScoped<IUserRoleService, UserRoleService>();
        services.TryAddScoped<IUserRolePermissionFacadeService, UserRolePermissionFacadeService>();

        services.TryAddScoped<IEventService, EventService>();
        services.TryAddScoped<IRegistrationCounterService, RegistrationCounterService>();
        services.TryAddScoped<ILookupService, LookupService>();
        services.TryAddScoped<IPassCategoryService, PassCategoryService>();
        services.TryAddScoped<IVisitorStore, VisitorStore>();
        services.TryAddScoped<ITeamMemberService, TeamMemberService>();
        services.TryAddScoped<IVisitorService, VisitorService>();
        services.TryAddScoped<IVisitorRegistrationStore, VisitorRegistrationStore>();
        services.TryAddScoped<IVisitorRegistrationService, VisitorRegistrationService>();
        services.TryAddScoped<IInvoiceStore, InvoiceStore>();
        services.TryAddScoped<IInvoiceService, InvoiceService>();
        services.TryAddScoped<ITeamMemberEventService, TeamMemberEventService>();
        services.TryAddScoped<IPaymentService, PaymentService>();
        services.TryAddScoped<ITeamMemberActivityService, TeamMemberActivityService>();
        services.TryAddScoped<IPassTemplateService, PassTemplateService>();
        services.TryAddScoped<IReportService, ReportService>();
        services.TryAddScoped<IOTPService, OTPService>();
        services.TryAddScoped<IOTPStore, OTPStore>();
        services.TryAddScoped<IVisitorActivityService, VisitorActivityService>();
        services.TryAddScoped<IEventVisitorService, EventVisitorService>();
        services.TryAddScoped<IPavilionService, PavilionService>();
        services.TryAddScoped<IVisitorPavilionService, VisitorPavilionService>();
       
        services.TryAddScoped<IEventDateLineItemHandler, EventDateLineItemHandler>();
        services.TryAddScoped<IPavilionSessionLineItemHandler, PavilionSessionLineItemHandler>();
        services.TryAddScoped<IEventSettingsService, EventSettingsService>();

        return services;
    }
}
