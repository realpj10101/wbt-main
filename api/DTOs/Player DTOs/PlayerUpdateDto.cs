namespace api.DTOs;

public record PlayerUpdateDto(
    string Name,
    string LastName,
    int Height,
    int Weight,
    string Gender,
    string ExperienceLevel,
    string Skills,
    string GamesPlayed,
    string PointsPerGame,
    string Rebounds,
    [Length(3, 20)]string City,
    [Length(3, 20)]string Country
);