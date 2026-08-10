using Ardalis.Specification;
using Facets.Core.Common.Filters;
using Facets.Core.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Facets.Core.Common.Specs;

    internal class EventByUserLookupSpec : Specification<UserAssignedEvent, KeyValuePair<Guid, string>>
{
    public EventByUserLookupSpec(string userId)
    {
        Query.Where(p => p.UserProfileId == new Guid(userId) && p.Event.Status == SharedKernal.AppEnums.EventStatus.Active);

        Query.Select(e => new KeyValuePair<Guid, string>
        (
         e.Event.Id,
         e.Event.Name
        ));
    }
}

