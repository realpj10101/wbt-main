namespace api.Models.Helpers;

public class TeamStatus
{
    public bool IsSuccess { get; set; }
    public bool IsAlreadyJoined { get; set; }
    public bool IsJoiningThemself { get; set; }
    public bool IsTargetMemberNotFound { get; set; }
}