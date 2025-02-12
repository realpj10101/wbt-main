namespace api.DTOs;

public record TestPlayer(
    [MaxLength(50)]string? Name,
    [MaxLength(50)]string? LastName
    // string Gender
    );