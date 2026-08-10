namespace Facets.Core.Common.Entities;

public sealed class Country
{
    public static readonly Guid SriLanka = Guid.Parse("480eda47-363a-4d97-a90d-5992392442e9");

    public Guid Id { get; set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
}
