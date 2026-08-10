using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Counters.Interfaces;

public interface IRegistrationCounterService
{
    Task<ResponseResult<RegistrationCounterDto>> CreateRegistrationCounter(Guid eventId, CreateRegistrationCounterDto model, CancellationToken cancellationToken);
    Task<ResponseResult<RegistrationCounterDto>> GetRegistrationCounterById(Guid eventId, Guid id, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<RegistrationCounterDto>>> GetRegistrationCounters(Guid eventId, Paginator paginator, RegistrationCounterFilter filter, CancellationToken token);
    Task<ResponseResult> UpdateRegistrationCounter(Guid eventId, Guid id, UpdateRegistrationCounterDto model, CancellationToken token);
    Task<ResponseResult> Delete(Guid eventId, Guid id, CancellationToken token);

    internal Task<ResponseResult<UserAssignedRegistrationCounterDto>> GetUserAssignedRegistrationCounter(Guid eventId, CancellationToken token);

    Task<ResponseResult<UserAssignedRegistrationCounterDto>> AssignRegistrationCounter(Guid counterId, AssignRegistrationCounterDto model, CancellationToken token);
    Task<ResponseResult<CounterAssignmentStatusDto>> GetCounterAssignmentForCurrentUser(CounterAssignmentFilter filter, CancellationToken token);

    Task<ResponseResult> UnAssignRegistrationCounter(CancellationToken token);
    Task<ResponseResult> UnlockCounter(Guid eventId, Guid id, CancellationToken token);
}
