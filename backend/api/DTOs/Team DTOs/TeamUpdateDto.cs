namespace api.DTOs.Team;

public record TeamUpdateDto(
    int NumberOfGames,
    int NumberOfWins,
    int NumberOfLosses,
    string GameHistory,
    string GameResults
);