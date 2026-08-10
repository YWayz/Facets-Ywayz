using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Events;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using MediatR;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.EventHandler;

internal sealed class TeamMemberEventRegistrationCancellatingActivityEventHandler : INotificationHandler<TeamMemberEventRegisterationCancellingEvent>
{
    private readonly ITeamMemberActivityService _teamMemberActivityService;

    public TeamMemberEventRegistrationCancellatingActivityEventHandler(ITeamMemberActivityService teamMemberActivityService)
    {
        _teamMemberActivityService = teamMemberActivityService;
    }

    public Task Handle(TeamMemberEventRegisterationCancellingEvent notification, CancellationToken cancellationToken)
    {
        var registrationDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        _teamMemberActivityService.AddTeamMemberActivity(new TeamMemberActivity(notification.TeamMemberEvent.EventId,
                                                                                notification.TeamMemberEvent.TeamMemberId,
                                                                                $"{registrationDateAndTime} - Cancelled registration",
                                                                                TeamMemberActivityType.RegistrationCancelled));

        return Task.CompletedTask;
    }
}
