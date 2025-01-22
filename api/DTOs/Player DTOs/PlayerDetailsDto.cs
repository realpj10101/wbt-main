namespace api.DTOs;

public record PlayerDetailsDto(
    string Name,
    string LastName,
    string NationalCode,
    int Height,
    string? KnownAs,
    string? LookingFor,
    string? Records,
    string City,
    string Country
);
    