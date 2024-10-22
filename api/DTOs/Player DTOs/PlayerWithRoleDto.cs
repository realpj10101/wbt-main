namespace api.DTOs;

public record PlayerWithRoleDto(
    string UserName,
    IEnumerable<string> Roles
);

