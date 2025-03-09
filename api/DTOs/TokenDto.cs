namespace api.DTOs;

public record TokenDto(
    string AccessToken,
    string RefreshToken
);