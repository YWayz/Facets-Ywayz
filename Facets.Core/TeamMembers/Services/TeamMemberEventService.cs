using Facets.Core.Common.Interfaces;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Filters;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.TeamMembers.Specs;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using System.Text;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Services;

public sealed class TeamMemberEventService : ITeamMemberEventService
{
    private readonly ITeamMemberEventRepository _teamMemberEventRepository;
    private readonly IPassTemplateService _passTemplateService;
    private readonly IFileRespository _fileRespository;
    private readonly IApplicationContext _applicationContext;
    private readonly ITeamMemberActivityService _teamMemberActivityService;
    private readonly ILoggedInUserService _loggedInUser;

    public TeamMemberEventService(ITeamMemberEventRepository teamMemberEventRepository, ILoggedInUserService loggedInUser, IPassTemplateService passTemplateService, IFileRespository fileRespository, IApplicationContext applicationContext, ITeamMemberActivityService teamMemberActivityService)
    {
        _teamMemberEventRepository = teamMemberEventRepository;
        _passTemplateService = passTemplateService;
        _fileRespository = fileRespository;
        _applicationContext = applicationContext;
        _teamMemberActivityService = teamMemberActivityService;
        _loggedInUser = loggedInUser;
    }

    public async Task<ResponseResult<IReadOnlyList<TeamMemberEventDto>>> GetTeamMembersByEvent(Paginator paginator, TeamMemberEventFilter filter, CancellationToken token)
    {
        var (list, totalRecords) = await _teamMemberEventRepository.GetProjectedListBySpec(paginator, new TeamMembersByEventSpec(filter), token);

        return new(list, totalRecords);

    }

    public async Task<ResponseResult> RemoveTeamMemberFromEvent(Guid eventId, Guid teamMemberId, CancellationToken cancellationToken)
    {
        var teamMemberEvent = await _teamMemberEventRepository.GetTeamMemberEventBySpec(new TeamMemberEventByIdAndEventIdSpec(teamMemberId, eventId), cancellationToken, asTracking: true);

        if (teamMemberEvent is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        teamMemberEvent.CancelTeamMemberFromEvent();

        await _teamMemberEventRepository.SaveChangesAsync(cancellationToken);
        return new();
    }

    public async Task<ResponseResult<TeamMemberPassTemplateDto>> GetTeamMemberPassGenerationTemplate(Guid teamMemberId, CancellationToken token)
    {
        var template = await _passTemplateService.GetPassTemplates(new Paginator(), _loggedInUser.FacetsEventId, new PassTemplateFilter(PassType.TeamMember), token);

        if (template.TotalRecordCount is 0) return new ResponseResult<TeamMemberPassTemplateDto>(new OperationFailedException("Team Member Pass Template", "No Pass template available for current event"));

        var teamMemberPass = await _teamMemberEventRepository.GetPassTemplateTeamMemberBySpec(new PassTemplateTeamMemberByTeamMemberIdSpec(teamMemberId, _loggedInUser.FacetsEventId), token);

        var imageResponse = await _fileRespository.DownloadAsStream(AppConstants.BlobStorage.ContainerName.TeamMemberProfileImage, teamMemberPass.ProfileImage, token);

        if (imageResponse.Success is false) return new(imageResponse.Errors);

        var templateData = template.Data!.First();

        StringBuilder templateBuilder = new(templateData.TemplateText);

        var imageData = ImageHelper.GetThumbnailAsBase64(imageResponse.Data!);

        string templateText = templateBuilder
                              .Replace("Event Name", teamMemberPass.EventName.ToUpper())
                              .Replace("First Name", teamMemberPass.FirstName.ToUpper())
                              .Replace("Last Name", teamMemberPass.LastName.ToUpper())
                              .Replace("NIC Number/ Passport Number", teamMemberPass.VisitorIdentityType == VisitorIdentityType.NIC ?
                                                                      teamMemberPass.NICNumber!.ToUpper() : teamMemberPass.PassportNumber!.ToUpper())
                              .Replace("Mobile Number", teamMemberPass.MobileNumber)
                              .Replace("Pass Date", teamMemberPass.PassDate.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat())
                              .Replace("Pass Generated Date & Time", teamMemberPass.PassGeneratedDateTime.ToString().ToUpper())
                              .Replace("Company", teamMemberPass.CompanyName?.ToUpper())
                              //.Replace($"{_applicationContext.FEUrl}assets/images/user-profile.png", imageData)
                              .Replace($"https://exhibition.facetssrilanka.com/assets/images/user-profile.png", imageData)
                              .ToString();

        //var img = ImageHelper.GetThumbnailAsBase64(imageResponse.Data!);

        var fullName = teamMemberPass.FirstName.ToUpper() + " " + teamMemberPass.LastName.ToUpper();

        await AddTeamMemberPassGenerationActivity(_loggedInUser.FacetsEventId, teamMemberId, teamMemberPass);

        return new ResponseResult<TeamMemberPassTemplateDto>(new TeamMemberPassTemplateDto(templateData.Width, templateData.Height, templateText, teamMemberPass.PassCategoryColor, teamMemberPass.PassCategoryName.ToUpper(), teamMemberPass.ProfileImage, fullName));

        async Task AddTeamMemberPassGenerationActivity(Guid eventId, Guid teamMemberId, PassTemplateTeamMemberDto teamMemberPass)
        {
            string description = $"{teamMemberPass.PassGeneratedDateTime} - Generated pass";

            TeamMemberActivity activity = new(eventId, teamMemberId, description, TeamMemberActivityType.PassGenerated);

            _teamMemberActivityService.AddTeamMemberActivity(activity);

            await _teamMemberEventRepository.SaveChangesAsync(token);
        }
    }
  
    public async Task<ResponseResult<QRVerifiedTeamMemberDto>> VerifyPass(TeamMemberPassVerificationDto model, CancellationToken token)
    {
        var teamMeberEvent = await _teamMemberEventRepository.GetPassToVerify(model.EventId, model.TeamMemberId, token);

        if (teamMeberEvent is null)
            return new(new OperationFailedException("Team Member Event", "Team member is not assigned to this event"));

        return new(teamMeberEvent);
    }
}
