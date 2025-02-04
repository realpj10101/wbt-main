namespace api.DTOs.Team_DTOs;

public record CreateTeamDto(
    string TeamName,
    string TeamLevel,
    string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost,
    DateTime CreatedAt
    );

public class ShowTeamDto
{
    public string TeamName { get; init; } = string.Empty;
    public List<ObjectId> MembersIds { get; init; } = [];
    public string TeamLevel { get; init; } = string.Empty;
    public string Achievements { get; init; } = string.Empty;
    public int GamesPlayed { get; init; }
    public int GamesWon { get; init; }
    public int GamesLost { get; init; }
    public DateTime CreatedAt { get; init; }
}