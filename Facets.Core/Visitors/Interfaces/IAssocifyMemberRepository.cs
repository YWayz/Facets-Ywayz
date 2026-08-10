using Facets.Core.Visitors.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Visitors.Interfaces;

public interface IAssocifyMemberRepository
{
    Task<ResponseResult<bool>> IsVisitorAnAssocifyMember(int tenantId, string? nICNumber, string? passportNumber);
    Task<VisitorSearchDto?> GetMemberBySearchValue(int tenantId, string? searchValue);
}
