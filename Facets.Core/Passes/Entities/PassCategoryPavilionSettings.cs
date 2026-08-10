using Facets.Core.Events.Entities;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Passes.Entities;

public sealed class PassCategoryPavilionSettings : EntityBase, ICreatedAudit, IUpdatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    
    public decimal PavilionRate { get; private set; }

    public Guid PassCategoryId { get; private set; }
    public PassCategory PassCategory { get; private set; } = null!;

    public Guid PavilionId { get; private set; }
    public Pavilion Pavilion { get; private set; } = null!;

    private PassCategoryPavilionSettings() { }

    public PassCategoryPavilionSettings(Guid passCategoryId, Guid pavilionId, decimal pavilionRate)
    {
        PassCategoryId = passCategoryId;
        PavilionId = pavilionId;
        PavilionRate = pavilionRate;
    }

    internal ResponseResult UpdatePavilionRate(decimal pavilionRate)
    {
        PavilionRate = pavilionRate;

        return new ResponseResult();
    }
}
