namespace api.DTOs.Team_DTOs;

public record UpdateTeamDto(
    string UserName,
    string TeamName,
    string TeamLevel,
    string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost
    );