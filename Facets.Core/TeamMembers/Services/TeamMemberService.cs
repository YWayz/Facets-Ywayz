using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.Core.Common.Validators;
using Facets.Core.Passes.Interfaces;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.TeamMembers.Specs;
using Facets.Core.TeamMembers.Validators;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Interfaces;
using Facets.Core.Visitors.Specs;
using Facets.Core.Visitors.Validators;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Http;
using static Facets.Core.Security.Claims.ApplicationClaimValues;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Services;

public sealed class TeamMemberService : ITeamMemberService
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IModelValidator _validator;
    private readonly IFileRespository _fileRespository;
    private readonly IPassCategoryService _passCategoryService;
    private readonly ILoggedInUserService _loggedInUser;

    public TeamMemberService(ITeamMemberRepository teamMemberRepository, IModelValidator modelValidator, IFileRespository fileRespository, IPassCategoryService passCategoryService, ILoggedInUserService loggedInUserService)
    {
        _teamMemberRepository = teamMemberRepository;
        _validator = modelValidator;
        _fileRespository = fileRespository;
        _passCategoryService = passCategoryService;
        _loggedInUser = loggedInUserService;
    }

    public async Task<ResponseResult<CreatedTeamMemberDto>> CreateTeamMember(CreateTeamMemberDto model, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<CreateTeamMemberDtoValidator, CreateTeamMemberDto>(model, token);
        if (validationResult.IsValid is false) return new ResponseResult<CreatedTeamMemberDto>(validationResult.Errors);

        TeamMember teamMember = new(model.Title, model.FirstName, model.LastName, model.MobileNumber, model.Email, model.Address, model.CountryId, model.NicNumber, model.PassportNumber, model.CompanyName, model.IdentityType);

        var createdTeamMember = _teamMemberRepository.AddTeamMember(teamMember);

        await _teamMemberRepository.SaveChangesAsync(token);

        return new(new CreatedTeamMemberDto(
            createdTeamMember.Id,
            createdTeamMember.Title,
            createdTeamMember.IdentityType,
            createdTeamMember.NICNumber,
            createdTeamMember.PassportNumber,
            createdTeamMember.CountryId,
            createdTeamMember.FirstName,
            createdTeamMember.LastName,
            createdTeamMember.MobileNumber,
            createdTeamMember.Email,
            createdTeamMember.Address,
            createdTeamMember.CompanyName
        ));
    }

    public async Task<ResponseResult<TeamMemberDto>> GetTeamMemberById(Guid teamMemberId, CancellationToken token)
    {
        var teamMember = await _teamMemberRepository.GetProjectedTeamMemberBySpec(new TeamMemberByIdAndEventIdSpec(teamMemberId,
                                                                                                                   _loggedInUser.FacetsEventId),
                                                                                                                   token);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        return new(teamMember);
    }

    public async Task<ResponseResult<TeamMemberSearchDto>> SearchMember(string? searchValue, CancellationToken token)
    {
        var teamMemberResponse = await SearchTeamMember(searchValue, token);

        if (teamMemberResponse.Success) return teamMemberResponse;

        return new(new TeamMemberSearchDto(
                                        IsRegisteredToFacets: false,
                                        FirstName: null,
                                        LastName: null,
                                        NICNumber: null,
                                        PassportNumber: null,
                                        MobileNumber: null,
                                        Email: null,
                                        ImageURL: null,
                                        Title: Title.None,
                                        Address: null,
                                        PassCategoryId: null,
                                        CountryId: null,
                                        IdentityType: VisitorIdentityType.None,
                                        Status: TeamMemberStatus.None,
                                        TeamMemberId: null));
    }

    private async Task<ResponseResult<TeamMemberSearchDto>> SearchTeamMember(string? searchValue, CancellationToken token)
    {
        var teamMember = await _teamMemberRepository.GetProjectedTeamMemberBySpec(new TeamMemberSearchSpec(searchValue, _loggedInUser.FacetsEventId), token);

        return new(teamMember);
    }

    public async Task<ResponseResult> DeleteTeamMemberDocument(Guid teamMemberId, Guid documentId, CancellationToken cancellationToken)
    {
        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentDeleteSpec(teamMemberId, documentId), cancellationToken, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        foreach (var document in teamMember.Attachments)
        {
            document.Delete();
        }

        await _teamMemberRepository.SaveChangesAsync(cancellationToken);

        return new();
    }

    public async Task<ResponseResult> AssignTeamMemberToEvent(Guid teamMemberId, Guid eventId, Guid passCategoryId, CancellationToken cancellationToken)
    {
        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new AssignTeamMemberToEventSpec(teamMemberId, eventId, passCategoryId), cancellationToken, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        if (teamMember.TeamMemberEvents.Count > 0)
        {
            var teamMemberEvent = teamMember.TeamMemberEvents.Where(s => s.TeamMemberId == teamMemberId && s.EventId == eventId).First();

            if (teamMemberEvent.ActiveStatus is TeamMemberStatus.Active)
            {
                return new(new OperationFailedException(nameof(eventId), "This Team Member is already assigned to the current Event"));
            }

            teamMemberEvent.AddTeamMemberToEvent();
        }
        else
        {
            teamMember.AssignTeamMemberEvent(eventId, passCategoryId);
        }

        await _teamMemberRepository.SaveChangesAsync(token: cancellationToken);

        return new();
    }

    public async Task<ResponseResult> UpdateTeamMember(Guid teamMemberId, UpdateTeamMemberDto updateTeamMemberDto, CancellationToken cancellationToken)
    {
        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new AssignTeamMemberToEventSpec(teamMemberId, updateTeamMemberDto.EventId, updateTeamMemberDto.PassCategoryId), cancellationToken, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        var teamMemberEvent = teamMember.TeamMemberEvents.Where(s => s.TeamMemberId == teamMemberId && s.EventId == updateTeamMemberDto.EventId).FirstOrDefault();

        if (teamMemberEvent is not null) teamMemberEvent.AssignTeamMemberPassCategory(updateTeamMemberDto.PassCategoryId);

        teamMember.UpdateTeamMember(
            updateTeamMemberDto.FirstName,
            updateTeamMemberDto.LastName,
            updateTeamMemberDto.Title,
            updateTeamMemberDto.MobileNumber,
            updateTeamMemberDto.Email,
            updateTeamMemberDto.Address,
            updateTeamMemberDto.CountryId,
            updateTeamMemberDto.NicNumber,
            updateTeamMemberDto.PassportNumber,
            updateTeamMemberDto.CompanyName,
            updateTeamMemberDto.IdentityType
        );

        await _teamMemberRepository.SaveChangesAsync(token: cancellationToken);

        return new();
    }

    public async Task<ResponseResult<FileDto>> AddTeamMemberProfileImage(Guid teamMemberId, IFormFile file, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync<ImageFileValidator, IFormFile>(file, token);

        if (validationResult.IsValid is false) return new ResponseResult<FileDto>(validationResult.Errors);

        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberByIdSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        using var stream = file.OpenReadStream();

        var compressedStream = ImageHelper.CompressImage(stream);

        var documentUploadResponse = await _fileRespository.UploadFile(compressedStream,
                                                                       documentName: file.FileName,
                                                                       contentType: file.ContentType,
                                                                       containerName: AppConstants.BlobStorage.ContainerName.TeamMemberProfileImage,
                                                                       folderPath: null,
                                                                       cancellationToken: token);

        if (documentUploadResponse.Success is false) return new(new BadRequestException("FileUpload", $"File ({file.FileName}) failed to upload"));

        var uploadedFileInfo = documentUploadResponse.Data!;

        teamMember.SetImageUrl(uploadedFileInfo.URI);

        await _teamMemberRepository.SaveChangesAsync(token);

        FileDto documentDtos = new()
        {
            Id = uploadedFileInfo.URI,
            FileName = uploadedFileInfo.FileName,
            UniqueName = uploadedFileInfo.UniqueName,
            URI = uploadedFileInfo.URI,
            RelatedEntityId = teamMemberId
        };

        return new(documentDtos);
    }

    public async Task<ResponseResult> DeleteTeamMemberProfileImage(Guid teamMemberId, CancellationToken token)
    {
        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberByIdSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        teamMember.RemoveProfileImage();

        await _teamMemberRepository.SaveChangesAsync(token);

        return new();
    }

    public async Task<ResponseResult<IReadOnlyCollection<FileDto>>> GetTeamMemberDocuments(Guid teamMemberId, CancellationToken token)
    {
        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberByIdSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        var teamMemberWithDocuments = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentSpec(teamMemberId), token);

        if (teamMemberWithDocuments is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        var teamMemberDocumentsDto = teamMemberWithDocuments.Attachments.Select(s => new TeamMemberDocumentDto(s.Id, s.TeamMemberId, s.AttachmentURL, s.AttachmentType)).ToList();

        return new(teamMemberWithDocuments.Attachments.Select(s => new FileDto
        {
            Id = s.Id.ToString(),
            ExtenstionData = new { s.AttachmentType },
            FileName = s.DisplayName,
            RelatedEntityId = s.TeamMemberId,
            UniqueName = s.UniqueName,
            URI = s.AttachmentURL
        }).ToList().AsReadOnly());
    }

    public async Task<ResponseResult<IReadOnlyList<FileDto>>> UploadTeamMemberDocuments(Guid teamMemberId, List<KeyValuePair<AttachmentType, IFormFile>> files, CancellationToken token)
    {
        if (files.Select(s => s.Key).Any(a => a is AttachmentType.None))
            return new(new BadRequestException(nameof(AttachmentType), $"One or more document type is having the Key {nameof(AttachmentType.None)}"));

        var docs = files.Select(s => s.Value).ToList();

        var validationResult = await _validator.ValidateAsync<ImageFileCollectionValidator, IEnumerable<IFormFile>>(docs, token);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentUploadSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

        List<FileDto> fileListDto = new();

        if (files is null or { Count: 0 }) return new(fileListDto);

        foreach (var file in files)
        {
            var doc = file.Value;
            using var stream = doc.OpenReadStream();

            var compressedStream = ImageHelper.CompressImage(stream);

            var documentUploadResponse = await _fileRespository.UploadFile(compressedStream,
                                                                           documentName: doc.FileName,
                                                                           contentType: doc.ContentType,
                                                                           containerName: AppConstants.BlobStorage.ContainerName.TeamMemberDocuments,
                                                                           folderPath: teamMemberId.ToString(),
                                                                           cancellationToken: token);

            if (documentUploadResponse.Success is false) return new(new BadRequestException("FileUpload", $"File ({doc.FileName}) failed to upload"));

            var uploadedFileInfo = documentUploadResponse.Data!;

            FileDto documentDto = new()
            {
                Id = uploadedFileInfo.URI,
                FileName = uploadedFileInfo.FileName,
                UniqueName = uploadedFileInfo.UniqueName,
                URI = uploadedFileInfo.URI,
                RelatedEntityId = teamMemberId,
                ExtenstionData = new { AttachmentType = file.Key }
            };

            fileListDto.Add(documentDto);

            teamMember.AddDocument(uploadedFileInfo.URI, file.Key, displayName: documentDto.FileName, uniqueName: documentDto.UniqueName);
        }

        await _teamMemberRepository.SaveChangesAsync(token);

        return new(new List<FileDto>(fileListDto.AsReadOnly()));
    }





    public async Task<ResponseResult> MarkAsBlackListed(Guid teamMemberId, CancellationToken token)
    {
        //var validationResult = await _validator.ValidateAsync<MarkAsBlackListedDtoValidator, MarkAsBlackListedDto>(model, token);

        //if (validationResult.IsValid is false) return new(validationResult.Errors);

        //var visitor = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentUploadSpec(teamMemberId)), token, asTracking: true);

        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentUploadSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "TeamMember", teamMemberId));


        if (teamMember.blacklistStatus is MemberBlacklistStatus.BlackListed) return new();

        //if (visitor.VisitorStatus is VisitorStatus.BlackListed) return new();

        teamMember.MarkAsBlacklisted();


        //var blacklistedDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");
        //var blacklistedUntil = model.BlackListUntil?.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat();

        //var description = $"{blacklistedDateAndTime} - Blacklisted {(blacklistedUntil is not null ? " until " + blacklistedUntil : string.Empty)}";

        //VisitorActivity activity = new(_loggedInUser.FacetsEventId,
        //visitor.Id,
        //                               description,
        //                               VisitorActivityType.MarkedAsBlacklisted);

        //_visitorRepository.AddVisitorActivity(activity);

        //await _visitorRepository.SaveChangesAsync(token);
        await _teamMemberRepository.SaveChangesAsync(token);

        return new();
    }

    //public async Task<ResponseResult> MarkAsBlackListed(Guid teamMemberId, CancellationToken token)
    //{

    //}




    //public async Task<ResponseResult> MarkAsBlackListed(Guid teamMemberId, CancellationToken cancellationToken)
    //{
    //    var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new AssignTeamMemberToEventSpec(teamMemberId, eventId, passCategoryId), cancellationToken, asTracking: true);

    //    if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "Team Member", teamMemberId));

    //    if (teamMember.TeamMemberEvents.Count > 0)
    //    {
    //        var teamMemberEvent = teamMember.TeamMemberEvents.Where(s => s.TeamMemberId == teamMemberId && s.EventId == eventId).First();

    //        if (teamMemberEvent.ActiveStatus is TeamMemberStatus.Active)
    //        {
    //            return new(new OperationFailedException(nameof(eventId), "This Team Member is already assigned to the current Event"));
    //        }

    //        teamMemberEvent.AddTeamMemberToEvent();
    //    }
    //    else
    //    {
    //        teamMember.AssignTeamMemberEvent(eventId, passCategoryId);
    //    }

    //    await _teamMemberRepository.SaveChangesAsync(token: cancellationToken);

    //    return new();
    //}



    public async Task<ResponseResult> RemoveBlackListedMember(Guid teamMemberId, CancellationToken token)
    {
        //var validationResult = await _validator.ValidateAsync<MarkAsBlackListedDtoValidator, MarkAsBlackListedDto>(model, token);

        //if (validationResult.IsValid is false) return new(validationResult.Errors);

        //var visitor = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentUploadSpec(teamMemberId)), token, asTracking: true);

        var teamMember = await _teamMemberRepository.GetTeamMemberBySpec(new TeamMemberDocumentUploadSpec(teamMemberId), token, asTracking: true);

        if (teamMember is null) return new(new NotFoundException(nameof(teamMemberId), "TeamMember", teamMemberId));


        if (teamMember.blacklistStatus is MemberBlacklistStatus.None) return new();

        //if (visitor.VisitorStatus is VisitorStatus.BlackListed) return new();

        teamMember.RemoveBlacklisted();


        //var blacklistedDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");
        //var blacklistedUntil = model.BlackListUntil?.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat();

        //var description = $"{blacklistedDateAndTime} - Blacklisted {(blacklistedUntil is not null ? " until " + blacklistedUntil : string.Empty)}";

        //VisitorActivity activity = new(_loggedInUser.FacetsEventId,
        //visitor.Id,
        //                               description,
        //                               VisitorActivityType.MarkedAsBlacklisted);

        //_visitorRepository.AddVisitorActivity(activity);

        //await _visitorRepository.SaveChangesAsync(token);
        await _teamMemberRepository.SaveChangesAsync(token);

        return new();
    }









}
