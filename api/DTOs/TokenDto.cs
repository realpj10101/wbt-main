namespace api.DTOs;

public record TokenDto(
    string AccessToken,
    RefreshTokenResponse RefreshTokenResponse
);