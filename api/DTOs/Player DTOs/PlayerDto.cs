namespace api.DTOs;

public record PlayerDto(
    // string Id,
    string UserName,
    string Name,
    string LastName,
    int Height,
    int Age,
    DateTime Created,
    DateTime LastActive,
    string Gender,
    string City,
    string Country,
    List<Photo> Photos,
    bool IsFollowing,
    bool IsCaptain
);
