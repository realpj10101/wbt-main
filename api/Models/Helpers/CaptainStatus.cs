namespace api.Models.Helpers;

public class CaptainStatus
{
    public bool NotInTeam { get; set; }
    public bool NotTeamMember { get; set; }
    public bool UserNotFound { get; set; }
    public bool AlreadyCaptain { get; set; }
    public bool TeamNotExist { get; set; }
    public bool CoachNotFound { get; set; }
    public bool CoachHasNoTeam { get; set; }
    public bool IsSuccess { get; set; }
}