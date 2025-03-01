namespace api.Helpers;

public class MemberParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    public string OrderBy { get; set; } = "lastActive";
    public string? Search { get; set; } = String.Empty;
    public int MinAge { get; set; } = 6;
    public int MaxAge { get; set; } = 100;
    public string? Gender { get; set; }
}