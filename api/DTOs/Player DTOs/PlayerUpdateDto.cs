namespace api.DTOs;

public record PlayerUpdateDto(
    string Name,
    string LastName,
    int Height,
    int Weight,
    string Gender,
    string Position,
    string ExperienceLevel,
    string Skills,
    int GamesPlayed,
    float PointsPerGame,
    float ReboundsPerGame,
    float AssistsPerGame,
    string Bio,
    string Achievements,
    [Length(3, 20)]string City,
    [Length(3, 20)]string Region,
    [Length(3, 20)]string Country
);