using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.Passes.Entities;

public sealed class PassType : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    internal static Guid VisitorPassTypeId = Guid.Parse("8916d851-6dd4-4592-99c0-4a45e46fb269");
    internal static Guid TeamMemberPassTypeId = Guid.Parse("53605f0f-4309-445d-a1f6-962567aa5af2");

    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; } = null!;

    public bool IsDefault { get; private set; }

    private PassType() { }


    public static IReadOnlyList<PassType> SeedDefaultData()
    {
        IReadOnlyList<PassType> defaultPassTypes = new[]
        {
           new PassType()
           {
                Id = VisitorPassTypeId,
                Name = "Visitor",
                IsDefault = true
           },

             new PassType ()
             {
                Id = TeamMemberPassTypeId,
                Name = "Team Member",
                IsDefault = true
             }
        };

        return defaultPassTypes;
    }
}
