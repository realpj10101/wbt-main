namespace api.DTOs.Team;

public record TeamDto(
    string UserName,
    DateTime FoundedDate,
    int NumberOfGames,
    int NumberOfWins,
    int NumberOfLosses,
    string GameHistory,
    string GameResults,
    string City,
    string Country,
    string Records,
    List<Photo> Photos,
    bool IsJoining
);