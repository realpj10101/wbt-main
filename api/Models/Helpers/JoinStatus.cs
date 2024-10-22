namespace api.Models.Helpers;

public class JoinStatus
{
    public bool IsSuccess { get; set; }
    public bool IsAlreadyJoined { get; set; }
    public bool IsAlreadyLeft { get; set; }
    public bool IsJoiningThemself { get; set; }
    public bool IsTargetTeamNotFound { get; set; }
}