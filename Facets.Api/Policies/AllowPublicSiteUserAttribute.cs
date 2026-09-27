namespace Facets.Api.Policies;

/// <summary>
/// Marks an admin-API endpoint that the public visitor site is allowed to call with a visitor
/// (OTP) token, such as reading event details or pass categories. Every other admin endpoint
/// rejects visitor tokens; see <see cref="DefaultAuthorizationPolicy"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public sealed class AllowPublicSiteUserAttribute : Attribute { }
