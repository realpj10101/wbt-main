using api.Enums;

namespace api.DTOs;

public record PlayerDto(
    // string Id,
    string UserName,
    string? Name,
    string? LastName,
    int? Age,
    int? Height,
    int? Weight,
    PositionsEnum? Position,
    string ExperienceLevel,
    string Skills,
    int GamesPlayed,
    float PointsPerGame,
    float ReboundsPerGame,
    float AssistsPerGame,
    string Bio,
    string Achievements,
    DateTime Created,
    DateTime LastActive,
    string? Gender,
    string City,
    string Region,
    string Country,
    List<Photo> Photos,
    bool IsFollowing,
    bool IsCaptain,
    bool IsLiking,
    bool IsAccepted,
    bool IsInTeam
);
