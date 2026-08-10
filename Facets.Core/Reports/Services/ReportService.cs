using Facets.Core.Reports.Dtos;
using Facets.Core.Reports.Filters;
using Facets.Core.Reports.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Reports.Services;

internal sealed class ReportService : IReportService
{
    private readonly IVisitorReportRepository _visitorReportRepository;

    public ReportService(IVisitorReportRepository visitorReportRepository)
    {
        _visitorReportRepository = visitorReportRepository;
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorReportDto>>> VisitorReport(Paginator paginator, VisitorReportFilter filter, CancellationToken token)
    {
        var visitorReportDetails = await _visitorReportRepository.VisitorReport(paginator, filter, token);
        return visitorReportDetails;
    }

    public async Task<ResponseResult<IReadOnlyList<CollectionReportDto>>> CollectionReport(CollectionReportFilter filter, CancellationToken token)
    {
        var visitorCollectionReportDetails = await _visitorReportRepository.VisitorCollectionReport(filter, token);
        return visitorCollectionReportDetails;
    }
}
