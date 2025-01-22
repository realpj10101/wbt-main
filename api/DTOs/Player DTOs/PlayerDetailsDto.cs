namespace api.DTOs;

public record PlayerDetailsDto(
    string Name,
    string LastName,
    string NationalCode,
    int Height,
    DateOnly Age,
    string? KnownAs,
    string? LookingFor,
    string? Records,
    string City,
    string Country
);
    