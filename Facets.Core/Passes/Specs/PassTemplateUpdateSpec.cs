using Ardalis.Specification;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Passes.Specs;

internal class PassTemplateUpdateSpec : Specification<PassTemplate>
{
    public PassTemplateUpdateSpec(Guid eventId, Guid passTemplateId, SharedKernal.AppEnums.PassType passType)
    {
        Query.Where(e => e.Id == passTemplateId && e.EventId == eventId && e.PassType == passType);
    }
}