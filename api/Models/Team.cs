using System.Runtime.InteropServices;
using api.Enums;

namespace api.Models;

public record Team(
    [Optional]
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    ObjectId Id,
    ObjectId CreatorId,
    List<ObjectId> MembersIds, // Basic Details
    string TeamName,
    string TeamLevel, // Professional Details
    ObjectId TeamCaptainId,
    string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost,
    Status Status,
    DateTime CreatedAt,
    string RejectionReason,
    List<Photo> Photos
    // History
    // List<Photo> TeamLogo
    // string UpcomingMatches, // Team Schedule
    // string PracticeSession,
    // string Description, // Additional Details
);