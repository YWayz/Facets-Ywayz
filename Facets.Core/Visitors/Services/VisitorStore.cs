using Facets.Core.Common.Validators;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Configuration;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Services;

internal sealed class VisitorStore : IVisitorStore
{
    private readonly IAssocifyMemberRepository _associfyMemberService;
    private readonly IConfiguration _configuration;
    private readonly IRegistrationCounterService _registrationCounterService;
    private readonly IVisitorService _visitorService;
    private readonly IModelValidator _validator;

    public VisitorStore(IAssocifyMemberRepository associfyMemberRepository, IConfiguration configuration, IRegistrationCounterService registrationCounterService, IVisitorService visitorService, IModelValidator validator)
    {
        _associfyMemberService = associfyMemberRepository;
        _configuration = configuration;
        _registrationCounterService = registrationCounterService;
        _visitorService = visitorService;
        _validator = validator;
    }

    public async Task<ResponseResult<VisitorCreatedDto>> Register(RegisterVisitorDto model, bool registerOnline, CancellationToken cancellationToken)
    {
        int tenantId = int.Parse(_configuration["Assocify:TenantId"] ?? "0");

        var associfyMemberResponse = await _associfyMemberService.IsVisitorAnAssocifyMember(tenantId, model.NICNumber, model.PassportNumber);

        if (associfyMemberResponse.Success is false) return new(new OperationFailedException("Associvy Memeber", "Assocify API failed"));

        RegisterVisitorInternalDto internalModel = new(model.VisitorIdentityType,
                                                             model.NICNumber,
                                                             model.PassportNumber,
                                                             model.CountryId,
                                                             model.FirstName,
                                                             model.LastName,
                                                             model.MobileNumber,
                                                             model.CompanyName,
                                                             model.Email,
                                                             model.Address,
                                                             IsAssocifyMember: associfyMemberResponse.Data,
                                                             RegisterOnline: registerOnline);

        var visitorRegistrationResponse = await _visitorService.RegisterVisitor(internalModel, cancellationToken);

        if (visitorRegistrationResponse.Success is false) return visitorRegistrationResponse;

        return visitorRegistrationResponse;
    }

    public async Task<ResponseResult<VisitorSearchDto>> SearchVisitor(string? searchValue, CancellationToken token)
    {
        var visitorResponse = await _visitorService.SearchVisitor(searchValue, token);

        if (visitorResponse.Success) return visitorResponse;

        //int associfyTenantId = int.Parse(_configuration["Assocify:TenantId"] ?? "0");

        //var associfyMemeber = await _associfyMemberService.GetMemberBySearchValue(associfyTenantId, searchValue);

        //if (associfyMemeber is not null) return new(associfyMemeber);

        return new(new VisitorSearchDto(IsAssocifyMember: false,
                                        IsRegisteredToFacets: false,
                                        FirstName: null,
                                        LastName: null,
                                        NICNumber: null,
                                        PassportNumber: null,
                                        MobileNumber: null,
                                        Email: null,
                                        VisitorId: null,
                                        VisitorStatus: null));
    }

    public async Task<ResponseResult<PublicVisitorSearchDto>> PublicSearchVisitor(string? searchValue, CancellationToken token)
    {
        var searchResponse = await this.SearchVisitor(searchValue, token);

        if (searchResponse.Success is false) return new(searchResponse.Errors);

        if (searchResponse.Data is null) return new(new NotFoundException(nameof(searchValue), "Visitor", searchValue ?? string.Empty));

        var visitor = searchResponse.Data;

        PublicVisitorSearchDto searchDto = new(visitor.IsAssocifyMember,
                                               visitor.IsRegisteredToFacets,
                                               visitor.FirstName,
                                               visitor.LastName,
                                               visitor.NICNumber,
                                               visitor.PassportNumber,
                                               visitor.MobileNumber,
                                               visitor.Email,
                                               visitor.VisitorId,
                                               visitor.VisitorStatus);

        return new(searchDto);
    }

    public async Task<ResponseResult<VisitorAvailabilityDto>> AvailabilityCheck(string identificationNumber, CancellationToken token)
    {
        var searchResponse = await this.SearchVisitor(identificationNumber, token);

        if (searchResponse.Success is false) return new(new OperationFailedException("Availability check", "Visitor availability check failed"));

        if (searchResponse.Data is { IsRegisteredToFacets: false, IsAssocifyMember: false })
            return new(new VisitorAvailabilityDto(Available: false, IdentificationNumber: identificationNumber, VisitorStatus: VisitorStatus.None));

        return new(new VisitorAvailabilityDto(Available: true,
                                              IdentificationNumber: identificationNumber,
                                              VisitorStatus: searchResponse.Data!.VisitorStatus!.Value));
    }
}
