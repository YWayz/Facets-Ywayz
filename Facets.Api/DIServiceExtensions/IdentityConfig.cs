using Facets.Api.Policies;
using Facets.Api.PolicyRequriements.EventAccessRequirments;
using Facets.Api.PolicyRequriements.PublicSiteUserAccessRequirements;
using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security;
using Facets.Core.Security.Entities;
using Facets.Persistence;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace Facets.Api.DIServiceExtensions;

public static class IdentityConfig
{
    private const string applicationJSONContentType = "application/json";

    public static void AddIdentityConfig(this IServiceCollection services, WebApplicationBuilder builder)
    {

        services.AddIdentity<ApplicationUser, Role>(options =>
        {
            options.Password.RequiredLength = 14;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

        services.Configure<PasswordHasherOptions>(option =>
        {
            option.IterationCount = 512_000;
        });

        JwtConfig jwtData = new();

        builder.Configuration.Bind(nameof(JwtConfig), jwtData);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
          .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtData.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtData.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtData.SigningKey)),
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };

                //This code came from https://www.blinkingcaret.com/2018/05/30/refresh-tokens-in-asp-net-core-web-api/
                //It returns a useful header if the JWT Token has expired

                options.Events = new JwtBearerEvents
                {
                    // A staff JWT lives for hours; without this a deleted or locked-out user kept working until it
                    // expired. Visitor tokens are checked by PublicSiteUserAccessRequirementHandler instead.
                    OnTokenValidated = async context =>
                    {
                        var principal = context.Principal;
                        if (principal is null) return;

                        string? subject = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                                          ?? principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                        if (subject == Facets.SharedKernal.AppConstants.PublicSite.PublicSiteUserId || Guid.TryParse(subject, out Guid userId) is false) return;

                        var users = context.HttpContext.RequestServices.GetRequiredService<Facets.Core.Security.Interfaces.IUserSecurityRespository>();
                        var user = await users.GetUser(userId, context.HttpContext.RequestAborted);

                        if (user is null || user.IsDeleted || (user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow))
                        {
                            context.Fail("The account is no longer active.");
                        }
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.ContentType = applicationJSONContentType;
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;

                        await context.Response.WriteAsync(Serializer.Serialize(new ErrorResponse()
                        {
                            Errors = new List<KeyValuePair<string, IEnumerable<string>>>
                                {
                                new (nameof(HttpStatusCode.Forbidden), new[] { "Access denied" })
                                }
                        }));
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.ContentType = applicationJSONContentType;
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        var isPublicUser = context.Request
                                                  .Path.StartsWithSegments($"/api/{APIConstants.APIGroup.Public}", StringComparison.OrdinalIgnoreCase);

                        var msg = isPublicUser ? "Your session has expired" : "Your login has expired, please login again";

                        await context.Response.WriteAsync(Serializer.Serialize(new ErrorResponse()
                        {
                            Errors = new List<KeyValuePair<string, IEnumerable<string>>>
                            {
                                new (nameof(HttpStatusCode.Unauthorized), new[] { msg })
                            }
                        }));
                    }
                };
            });

        services.AddTransient<IAuthorizationHandler, EventAccessRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, UserClaimRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, PublicSiteUserAccessRequirementHandler>();

        services.AddAuthorization(options =>
        {
            var authPolicyApplicators = typeof(Program).Assembly
                                                       .ExportedTypes
                                                       .Where(x => typeof(IAuthPolicyApplyer)
                                                       .IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                                       .Select(Activator.CreateInstance)
                                                       .Cast<IAuthPolicyApplyer>()
                                                       .ToList();

            foreach (var authPolicyApplicator in authPolicyApplicators)
            {
                authPolicyApplicator.Apply(options);
            }

            // Visitor (public-site) tokens may only reach endpoints that explicitly allow them.
            options.DefaultPolicy = DefaultAuthorizationPolicy.Build();
        });
    }
}
