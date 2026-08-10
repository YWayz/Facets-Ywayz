using Facets.Core.Common.Validators;
using Facets.Core.Events.Entities;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Specs;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Services;

internal sealed class VisitorPavilionService : IVisitorPavilionService
{
    private readonly IVisitorPavilionSessionRepository _visitorPavilionSessionRepository;
    private readonly IModelValidator _validator;
    private readonly IVisitorRegistrationRepository _visitorRegistrationRepository;
    private readonly ILoggedInUserService _loggedInUserService;

    public VisitorPavilionService(IVisitorPavilionSessionRepository visitorPavilionSessionRepository,
                                  IModelValidator validator,
                                  IVisitorRegistrationRepository visitorRegistrationRepository,
                                  ILoggedInUserService loggedInUserService)
    {
        _visitorPavilionSessionRepository = visitorPavilionSessionRepository;
        _validator = validator;
        _visitorRegistrationRepository = visitorRegistrationRepository;
        _loggedInUserService = loggedInUserService;
    }

    public async Task<ResponseResult<IReadOnlyList<VisitorPavilionSessionAttendanceScheduleDto>>> GetVisitorPavilionSessions(Guid visitorRegistrationId,
                                                                                                                             CancellationToken token)
    {
        var (list, totalRecords) = await _visitorPavilionSessionRepository.GetProjectedListBySpec(new VisitorPavilionSessionAttendanceSpec
                                                                                                      (visitorRegistrationId),
                                                                                                  token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult> RegistorVisitorToPavilionSessions(AddVisitorToPavilionSessionInternalDto model, CancellationToken token)
    {
        var pavilionSessions = await _visitorPavilionSessionRepository.GetListBySpec(new AddVisitorToPavilsionSessionSpec(model.PavilionSessionIDs),
                                                                                     token,
                                                                                     asTracking: true);

        var visitorCountValidationResult = await ValidateRegisteredPavilionSessionVisitorsCount(pavilionSessions,
                                                                                                model.PavilionSessionIDs,
                                                                                                model.EventDateIDs,
                                                                                                token);

        if (visitorCountValidationResult.Success is false) return visitorCountValidationResult;


        foreach (var pavilionSession in pavilionSessions)
        {
            var response = pavilionSession.AddVisitor(model.VisitorRegistrationId, model.PavilionSessionIDs, model.EventDateIDs);

            if (response.Success is false) return response;
        }

        return new();
    }
    public async Task<ResponseResult> UpdateVisitorPavilionSessions(UpdateVisitorToPavilionSessionInternalDto model, CancellationToken token)
    {
        var visitorRegistration = await _visitorRegistrationRepository.FindById(model.VisitorRegistrationId);

        var pavilionSessions = await _visitorPavilionSessionRepository.GetListBySpec(new AddVisitorToPavilsionSessionSpec(model.PavilionSessionIDs),
                                                                                     token,
                                                                                     asTracking: true);

        var eventDateIDs = visitorRegistration!.VisitorAttendanceSchedules.Where(w => w.Cancelled is false).Select(s => s.EventDateId).ToList();

        var visitorCountValidationResult = await ValidateRegisteredPavilionSessionVisitorsCount(pavilionSessions,
                                                                                                model.PavilionSessionIDs,
                                                                                                eventDateIDs,
                                                                                                token);

        if (visitorCountValidationResult.Success is false) return visitorCountValidationResult;


        foreach (var pavilionSession in pavilionSessions)
        {
            var response = pavilionSession.AddVisitor(model.VisitorRegistrationId, model.PavilionSessionIDs, eventDateIDs);

            if (response.Success is false) return response;
        }

        return new();
    }

    private async Task<ResponseResult> ValidateRegisteredPavilionSessionVisitorsCount(IEnumerable<PavilionSession> pavilionSessions,
                                                                                      IEnumerable<Guid> pavilionSessionIDs,
                                                                                      IEnumerable<Guid> eventDateIDs,
                                                                                      CancellationToken token)
    {
        var visitorPavilionSessionCount = await _visitorPavilionSessionRepository.GetVisitorCountByPavilionSessions(pavilionSessionIDs, token);

        foreach (var pavilionSession in pavilionSessions)
        {
            var pavilionSessionVisitorCount = visitorPavilionSessionCount.First(a => a.Key == pavilionSession.Id);

            if (pavilionSession.AllowedVisitorCount > pavilionSessionVisitorCount.Value) continue;

            return new(new OperationFailedException("Visitor Count", "Visitor Count has been exceeded for the session"));
        }

        return new();
    }

    public async Task<ResponseResult<IReadOnlyCollection<PavilionSessionVisitorDto>>> GetPavilionSessionVisitors(Paginator paginator, PavilionSessionVisitorFilter filter, CancellationToken cancellationToken)
    {
        var result = await _visitorPavilionSessionRepository.GetPavilionSessionVisitors(paginator, _loggedInUserService.FacetsEventId, filter, cancellationToken);
       return new(result.list, result.totalRecordCount);
    }
}
