using Facets.Core.Events.DTOs;
using Facets.Core.Passes.DTOs;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Specs;

public sealed record PavilionSummaryDto(Guid Id,
                                        string Name,
                                        int NumberOfSession,
                                        PavilionStatus Status,
                                        IReadOnlyCollection<PavilionSessionSummaryDto> PavilionSessionSummaries);
