using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Entities;
using Facets.Core.Counters.Filters;
using Facets.SharedKernal.Models;

namespace Facets.Core.Counters.Interfaces;

public interface IRegistrationCounterRepository : IBaseRepository
{
    VisitorRegistrationCounter AddRegistrationCounter(VisitorRegistrationCounter entity);

    Task<bool> IsRegistrationCounterNameTaken(Guid eventId, string name, Guid? id = null, CancellationToken cancellationToken = default);

    Task<TResult?> GetProjectedRegistrationCounterBySpec<TResult>(ISpecification<VisitorRegistrationCounter, TResult> specification, CancellationToken token);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorRegistrationCounter, TResult> specification, CancellationToken token);

    Task<VisitorRegistrationCounter?> GetRegistrationCounterBySpec(ISpecification<VisitorRegistrationCounter> specification, CancellationToken token, bool asTracking = false);

    Task<VisitorRegistrationCounter?> UnAssignRegistrationCounter(Guid eventId, string userId);

    Task<UserAssignedRegistrationCounterDto?> GetCounterAssignmentForCurrentUser(Guid facetsEventId, string userId, CounterAssignmentFilter? filter, CancellationToken token);
}
