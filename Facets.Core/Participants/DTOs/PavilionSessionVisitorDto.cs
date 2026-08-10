using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.DTOs;

public sealed record PavilionSessionVisitorDto(string VisitorFullName, IReadOnlyList<VisitorPavilionSessionsDto> VisitorPavilionSessions);
