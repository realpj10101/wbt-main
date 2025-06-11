namespace api.DTOs;

public record OnlineUserDto(
    string UserName,
    DateTimeOffset LastActive
);