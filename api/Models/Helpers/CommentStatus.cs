namespace api.Models.Helpers;

public class CommentStatus
{
    public bool IsSuccess { get; set; }
    public bool IsTargetMemberNotFound { get; set; }
    public bool IsAlreadyDeleted { get; set; }
}