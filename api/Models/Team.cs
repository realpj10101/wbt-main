using System.Runtime.InteropServices;

namespace api.Models;

public record Team(
    [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
    ObjectId CreatorId,
    ObjectId[] MemberId, // Basic Details
    string TeamName
    // Photo TeamLogo,
    // string TeamLevel, // Professional Details
    // string Achievements,
    // int GamesPlayed,
    // int GamesWon,
    // int GamesLost,
    // string UpcomingMatches, // Team Schedule
    // string PracticeSession,
    // string Description, // Additional Details
    // DateTime CreatedAt // History
    );