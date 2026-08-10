namespace Facets.Core.Security.Dtos;

public record UpdateRoleDto(string RoleName, IEnumerable<string> Permissions);
