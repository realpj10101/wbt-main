using api.Enums;

namespace api.DTOs.Team_DTOs;

public record CreateTeamDto(
    [MaxLength(50)]string TeamName,
    [MaxLength(20)]string TeamLevel,
    [MaxLength(50)]string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost,
    DateTime CreatedAt
    );

public record EnrolledTeam(
    ObjectId TeamId
);

public class ShowTeamDto
{
    public string TeamName { get; init; } = string.Empty;
    public List<string> MembersUserNames { get; init; } = [];
    public string TeamLevel { get; init; } = string.Empty;
    public string Achievements { get; init; } = string.Empty;
    public int GamesPlayed { get; init; }
    public int GamesWon { get; init; }
    public int GamesLost { get; init; }
    public Status Status { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public List<Photo> Photos { get; init; } = [];
}
