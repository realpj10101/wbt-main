namespace api.DTOs;

public record PlayerUpdateDto(
    int Height,
    string  KnownAs,
    string? LookingFor,
    [Length(3, 20)]string City,
    [Length(3, 20)]string Country
);