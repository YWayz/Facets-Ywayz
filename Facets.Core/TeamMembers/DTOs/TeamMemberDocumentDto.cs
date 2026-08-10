using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberDocumentDto(Guid Id, Guid TeamMemberId, string AttachmentURL, AttachmentType AttachmentType);
