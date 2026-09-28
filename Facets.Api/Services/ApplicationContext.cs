using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Interfaces;

namespace Facets.Api.Services;

public sealed class ApplicationContext : IApplicationContext
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string? _configuredPublicUrl;

    public ApplicationContext(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _configuredPublicUrl = configuration["PublicBaseUrl"];
    }

    /// <summary>
    /// Base URL used in links we email (password reset). Taken from the PublicBaseUrl setting when present,
    /// so a forged Host header can never steer a reset link to an attacker's site; the request host is the fallback.
    /// </summary>
    public string BaseUrl
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_configuredPublicUrl) is false) return _configuredPublicUrl.TrimEnd('/') + "/";

            return _httpContextAccessor.HttpContext?.Request.BaseUrl()!;
        }
    }

    public string FEUrl
    {
        get
        {
            var baseUrl = _webHostEnvironment.IsDevelopment() ? "http://localhost:4200/" : _httpContextAccessor.HttpContext?.Request.BaseUrl();

            return baseUrl!;
        }
    }
}
