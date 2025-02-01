namespace api.DTOs.Team_DTOs;

public record CreateTeamDto(
    ObjectId CreatorId,
    string CreatorUserName,
    string TeamName
    );