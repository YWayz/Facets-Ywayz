using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

namespace Facets.Api.DIServiceExtensions;

internal static class SwaggerConfig
{
    public static void AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddFluentValidationRulesToSwagger(configureRegistration: cfg =>
        {
            // Enable this if using Newtonsoft json for serialization 
            //cfg.RegisterJsonSerializerOptions = false;
        });

        services.AddSwaggerGen(c =>
        {
            c.OrderActionsBy(cfg => $"{cfg.ActionDescriptor.RouteValues["controller"]}_{cfg.RelativePath}");
            c.EnableAnnotations();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @$"JWT Authorization header using the Bearer scheme.
                                <br/>                               
                                Enter your token in the text input below.
                                <br/> 
                                Example: 'ezdsda12345abcdef'",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",

            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {

                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer" }
                    }, new List<string>()
                }
            });

            c.SwaggerDoc(APIConstants.APIGroup.Admin, new OpenApiInfo
            {
                Version = "v1",
                Title = "Facets Admin API",
            });

            c.SwaggerDoc(APIConstants.APIGroup.Public, new OpenApiInfo
            {
                Version = "v1",
                Title = "Facets Public site API",
            });

            // Enable to add swagger documentation
            //var apiCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var apiCommentsFullPath = Path.Combine(AppContext.BaseDirectory, apiCommentsFile);
            //c.IncludeXmlComments(apiCommentsFullPath);
        });

        // Enable the below if using newtonsoft
        // services.AddSwaggerGenNewtonsoftSupport();
    }
}