using Facets.Core.Common.Dtos;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Interfaces;

public interface IVisitorService
{
    internal Task<ResponseResult<VisitorCreatedDto>> RegisterVisitor(RegisterVisitorInternalDto model, CancellationToken cancellationToken);

    Task<ResponseResult<VisitorSearchDto>> SearchVisitor(string? searchValue, CancellationToken token);

    Task<ResponseResult<IReadOnlyList<FileDto>>> AddDocumentsToExistingVisitor(Guid visitorId, List<KeyValuePair<AttachmentType, IFormFile>> files, CancellationToken token);
    Task<ResponseResult> UpdateVisitor(Guid visitorId, UpdateVisitorDto model, CancellationToken token);
    Task<ResponseResult<VisitorDto>> GetVisitorById(Guid id, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<VisitorSummaryDto>>> GetVisitors(Paginator paginator, VisitorFilter filter, CancellationToken token);
    internal void AddVisitorActivity(VisitorActivity visitorActivity);
    Task<ResponseResult<IReadOnlyList<FileDto>>> GetDocuments(Guid visitorId, VisitorDocumentFilter filter, CancellationToken token);
    Task<ResponseResult> DeleteDocument(Guid visitorId, Guid id, CancellationToken token);
    Task<ResponseResult> MarkAsBlackListed(Guid id, MarkAsBlackListedDto model, CancellationToken token);
    Task<ResponseResult> RemoveFromBlackList(Guid visitorId, CancellationToken token);
    Task<ResponseResult<VisitorDto>> GetVisitorByIdentificationNumber(string identityNumber, CancellationToken cancellationToken);
    Task<ResponseResult> UpdateOTPVerificationStatus(string identityNumber, CancellationToken cancellationToken);
    Task<ResponseResult<IReadOnlyList<VisitorActivityDto>>> GetVisitorActivities(Paginator paginator, Guid visitorId, CancellationToken token);

    Task<ResponseResult<IReadOnlyList<AttendenceReportDto>>> AttendanceReport(Paginator paginator, AttendancereportFilter filter, CancellationToken token);
}
