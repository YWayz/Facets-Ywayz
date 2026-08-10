using Ardalis.Specification;
using Facets.Core.Security.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Specs;

internal sealed class UserLoginSpec : Specification<ApplicationUser>
{
    public UserLoginSpec()
    {
        Query.Include(i => i.UserProfile)
                .ThenInclude(i => i.UserEvents.Where(w => w.Event.Status == EventStatus.Active))
                    .ThenInclude(e => e.Event)
             .AsSplitQuery();
    }
}
