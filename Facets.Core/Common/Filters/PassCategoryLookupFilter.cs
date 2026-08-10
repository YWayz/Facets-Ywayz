using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Common.Filters;

public sealed record PassCategoryLookupFilter(bool? IsDefault, IEnumerable<PassCategoryType>? PassCategoryType);
