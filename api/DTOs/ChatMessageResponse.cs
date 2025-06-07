namespace api.DTOs;

public record ChatMessageResponse (
  string SenderUserName,
  string Message,
  DateTime TimeStamp  
);