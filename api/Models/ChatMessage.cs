namespace api.Models;

public record ChatMessage(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    string? Id, // hamishe sabet
    string UserName,
    string Message,
    DateTime TimeStamp
);