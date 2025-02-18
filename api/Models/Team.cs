using System.Runtime.InteropServices;

namespace api.Models;

public record Team(
    [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
    ObjectId CreatorId,
    List<ObjectId> MembersIds, // Basic Details
    List<string> MembersUserNames,
    string TeamName,
    string TeamLevel, // Professional Details
    string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost,
    DateTime CreatedAt // History
    // List<Photo> TeamLogo
    // string UpcomingMatches, // Team Schedule
    // string PracticeSession,
    // string Description, // Additional Details
    );