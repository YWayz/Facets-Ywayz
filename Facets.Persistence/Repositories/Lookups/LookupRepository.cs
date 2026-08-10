using Facets.Core.Common.Entities;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;
using Facets.Core.Lookups.Interfaces;
using Facets.Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Lookups;

public sealed class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _dbContext;

    public LookupRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Country>> GetCountries(CancellationToken token)
    {
        var countries = await _dbContext.Set<Country>().OrderBy(o => o.Name).ToListAsync(cancellationToken: token);

        return countries;
    }

    public async Task<IReadOnlyList<KeyValuePair<Guid, string>>> GetAssignedUsersToEvent(Guid eventId, CancellationToken token)
    {
        var users = await _dbContext.Set<UserAssignedEvent>()
                                    .Where(w => w.EventId == eventId &&
                                                w.UserProfile.ApplicationUser.IsDeleted == false)
                                    .Select(s => new KeyValuePair<Guid, string>(s.UserProfileId, s.UserProfile.FullName))
                                    .ToListAsync(cancellationToken: token);

        return users.AsReadOnly();
    }

    public async Task<IReadOnlyList<KeyValuePair<Guid, string>>> GetPavilions(Guid eventId, CancellationToken token)
    {
        var pavilions = await _dbContext.Set<Pavilion>()
                                    .Where(w => w.EventId == eventId && w.Status == PavilionStatus.Active && w.IsDeleted == false)
                                    .OrderByDescending(o => o.CreatedOn)
                                    .Select(s => new KeyValuePair<Guid, string>(s.Id, s.Name))
                                    .ToListAsync(cancellationToken: token);

        return pavilions.AsReadOnly();
    }

    public async Task<IReadOnlyList<PavilionSessionSummaryDto>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token)
    {
        var pavilionSessions = await _dbContext.Set<PavilionSession>()
                                    .Where(w => w.Pavilion.EventId == eventId && w.PavilionId == pavilionId && w.IsDeleted == false)
                                    .Select(s => new PavilionSessionSummaryDto(s.Id,
                                                                               s.EventDateId,
                                                                               s.StartTime,
                                                                               s.EndTime,
                                                                               s.AllowedVisitorCount,
                                                                               s.PavilionId,
                                                                               s.EventDate.Date,
                                                                               s.VisitorPavilionSessionAttendanceSchedules.Any()))
                                    .ToListAsync(cancellationToken: token);

        return pavilionSessions;
    }
}
