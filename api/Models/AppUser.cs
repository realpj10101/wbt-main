using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace api.Models;

[CollectionName("users")]
public class AppUser : MongoIdentityUser<ObjectId>
{
    public string? IdentifierHash { get; init; }    
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string NationalCode { get; init; } = string.Empty;
    public int Height { get; init; } = 0;
    public DateOnly Age { get; init; }
    public string KnownAs { get; init; } = string.Empty;
    public DateTime LastActive { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? LookingFor { get; init; }
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Records { get; init; } = string.Empty;
    public List<Photo> Photos { get; init; } = [];
    public int FollowingsCount { get; init; }
    public int FollowersCount { get; init; }
    public int LikingsCount { get; init; }
    public int LikersCount { get; init; }
    public bool IsCaptain { get; init; }
    public bool IsAdmin { get; init; }
    public DateTime FoundedDate { get; init; }
    public int NumberOfGames{ get; init;}
    public int NumberOfWins { get; init;}
    public int NumberOfLosses { get; init;}
    public string GameHistory { get; init; } = string.Empty;
    public string GameResults { get; init;} = string.Empty;
    public int JoinersCount { get; init; }
    
}
