namespace api.DTOs;

public record CoachWithRoleDto(
    string UserName,
    IEnumerable<string> Roles
);
