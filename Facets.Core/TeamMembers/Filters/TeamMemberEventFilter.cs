using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Filters;

public sealed record TeamMemberEventFilter(Guid EventId, string? SearchQuery, IEnumerable<Guid>? PassCategoryIds, TeamMemberStatus? TeamMemberStatus);
