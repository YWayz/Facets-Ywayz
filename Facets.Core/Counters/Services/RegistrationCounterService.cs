using Facets.Core.Common.Validators;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Entities;
using Facets.Core.Counters.Filters;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Counters.Specs;
using Facets.Core.Counters.Validators;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Counters.Services;

public sealed class RegistrationCounterService : IRegistrationCounterService
{
    private readonly IRegistrationCounterRepository _registrationCounterRepository;
    private readonly IModelValidator _validator;
    private readonly ILoggedInUserService _loggedInUser;

    public RegistrationCounterService(IRegistrationCounterRepository registrationCounterRepository, IModelValidator validator, ILoggedInUserService loggedInUser)
    {
        _registrationCounterRepository = registrationCounterRepository;
        _validator = validator;
        _loggedInUser = loggedInUser;
    }

    public async Task<ResponseResult<RegistrationCounterDto>> CreateRegistrationCounter(Guid eventId, CreateRegistrationCounterDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<CreateRegistrationCounterDtoValidator, CreateRegistrationCounterDto>(model, cancellationToken);

        bool isNameTaken = await _registrationCounterRepository.IsRegistrationCounterNameTaken(eventId, model.Name);

        if (isNameTaken) return new ResponseResult<RegistrationCounterDto>(new BadRequestException(nameof(model.Name), "Registration counter name is already taken"));

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        VisitorRegistrationCounter visitorRegistrationCounter = new(model.Name,
                           model.Description,
                           eventId,
                           model.CounterType);

        _registrationCounterRepository.AddRegistrationCounter(visitorRegistrationCounter);

        await _registrationCounterRepository.SaveChangesAsync(cancellationToken);

        return new(new RegistrationCounterDto(visitorRegistrationCounter.Id,
                                              visitorRegistrationCounter.Name,
                                              visitorRegistrationCounter.Description,
                                              visitorRegistrationCounter.IsLocked,
                                              visitorRegistrationCounter.EventId,
                                              visitorRegistrationCounter.UserAssignedRegistrationCounters.Select(s => s.AssginedUser.FirstName + ' ' + s.AssginedUser.LastName).FirstOrDefault(),
                                              visitorRegistrationCounter.CounterType));
    }

    public async Task<ResponseResult<RegistrationCounterDto>> GetRegistrationCounterById(Guid eventId, Guid id, CancellationToken token)
    {
        var registrationCounterDetailDto = await _registrationCounterRepository.GetProjectedRegistrationCounterBySpec(new RegistrationCounterByIdSpec(eventId, id), token);

        if (registrationCounterDetailDto is null) return new(new NotFoundException(nameof(id), "Registration counter", id));

        return new(registrationCounterDetailDto);
    }

    public async Task<ResponseResult<IReadOnlyList<RegistrationCounterDto>>> GetRegistrationCounters(Guid eventId, Paginator paginator, RegistrationCounterFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _registrationCounterRepository.GetProjectedListBySpec(paginator,
                                                                                               new RegistrationCounterListSpec(eventId, filter),
                                                                                               token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult> UpdateRegistrationCounter(Guid eventId, Guid id, UpdateRegistrationCounterDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<UpdateRegistrationCounterDtoValidator, UpdateRegistrationCounterDto>(model, token);

        bool isNameTaken = await _registrationCounterRepository.IsRegistrationCounterNameTaken(eventId, model.Name, id, token);

        if (isNameTaken) return new ResponseResult(new BadRequestException(nameof(model.Name), "Registration counter name is already taken"));

        if (validationResult.IsValid is false) return new ResponseResult(validationResult.Errors);

        var registrationCounter = await _registrationCounterRepository.GetRegistrationCounterBySpec(new RegistrationCounterUpdateSpec(eventId, id),
                                                                                                    token,
                                                                                                    asTracking: true);

        if (registrationCounter is null) return new ResponseResult(new NotFoundException(nameof(id), "Registration counter", id));

        ResponseResult result = registrationCounter.UpdateRegistrationCounterInfo(model.Name, model.Description, model.CounterType);

        if (result.Success is false) return result;

        await _registrationCounterRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult> Delete(Guid eventId, Guid id, CancellationToken token)
    {
        var registrationCounter = await _registrationCounterRepository.GetRegistrationCounterBySpec(new RegistrationCounterDeleteSpec(eventId, id),
                                                                                                    asTracking: true,
                                                                                                    token: token);

        if (registrationCounter is null) return new ResponseResult(new NotFoundException(nameof(id), nameof(VisitorRegistrationCounter), id));

        var response = registrationCounter.Delete();

        if (response.Success is false) return response;

        await _registrationCounterRepository.SaveChangesAsync(token);

        return new ResponseResult();
    }

    public async Task<ResponseResult<UserAssignedRegistrationCounterDto>> GetUserAssignedRegistrationCounter(Guid eventId, CancellationToken token)
    {
        var registrationCounterDetailDto = await _registrationCounterRepository
                                                 .GetCounterAssignmentForCurrentUser(_loggedInUser.FacetsEventId,
                                                                                     _loggedInUser.UserId,
                                                                                     null,
                                                                                     token);

        if (registrationCounterDetailDto is null)
            return new(new OperationFailedException(nameof(_loggedInUser.UserId), "No counter assigned to current user"));

        return new(registrationCounterDetailDto);
    }

    public async Task<ResponseResult<UserAssignedRegistrationCounterDto>> AssignRegistrationCounter(Guid counterId, AssignRegistrationCounterDto model, CancellationToken token)
    {

        await UnAssignRegistrationCounter(token);

        var registrationCounter = await _registrationCounterRepository.GetRegistrationCounterBySpec(new RegistrationCounterForAssignmentSpec
                                                                                                    (_loggedInUser.FacetsEventId, counterId),
                                                                                                    token,
                                                                                                    asTracking: true);

        if (registrationCounter is null) return new(new NotFoundException(nameof(counterId), "Registration counter", counterId));

        if (registrationCounter.IsLocked) return new(new OperationFailedException(nameof(counterId), "Registration counter is locked"));

        registrationCounter.AssignRegistrationCounter(model.userId);

        await _registrationCounterRepository.SaveChangesAsync(token);

        return new(new UserAssignedRegistrationCounterDto(registrationCounter.UserAssignedRegistrationCounters.First().Id,
                                                          registrationCounter.EventId,
                                                          registrationCounter.Name,
                                                          registrationCounter.IsLocked,
                                                          registrationCounter.Id,
                                                          registrationCounter.UserAssignedRegistrationCounters.First().AssignedUserId,
                                                          registrationCounter.CounterType));
    }

    public async Task<ResponseResult<CounterAssignmentStatusDto>> GetCounterAssignmentForCurrentUser(CounterAssignmentFilter filter, CancellationToken token)
    {
        Guid parsedLoggedInUserId = Guid.Parse(_loggedInUser.UserId);


        var registrationCounterDetailDto = await _registrationCounterRepository
                                                 .GetCounterAssignmentForCurrentUser(_loggedInUser.FacetsEventId,
                                                                                     _loggedInUser.UserId,
                                                                                     filter,
                                                                                     token);

        if (registrationCounterDetailDto is null ||
            registrationCounterDetailDto.AssignedUserId != parsedLoggedInUserId)
            return new(new CounterAssignmentStatusDto(hasCounterAssigned: false, null, null, null));

        return new(new CounterAssignmentStatusDto(hasCounterAssigned: true,
                                                  AssignmentId: registrationCounterDetailDto.Id,
                                                  CounterId: registrationCounterDetailDto.RegistrationCounterId,
                                                  AssignedUserId: registrationCounterDetailDto.AssignedUserId));
    }



    public async Task<ResponseResult> UnAssignRegistrationCounter(CancellationToken token)
    {
        var counter = await _registrationCounterRepository.UnAssignRegistrationCounter(_loggedInUser.FacetsEventId, _loggedInUser.UserId);

        counter?.UnAssignRegistrationCounter(Guid.Parse(_loggedInUser.UserId));

        return new();
    }

    public async Task<ResponseResult> UnlockCounter(Guid eventId, Guid counterId, CancellationToken token)
    {
        var registrationCounterDetailDto = await _registrationCounterRepository.GetRegistrationCounterBySpec(new UnlockRegistrationCounterSpec(eventId,
                                                                                                                                             counterId),
                                                                                                             token,
                                                                                                             asTracking: true);

        if (registrationCounterDetailDto is null) return new(new NotFoundException(nameof(counterId), "Registration counter", counterId));

        var unlockResponse = registrationCounterDetailDto.Unlock(Guid.Parse(_loggedInUser.UserId));

        if (unlockResponse.Success is false) return new(unlockResponse.Errors);

        await _registrationCounterRepository.SaveChangesAsync(token);

        return new();

    }
}
