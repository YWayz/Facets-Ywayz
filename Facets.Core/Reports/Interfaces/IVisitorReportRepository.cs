using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Reports.Interfaces;

public interface IVisitorReportRepository
{
    Task<ResponseResult<IReadOnlyList<VisitorReportDto>>> VisitorReport(Paginator paginator, VisitorReportFilter filter, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<CollectionReportDto>>> VisitorCollectionReport(CollectionReportFilter filter, CancellationToken token);
}
