namespace api.DTOs.Team;

public record TeamWithRoleDto(
    string Name,
    IEnumerable<string> Roles    
);