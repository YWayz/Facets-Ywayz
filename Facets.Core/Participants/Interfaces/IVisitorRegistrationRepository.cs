using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Participants.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Participants.Interfaces;

public interface IVisitorRegistrationRepository : IBaseRepository
{
    VisitorRegistration Add(VisitorRegistration visitorRegistration);

    Task<TResult?> GetProjectedRegistrationBySpec<TResult>(ISpecification<VisitorRegistration, TResult> specification, CancellationToken token);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorRegistration, TResult> specification, CancellationToken token);

    Task<VisitorRegistration?> GetRegistrationBySpec(ISpecification<VisitorRegistration> specification, CancellationToken token, bool asTracking = false);
    Task<VisitorRegistration?> FindById(Guid visitorRegistrationId);
}
