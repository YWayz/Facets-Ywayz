using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Filters;

public sealed record PavilionFilter(PavilionStatus? PavilionStatus);