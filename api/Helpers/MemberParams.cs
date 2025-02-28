namespace api.Helpers;

public class MemberParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    public string OrderBy { get; set; } = "lastActive";
    public string? Search { get; set; } = String.Empty;
    public string? Role { get; set; } = String.Empty;
}