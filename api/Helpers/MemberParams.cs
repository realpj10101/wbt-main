namespace api.Helpers;

public class MemberParams
{
    public ObjectId? UserId { get; set; }
    public string OrderBy { get; set; } = "lastActive";
}