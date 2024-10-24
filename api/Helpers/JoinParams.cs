namespace api.Helpers;

public class JoinParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    
    public string Predicate { get; set; } = string.Empty;
}