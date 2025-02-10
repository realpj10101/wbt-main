namespace api.Helpers;

public class MemberParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    public string OrderBy { get; set; } = "lastActive";
}