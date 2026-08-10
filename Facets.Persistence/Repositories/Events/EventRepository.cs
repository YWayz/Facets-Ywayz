using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Events.Entities;
using Facets.Core.Events.Interfaces;
using Facets.Core.Participants.Entities;
using Facets.Core.Security.Entities;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Events;

internal sealed class EventRepository : BaseRepository, IEventRepository
{
    private readonly DbSet<Event> _table;
    private readonly DbSet<UserAssignedEvent> _userAssignedtable;

    public EventRepository(AppDbContext dbContext) : base(dbContext)
    {

        _table = _dbContext.Set<Event>();
        _userAssignedtable = _dbContext.Set<UserAssignedEvent>();
    }


    public Event AddEvent(Event @event)
    {
        _table.Add(@event);

        return @event;
    }

    public async Task<ResponseResult> CanDeleteEvent(Guid id, CancellationToken token)
    {
        bool hasRegsitration = await _dbContext.Set<VisitorRegistration>()
                                               .AnyAsync(w => w.EventId == id && w.RegistrationCancelled == false, token);

        if (hasRegsitration) return new(new OperationFailedException("Event Delete", "Cannot delete event as it has active registration(s)"));


        hasRegsitration = await _dbContext.Set<TeamMemberEvent>()
                                          .AnyAsync(w => w.EventId == id && w.ActiveStatus == TeamMemberStatus.Active, token);

        if (hasRegsitration) return new(new OperationFailedException("Event Delete", "Cannot delete event as it has active team member(s)"));


        return new();
    }

    public async Task<Event?> GetEventBySpec(ISpecification<Event> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var @event = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return @event;
    }

    public async Task<TResult?> GetProjectedEventBySpec<TResult>(ISpecification<Event, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Event, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<bool> IsEventNameTaken(string name, Guid? eventId = null, CancellationToken cancellationToken = default)
    {
        var isEmailTaken = await _table.AnyAsync(f => f.Name == name && (eventId == null || f.Id != eventId.Value), cancellationToken);

        return isEmailTaken;
    }

    public async Task<bool> DoesAllProvidedEventsExist(IEnumerable<Guid> eventId, CancellationToken cancellation)
    {
        eventId = eventId.Distinct().ToList();

        var eventIds = await _table.AsNoTracking()
                                   .Where(w => eventId.Contains(w.Id))
                                   .Select(s => s.Id)
                                   .ToListAsync(cancellationToken: cancellation);

        var doesAllProvidedEventsExists = eventId.All(a => eventIds.Contains(a));

        return doesAllProvidedEventsExists;
    }

    public async Task<bool> IsEventActive(Guid facetsEventId, CancellationToken token)
    {
        return await _table.AsNoTracking().Where(w => w.Id == facetsEventId && w.IsDeleted == false && w.Status == EventStatus.Active).AnyAsync(token);
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetActiveAssignedEventListBySpec<TResult>(Paginator paginator, ISpecification<UserAssignedEvent, TResult> specification, CancellationToken token)
    {
        var query = _userAssignedtable.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<ResponseResult> CanUpdateEvent(Guid eventId, CancellationToken token)
    {
        bool hasRegsitration = await _dbContext.Set<VisitorRegistration>()
                                               .AnyAsync(w => w.EventId == eventId && w.RegistrationCancelled == false, token);

        if (hasRegsitration) return new(new OperationFailedException("Event Update", "Cannot update event as it has active registration(s)"));


        hasRegsitration = await _dbContext.Set<TeamMemberEvent>()
                                          .AnyAsync(w => w.EventId == eventId && w.ActiveStatus == TeamMemberStatus.Active, token);

        if (hasRegsitration) return new(new OperationFailedException("Event Update", "Cannot update event as it has active team member(s)"));


        return new();
    }
}
