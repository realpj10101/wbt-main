namespace api.DTOs;

public record CoachDto(
    // string Id,
    string Email,
    string Name,
    string LastName,
    string NationalCode,
    int Age,
    string KnownAs,
    DateTime Created,
    DateTime Lastactive,
    string Gender,
    string? LookingFor,
    string City,
    string Country,
    List<Photo> Photos,
    string Records,
    bool IsFollowing,
    bool IsAdmin
);
