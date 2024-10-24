namespace api.DTOs;

public record PlayerDto(
    // string Id,
    string UserName,
    string Name,
    string LastName,
    string NationalCode,
    int Height,
    int Age,
    string KnownAs,
    DateTime Created,
    DateTime LastActive,
    string Gender,
    string? LookingFor,
    string City,
    string Country,
    List<Photo> Photos,
    bool IsFollowing,
    bool IsCaptain
);
