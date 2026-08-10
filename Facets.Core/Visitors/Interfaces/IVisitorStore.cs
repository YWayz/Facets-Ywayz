using Facets.Core.Visitors.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Visitors.Interfaces;

public interface IVisitorStore
{
    Task<ResponseResult<VisitorCreatedDto>> Register(RegisterVisitorDto model, bool registerOnline, CancellationToken cancellationToken);
    Task<ResponseResult<VisitorSearchDto>> SearchVisitor(string? searchValue, CancellationToken token);
    Task<ResponseResult<PublicVisitorSearchDto>> PublicSearchVisitor(string? searchValue, CancellationToken token);
    Task<ResponseResult<VisitorAvailabilityDto>> AvailabilityCheck(string identificationNumber, CancellationToken token);
}
