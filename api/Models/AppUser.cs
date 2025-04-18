using api.DTOs.Team_DTOs;
using api.Enums;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MongoDbGenericRepository.Attributes;

namespace api.Models;

[CollectionName("users")]
public class AppUser : MongoIdentityUser<ObjectId>
{
    public string? IdentifierHash { get; init; }    
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int Height { get; init; }
    public int Weight { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public DateTime LastActive { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int ExperienceYears { get; init; }
    public string Specialization { get; init; } = string.Empty;
    public List<Photo> Certifications { get; init; } = [];
    public string TeamsManaged  { get; init; } = string.Empty;
    public string CurrentTeam { get; init; } = string.Empty;
    public string TrainingStyle { get; init; } = string.Empty;
    public string PreferredPlayers { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string? JtiValue { get; init; }
    // public PositionsEnum Position { get; init; }
    public string ExperienceLevel { get; init; } = string.Empty;
    public string Skills { get; init; } = string.Empty;
    public int GamesPlayed { get; init; }
    public float PointsPerGame { get; init; }
    public float ReboundsPerGame { get; init; }
    public float AssistsPerGame { get; init; }
    public string Bio { get; init; } = string.Empty;
    public string Achievements { get; init; } = string.Empty;
    public List<Photo> Photos { get; init; } = [];
    public ObjectId? EnrolledTeam { get; set; }
    public int FollowingsCount { get; init; }
    public int FollowersCount { get; init; }
    public int LikingsCount { get; init; }
    public int LikersCount { get; init; }
    public bool IsCaptain { get; init; }    
    public int CommentingCount { get; init; }
    public int CommentersCount { get; init; }
}
