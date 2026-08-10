using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Events;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using MediatR;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.EventHandler;

internal sealed class TeamMemberRegisteringActivityToEventEventHandler : INotificationHandler<TeamMemberRegisteringToEventEvent>
{
    private readonly ITeamMemberActivityService _teamMemberActivityService;    

    public TeamMemberRegisteringActivityToEventEventHandler(ITeamMemberActivityService teamMemberActivityService)
    {
        _teamMemberActivityService = teamMemberActivityService;
    }

    public Task Handle(TeamMemberRegisteringToEventEvent notification, CancellationToken cancellationToken)
    {
        var registrationDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        _teamMemberActivityService.AddTeamMemberActivity(new TeamMemberActivity(notification.TeamMemberEvent.EventId,
                                                                                notification.TeamMemberEvent.TeamMemberId,
                                                                                $"{registrationDateAndTime} - Registered", 
                                                                                TeamMemberActivityType.RegistrationCancelled));

        return Task.CompletedTask;
    }
}
