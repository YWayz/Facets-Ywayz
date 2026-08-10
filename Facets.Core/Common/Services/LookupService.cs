using Facets.Core.Common.Dtos;
using Facets.Core.Common.Filters;
using Facets.Core.Common.Interfaces;
using Facets.Core.Common.Specs;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Interfaces;
using Facets.Core.Lookups.Interfaces;
using Facets.Core.Passes.Interfaces;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Common.Services;

internal sealed class LookupService : ILookupService
{
    private readonly IEventRepository _eventRepository;
    private readonly IPassCategoryRepository _passCategoryRepository;
    private readonly IRegistrationCounterRepository _registrationCounterRepository;
    private readonly IPavilionRepository _pavilionRepository;
    private readonly ILookupRepository _lookupRepository;
    private readonly ILoggedInUserService _loggedInUser;

    public LookupService(IEventRepository eventRepository, 
                         ILookupRepository lookupRepository, 
                         IPassCategoryRepository passCategoryRepository,
                         IRegistrationCounterRepository registrationCounterRepository, 
                         IPavilionRepository pavilionRepository, 
                         ILoggedInUserService loggedInUser)
    {
        _eventRepository = eventRepository;
        _lookupRepository = lookupRepository;
        _passCategoryRepository = passCategoryRepository;
        _registrationCounterRepository = registrationCounterRepository;
        _pavilionRepository = pavilionRepository;
        _loggedInUser = loggedInUser;
    }

    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetEventList(Paginator paginator, EventLookupFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _eventRepository.GetProjectedListBySpec(paginator, new EventLookupListSpec(filter), token);
        return new(list, totalRecords);
    }

    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetUserAssignedEventList(Paginator paginator, CancellationToken token)
    {
        var (list, totalRecords) = await _eventRepository.GetActiveAssignedEventListBySpec(paginator, 
                                                                                           new EventByUserLookupSpec(_loggedInUser.UserId), 
                                                                                           token);

        return new(list, totalRecords);
    }


    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetCountries(CancellationToken token)
    {
        var countries = await _lookupRepository.GetCountries(token);

        var countryDtos = countries.Select(c => new KeyValuePair<Guid, string>(c.Id, c.Name))
                                   .ToList();

        return new ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>(countryDtos, countryDtos.Count);
    }

    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetPassCategories(Guid eventId, Paginator paginator, PassCategoryLookupFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _passCategoryRepository.GetProjectedListBySpec(paginator, 
                                                                                        new PassCategoryLookupListSpec(eventId, filter),
                                                                                        token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<IReadOnlyList<RegistrationCounterLookUpDto>>> GetRegistrationCounters(Guid eventId, Paginator paginator, RegistrationCounterLookupFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _registrationCounterRepository.GetProjectedListBySpec(paginator,
                                                                                               new RegistrationCounterLookupListSpec(eventId, filter),
                                                                                               token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetAssignedUsersToEvent(Guid eventId, CancellationToken token)
    {
        var result = await _lookupRepository.GetAssignedUsersToEvent(eventId, token);

        return new(result, result.Count);
    }

    public async Task<ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>> GetPavilions(Guid eventId, CancellationToken token)
    {
        var result = await _lookupRepository.GetPavilions(eventId, token);

        return new(result, result.Count);
    }

    public async Task<ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>> GetPavilionSessions(Guid eventId, Guid pavilionId, CancellationToken token)
    {
        var result = await _lookupRepository.GetPavilionSessions(eventId, pavilionId, token);

        return new(result, result.Count);
    }
}