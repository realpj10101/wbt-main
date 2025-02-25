using api.Enums;

namespace api.Helpers;

public class TeamParams : PaginationParams
{
    public ObjectId? UserId { get; set; }
    public TeamPredicateEnum Predicate { get; set; }
}