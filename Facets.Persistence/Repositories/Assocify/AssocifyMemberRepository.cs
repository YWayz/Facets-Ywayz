using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Assocify;

public sealed class AssocifyMemberRepository : IAssocifyMemberRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AssocifyMemberRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<VisitorSearchDto?> GetMemberBySearchValue(int tenantId, string? searchValue)
    {
        string apiKey = _configuration["Assocify:FuncAppKeys:GetMemberBySearchValue"] ?? string.Empty;
        searchValue = searchValue ?? string.Empty;
        var httResponse = await _httpClient.GetAsync($"api/tenant/{tenantId}/member/{searchValue}?{apiKey}");

        if (httResponse.IsSuccessStatusCode is false && httResponse.StatusCode is HttpStatusCode.NotFound) return null;

        var member = await httResponse.Content.ReadFromJsonAsync<MemberDto>();

        return new VisitorSearchDto(IsAssocifyMember: true,
                                    IsRegisteredToFacets: false,
                                    FirstName: member?.FirstName,
                                    LastName: member?.LastName,
                                    NICNumber: member?.NIC,
                                    PassportNumber: null,
                                    MobileNumber: member?.PhoneNumber,
                                    Email: member?.Email,
                                    VisitorId: null,
                                    VisitorStatus: VisitorStatus.Active);
    }

    public async Task<ResponseResult<bool>> IsVisitorAnAssocifyMember(int tenantId, string? nICNumber, string? passportNumber)
    {
        string apiKey = _configuration["Assocify:FuncAppKeys:IsMemberAvailable"] ?? string.Empty;
        string searchValue = nICNumber ?? passportNumber ?? string.Empty;
        var content = await _httpClient.GetFromJsonAsync<IsMemberAvailable>($"api/tenant/{tenantId}/member/{searchValue}/has-member?{apiKey}");

        //return new(content?.MemberAvailable ?? false);
        return new(content?.MemberAvailable ?? false);
    }
}

file record IsMemberAvailable(bool MemberAvailable);

file sealed class MemberDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? NIC { get; set; }
}

