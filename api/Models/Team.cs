using System.Runtime.InteropServices;

namespace api.Models;

public record Team(
    [Optional][property: BsonId, BsonRepresentation(BsonType.ObjectId)] ObjectId Id,
    string CreatorUserName,
    string TeamName,
    Photo TeamLogo,
    string TeamLevel,
    string Achievements,
    int GamesPlayed,
    int GamesWon,
    int GamesLost,
    string UpcomingMatches,
    string PracticeSession,
    string Description,
    DateTime CreatedAt
    );