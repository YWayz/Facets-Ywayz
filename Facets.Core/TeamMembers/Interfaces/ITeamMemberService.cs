using Facets.Core.Common.Dtos;
using Facets.Core.TeamMembers.DTOs;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberService
{
    Task<ResponseResult<CreatedTeamMemberDto>> CreateTeamMember(CreateTeamMemberDto model, CancellationToken token);
    Task<ResponseResult> DeleteTeamMemberDocument(Guid teamMemberId, Guid documentId, CancellationToken cancellationToken);
    Task<ResponseResult<IReadOnlyList<FileDto>>> UploadTeamMemberDocuments(Guid teamMemberId, List<KeyValuePair<AttachmentType, IFormFile>> files, CancellationToken token);
    Task<ResponseResult> AssignTeamMemberToEvent(Guid teamMemberId, Guid eventId, Guid passCategoryId, CancellationToken cancellationToken);
    Task<ResponseResult<FileDto>> AddTeamMemberProfileImage(Guid teamMemberId, IFormFile file, CancellationToken token);
    Task<ResponseResult> DeleteTeamMemberProfileImage(Guid teamMemberId, CancellationToken token);
    Task<ResponseResult<TeamMemberSearchDto>> SearchMember(string? searchValue, CancellationToken token);
    Task<ResponseResult<IReadOnlyCollection<FileDto>>> GetTeamMemberDocuments(Guid teamMemberId, CancellationToken token);
    Task<ResponseResult<TeamMemberDto>> GetTeamMemberById(Guid teamMemberId, CancellationToken token);    
    Task<ResponseResult> UpdateTeamMember(Guid teamMemberId, UpdateTeamMemberDto updateTeamMemberDto, CancellationToken cancellationToken);
    Task<ResponseResult> MarkAsBlackListed(Guid visitorId, CancellationToken token);

    Task<ResponseResult> RemoveBlackListedMember(Guid teamMemberId, CancellationToken token);

}
