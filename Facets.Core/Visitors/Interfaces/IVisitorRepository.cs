using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Visitors.Interfaces;

public interface IVisitorRepository : IBaseRepository
{
    Visitor Add(Visitor visitor);
    Task<TResult?> GetProjectedVisitorBySpec<TResult>(ISpecification<Visitor, TResult> specification, CancellationToken token);

    Task<Visitor?> GetVisitorBySpec(ISpecification<Visitor> specification, CancellationToken token, bool asTracking = false);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Visitor, TResult> specification, CancellationToken token);

    void AddVisitorActivity(VisitorActivity visitorActivity);
    
    Task<bool> VisitorRegistered(string identificationNumber, CancellationToken cancellationToken);

    Task<ResponseResult<IReadOnlyList<AttendenceReportDto>>> AttendanceCollectionReport(Paginator paginator, AttendancereportFilter filter, CancellationToken token);
}
