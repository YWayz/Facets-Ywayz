using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberFileUploadDto
{
    public List<KeyValuePair<AttachmentType, IFormFile>> Files { get; init; } = new();
}
